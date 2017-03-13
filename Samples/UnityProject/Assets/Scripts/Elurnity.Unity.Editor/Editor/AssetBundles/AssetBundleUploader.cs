using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Elurnity.AssetBundles
{
    public static class AssetBundleUploader
    {
        public static IEnumerator UploadCoroutine()
        {
            var platform = Paths.GetPlatformName();
            var manifestPath = PathExtensions.PathCombine(BuildScript.AssetBundleDirectory, platform, platform);
            if (File.Exists(manifestPath))
            {
                var bundle = AssetBundle.LoadFromFile(manifestPath);
                if (bundle != null)
                {
                    var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    if (manifest != null)
                    {
                        string uploadFileName = manifestPath;
                        Hash128 hash;
                        BuildPipeline.GetHashForAssetBundle(uploadFileName, out hash);

                        // Upload manifest

                        var uploadCoroutine = UploadAssetBundle(platform, platform, hash);
                        while (uploadCoroutine != null && uploadCoroutine.MoveNext())
                        {
                            yield return null;
                        }

                        // Upload asset bundles

                        int i = 0;
                        var assetbundles = manifest.GetAllAssetBundles();

                        foreach (var assetBundleName in assetbundles)
                        {
                            
                            // Upload asset bundle

                            i++;
                            uploadFileName = assetBundleName;
                            hash = manifest.GetAssetBundleHash(uploadFileName);
                            UnityEngine.Debug.Log(i + "/" + assetbundles.Length * 2 + " " + assetBundleName + " " + hash);

                            uploadCoroutine = UploadAssetBundle(uploadFileName, platform, hash);
                            while (uploadCoroutine != null && uploadCoroutine.MoveNext())
                            {
                                yield return null;
                            }

                            // Upload asset bundle manifest

                            i++;
                            uploadFileName = assetBundleName + ".manifest";
                            BuildPipeline.GetHashForAssetBundle(uploadFileName, out hash);
                            UnityEngine.Debug.Log(i + "/" + assetbundles.Length * 2 + " " + uploadFileName + " " + hash);

                            uploadCoroutine = UploadAssetBundle(uploadFileName, platform, hash);
                            while (uploadCoroutine != null && uploadCoroutine.MoveNext())
                            {
                                yield return null;
                            }
                        }

                        UnityEngine.Debug.Log("Upload complete!");
                    }
                    bundle.Unload(true);
                }
            }
        }

        public static IEnumerator UploadAssetBundle(string assetBundle, string platform, Hash128 hash)
        {
            var assetBundlePath = PathExtensions.PathCombine(BuildScript.AssetBundleDirectory, platform, assetBundle);
            if (File.Exists(assetBundlePath))
            {
                var bytes = File.ReadAllBytes(assetBundlePath);
                if (bytes != null)
                {
                    var form = new WWWForm();
                    form.AddField("build", "development");
                    form.AddField("hash", hash.ToString());
                    form.AddField("platform", Paths.GetPlatformName());
                    form.AddField("assetbundle", assetBundle);
                    form.AddBinaryData("file", bytes, assetBundle);

                    using (var uploadRequest = UnityWebRequest.Post(ServerURL, form))
                    {
                        yield return uploadRequest.Send();

                        while (!uploadRequest.isDone && string.IsNullOrEmpty(uploadRequest.error))
                        {
                            yield return null;
                        }

                        if (uploadRequest.isError)
                        {
                            UnityEngine.Debug.LogError(uploadRequest.error);
                        }
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogError("File not found " + assetBundlePath);
            }
        }

        private static string ServerURL
        {
            get
            {
                var urlFile = Resources.Load("AssetBundleUploadServerURL") as TextAsset;
                return urlFile != null ? urlFile.text.Trim() : "http://localhost:3000";
            }
        }
    }
}