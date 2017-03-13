using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;

namespace Elurnity.AssetBundles
{
    public static class Paths
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return "file://" + Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
            else if (Application.isWebPlayer)
                return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" +  Application.streamingAssetsPath;
        }
        
        public static string GetPlatformName()
        {
            #if UNITY_EDITOR
                return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
            #else
                return GetPlatformForAssetBundles(Application.platform);
            #endif
        }

        #if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.tvOS:
                    return "tvOS";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
        #endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.tvOS:
                    return "tvOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}
