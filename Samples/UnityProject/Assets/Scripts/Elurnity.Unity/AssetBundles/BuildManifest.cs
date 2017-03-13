using UnityEngine;
using UnityEngine.CloudBuild;
using System;
using System.Collections.Generic;

[Serializable]
public class BuildManifest
{
    [Serializable]
    public class AssetBundles
    {
        // list of bundles copied to Streaming Assets.
        public List<string> localBundles;

        // relative path within the Streaming Assets folder where local bundles are copied.
        public string localBundlesRelativePath;
    }

    // Commit or changelist built by UCB
    public string scmCommitId;

    // Name of the branch that was built
    public string scmBranch;

    // The Unity Cloud Build number corresponding to this build
    public string buildNumber;

    // The UTC timestamp when the build process was started
    public string buildStartTime;

    // The UCB project identifier
    public string projectId;

    // (iOS and Android only) The bundleIdentifier configured in Unity Cloud Build
    public string bundleId;

    // The version of Unity used by UCB to create the build
    public string unityVersion;

    // (iOS only) The version of XCode used to build the project
    public string xcodeVersion;

    // The name of the project build target that was built. Currently, this will correspond to the platform, as either "default-web”, “default-ios”, or “default-android".
    public string cloudBuildTargetName;

    public AssetBundles assetBundles;

    private BuildManifest _instance;
    public BuildManifest Instance
    {
        get
        {
            return _instance ?? (_instance = Load());
        }
    }

    private BuildManifest()
    {
    }

    private static BuildManifest Load()
    {
        var manifest = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
        if (manifest != null)
        {
            return JsonUtility.FromJson<BuildManifest>(manifest.text);
        }
        else
        {
            return new BuildManifest()
            {
                #if UNITY_EDITOR
                scmCommitId = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                scmBranch = "master",
                buildNumber = "Personal build",
                buildStartTime = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString(),
                projectId = UnityEditor.PlayerSettings.productName, //"com.yourproject.id";
                bundleId = UnityEditor.PlayerSettings.bundleIdentifier, //"com.yourbundle.id";
                unityVersion = Application.unityVersion,
                xcodeVersion = "5.5",
                #endif
            };
        }
    }

    public BuildManifest LoadFromUnityCloud(BuildManifestObject manifest)
    {
        return _instance = JsonUtility.FromJson<BuildManifest>(manifest.ToJson());
    }
}
