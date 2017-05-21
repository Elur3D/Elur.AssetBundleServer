using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Elurnity.AssetBundles
{
    public class BuildScript
    {
        public static string AndroidSdkRoot
        {
            get { return EditorPrefs.GetString("AndroidSdkRoot"); }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }

        public static string JdkRoot
        {
            get { return EditorPrefs.GetString("JdkPath"); }
            set { EditorPrefs.SetString("JdkPath", value); }
        }

        public static string AndroidNdkRoot
        {
            get { return EditorPrefs.GetString("AndroidNdkRoot"); }
            set { EditorPrefs.SetString("AndroidNdkRoot", value); }
        }

        static public string AssetBundleDirectory
        {
            get
            {
                return PathExtensions.PathCombine(Environment.CurrentDirectory, Paths.AssetBundlesOutputPath);
            }
        }

        static string AssetBundleManifestPath
        {
            get
            {
                return Path.Combine(AssetBundleDirectory, Paths.GetPlatformName()) + ".manifest";
            }
        }

        static public string CreateAssetBundleDirectory()
        {
            // Choose the output path according to the build target.
            string outputPath = AssetBundleDirectory;
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        }

        public static void CheckEnvironmentVariables()
        {
            var androidSDKEnv = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            var androidNDKEnv = Environment.GetEnvironmentVariable("ANDROID_NDK");
            var javaEnv = Environment.GetEnvironmentVariable("JAVA_HOME");

            if (string.IsNullOrEmpty(androidSDKEnv))
            {
               AndroidSdkRoot = androidSDKEnv;
            }

            if (string.IsNullOrEmpty(androidNDKEnv))
            {
               AndroidNdkRoot = androidNDKEnv;
            }

            if (string.IsNullOrEmpty(javaEnv))
            {
               JdkRoot = javaEnv;
            }
        }

        public static void BuildAssetBundles(bool copyToStreammingAssets = false)
        {
            CheckEnvironmentVariables();

            // Choose the output path according to the build target.
            string outputPath = CreateAssetBundleDirectory();

            var options = BuildAssetBundleOptions.None;

            bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
            #if UNITY_TVOS
            shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
            #endif
            if (shouldCheckODR)
            {
                #if ENABLE_IOS_ON_DEMAND_RESOURCES
                if (PlayerSettings.iOS.useOnDemandResources)
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                #endif
                #if ENABLE_IOS_APP_SLICING
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                #endif
            }
            else
            {
                options |= BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
            }

            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            BuildPipeline.BuildAssetBundles(outputPath, options, EditorUserBuildSettings.activeBuildTarget);

            if (copyToStreammingAssets)
            {
                BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Paths.AssetBundlesOutputPath));
                AssetDatabase.Refresh();
            }
        }

        public static void BuildPlayerAndAssetBundles()
        {
            BuildPlayerAndAssetBundles(false);
        }

        public static void BuildPlayerAndAssetBundles(bool copyToStreammingAssets)
        {
            // Build and copy AssetBundles.
            BuildScript.BuildAssetBundles(copyToStreammingAssets);

            BuildPlayer(copyToStreammingAssets);
        }

        public static void BuildPlayer()
        {
            BuildPlayer(false, "build");
        }

        public static void BuildPlayer(bool copyToStreammingAssets, string outputPath = null)
        {
            CheckEnvironmentVariables();

            outputPath = outputPath ?? EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            var target = EditorUserBuildSettings.activeBuildTarget;

            var options = BuildOptions.None | BuildOptions.AutoRunPlayer;

            if (EditorUserBuildSettings.development)
            {
                options |= BuildOptions.Development;
            }

            if (EditorUserBuildSettings.allowDebugging)
            {
                options |= BuildOptions.AllowDebugging;
            }

            if (EditorUserBuildSettings.connectProfiler)
            {
                options |= BuildOptions.ConnectWithProfiler;
            }

            BuildPlayer(outputPath, target, options);
        }

        private static void BuildPlayer(string outputPath, BuildTarget target, BuildOptions options)
        {
            UnityEngine.Debug.Log("BuildPlayer: BuildTarget " + target + ", BuildOptions " + options);

            string targetName = GetBuildTargetName(target);
            if (targetName == null)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                UnityEngine.Debug.Log("Nothing to build.");
                return;
            }

            #if UNITY_5_5_OR_NEWER
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = levels;
            buildPlayerOptions.locationPathName = PathExtensions.PathCombine(outputPath, targetName);
            buildPlayerOptions.assetBundleManifestPath = AssetBundleManifestPath;
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.options = options;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            #else
            BuildPipeline.BuildPlayer(levels, PathExtensions.PathCombine(outputPath, targetName), EditorUserBuildSettings.activeBuildTarget, options);
            #endif
        }

        public static void BuildPlayerCmd()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildOptions options = BuildOptions.None;

            string[] args = System.Environment.GetCommandLineArgs();
            bool found = false;
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.EndsWith("BuildPlayerCmd", StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    continue;
                }
                if (found)
                {
                    if (arg.StartsWith("BuildTarget.", StringComparison.CurrentCultureIgnoreCase))
                    {
                        BuildTarget paramTarget;
                        if (arg.Replace("BuildTarget.", string.Empty).TryParseEnum<BuildTarget>(out paramTarget))
                        {
                            target = paramTarget;
                        }
                    }

                    if (arg.StartsWith("BuildOptions.", StringComparison.CurrentCultureIgnoreCase))
                    {
                        BuildOptions optionsTarget;
                        if (arg.Replace("BuildOptions.", string.Empty).TryParseEnum<BuildOptions>(out optionsTarget))
                        {
                            options |= optionsTarget;
                        }
                    }

                    if (arg.StartsWith("-"))
                    {
                        break;
                    }
                }
            }

            UnityEngine.Debug.Log("BuildPlayerCmd: BuildTarget " + target + ", BuildOptions " + options);

            BuildPlayer("build", target, options);
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "test.apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "test.exe";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "test.app";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.tvOS:
                    return "tvOS";
                case BuildTarget.WebGL:
                    return "";
            // Add more build targets for your own.
                default:
                    UnityEngine.Debug.Log("Target not implemented.");
                    return null;
            }
        }

        static void CopyAssetBundlesTo(string outputPath)
        {
            // Clear streaming assets folder.
            if (System.IO.Directory.Exists(outputPath))
                FileUtil.DeleteFileOrDirectory(outputPath);
            Directory.CreateDirectory(outputPath);

            // Setup the source folder for assetbundles.
            var source = AssetBundleDirectory;
            if (!System.IO.Directory.Exists(source))
                UnityEngine.Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

            // Setup the destination folder for assetbundles.
            var destination = System.IO.Path.Combine(outputPath, Paths.GetPlatformName());
            if (System.IO.Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(destination);

            FileUtil.CopyFileOrDirectory(source, destination);
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }
    }
}
