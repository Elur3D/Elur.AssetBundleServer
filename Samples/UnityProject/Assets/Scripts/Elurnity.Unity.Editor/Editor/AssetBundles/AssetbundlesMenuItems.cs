using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Elurnity.AssetBundles
{
    public static class AssetBundlesMenuItems
    {
        const string kPrefix = "Elurnity/AssetBundles/";
        const string kSimulationMode        = kPrefix + "Simulation Mode";
        const string kLocalServerMenu       = kPrefix + "Local server";
        const string kLocalAssetsMenu       = kPrefix + "Local bundles";
        const string kLocalServerRunMenu    = kPrefix + "Run local asset server";
        const string kBuildBundles          = kPrefix + "Build AssetBundles";
        const string kInStreammingAssets    = kPrefix + "Copy to StreamingAssets";
        const string kUploadAssetBundles    = kPrefix + "Upload asset bundles";
        const string kBuildPlayer           = "Build player";
        const string kBuildPlayerAndAssets  = "Build player and bundles %&b";

        [MenuItem(kSimulationMode, false, 10)]
        public static void ToggleSimulationMode()
        {
            InEditorMode = Mode.Simulation;
        }

        [MenuItem(kSimulationMode, true, 10)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kSimulationMode, InEditorMode == Mode.Simulation);
            return true;
        }

        [MenuItem(kLocalServerMenu, false, 11)]
        public static void ToggleLocalAssetBundleServer()
        {
            InEditorMode = Mode.LocalServer;
        }

        [MenuItem(kLocalServerMenu, true, 11)]
        public static bool ToggleLocalAssetBundleServerValidate()
        {
            Menu.SetChecked(kLocalServerMenu, InEditorMode == Mode.LocalServer);
            return true;
        }

        [MenuItem(kLocalAssetsMenu, false, 12)]
        public static void ToggleLocalAssetBundles()
        {
            InEditorMode = Mode.LocalAssets;
        }

        [MenuItem(kLocalAssetsMenu, true, 12)]
        public static bool ToggleLocalAssetBundlesValidate()
        {
            Menu.SetChecked(kLocalAssetsMenu, InEditorMode == Mode.LocalAssets);
            return true;
        }

        [MenuItem(kInStreammingAssets, false, 30)]
        public static void CopyToStreammingAssets()
        {
            ToStreammingAssets = !ToStreammingAssets;
        }

        [MenuItem(kInStreammingAssets, true, 30)]
        public static bool CopyToStreammingAssetsValidate()
        {
            Menu.SetChecked(kInStreammingAssets, ToStreammingAssets);
            return true;
        }

        [MenuItem(kLocalServerRunMenu, false, 31)]
        public static void LocalAssetBundleServer()
        {
            LaunchAssetBundleServer.instance.Running = !LaunchAssetBundleServer.instance.Running;
        }

        [MenuItem(kLocalServerRunMenu, true, 31)]
        public static bool LocalAssetBundleServerValidate()
        {
            Menu.SetChecked(kLocalServerRunMenu, LaunchAssetBundleServer.instance.Running);
            return true;
        }

        [MenuItem(kUploadAssetBundles, false, 32)]
        public static void UploadAssetBundles()
        {
            TimeManager.EditorTimeManager.Add(AssetBundleUploader.UploadCoroutine());
        }

        [MenuItem(kPrefix + kBuildPlayer, false, 60)]
        static public void BuildStandalonePlayer()
        {
            BuildScript.BuildPlayer(ToStreammingAssets);
        }

        [MenuItem(kBuildBundles, false, 61)]
        static public void BuildAssetBundles()
        {
            BuildScript.BuildAssetBundles(ToStreammingAssets);
        }

        [MenuItem("File/" + kBuildPlayerAndAssets, false, 10)]
        [MenuItem(kPrefix + kBuildPlayerAndAssets, false, 62)]
        static public void BuildStandalonePlayerAndAssets()
        {
            BuildScript.BuildPlayerAndAssetBundles(ToStreammingAssets);
        }

        public enum Mode { Simulation, LocalServer, LocalAssets }

        const string kEditorPrefKey = "AssetBundlesInEditorMode";
        const string kCopyToStreamming = "AssetBundlesToStreammingAssets";

        // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        private static Mode? m_InEditorMode;
        public static Mode InEditorMode
        {
            get
            {
                return (m_InEditorMode ?? (m_InEditorMode = (Mode)EditorPrefs.GetInt(kEditorPrefKey, 0))).Value;
            }

            set
            {
                if (value != m_InEditorMode)
                {
                    m_InEditorMode = value;
                    int intValue = (int)value;
                    EditorPrefs.SetInt(kEditorPrefKey, intValue);
                }
            }
        }

        public static bool ToStreammingAssets
        {
            get
            {
                return EditorPrefs.GetInt(kCopyToStreamming, 0) > 0;
            }

            set
            {
                EditorPrefs.SetInt(kCopyToStreamming, value ? 1 : 0);
            }
        }
    }
}
