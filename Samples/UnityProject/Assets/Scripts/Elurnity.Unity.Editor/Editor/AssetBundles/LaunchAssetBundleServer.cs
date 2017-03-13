using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Elurnity.Unity.Editor;

namespace Elurnity.AssetBundles
{
    [InitializeOnLoad]
    internal class LaunchAssetBundleServer : ExecuteWithMono<LaunchAssetBundleServer>
    {
        const string ExecutableName = "AssetBundleServer";

        protected string executable
        {
            get
            {
                foreach (var guid in AssetDatabase.FindAssets(ExecutableName))
                {
                    var asset = AssetDatabase.GUIDToAssetPath(guid);
                    if (asset.EndsWith(ExecutableName + ".exe"))
                    {
                        return Path.GetFullPath(asset);
                    }
                }
                return null;
            }
        }

        protected override ProcessStartInfo StartProcess()
        {
            if (string.IsNullOrEmpty(executable))
            {
                UnityEngine.Debug.LogError(ExecutableName + " not found");
                return null;
            }

            BuildScript.CreateAssetBundleDirectory();
            string assetBundlesDirectory = Path.Combine(Environment.CurrentDirectory, "AssetBundles");
            string args = string.Format("\"{0}\" {1}", assetBundlesDirectory, Process.GetCurrentProcess().Id);
            var startInfo = MonoStartInfo(executable, args);
            startInfo.WorkingDirectory = assetBundlesDirectory;
            startInfo.UseShellExecute = false;
            return startInfo;
        }

        private string _URL = "";
        public string URL
        {

            get
            {
                if (string.IsNullOrEmpty(_URL))
                {
                    IPHostEntry host;
                    string localIP = "";
                    host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localIP = ip.ToString();
                            break;
                        }
                    }
                    _URL = "http://" + localIP + ":7888/";
                }
                return _URL;
            }
        }

        static LaunchAssetBundleServer()
        {
            /*
            AssetBundleManager.onInitialize += () =>
            {
                switch (AssetBundlesMenuItems.InEditorMode)
                {
                    case AssetBundlesMenuItems.Mode.Simulation:
                        {
                            AssetBundleManager.Instance.BaseDownloadingURL = "simulate://";
                            break;
                        }
                    case AssetBundlesMenuItems.Mode.LocalAssets:
                        {
                            // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
                            //AssetBundleManager.Instance.SetSourceAssetBundleDirectory(Application.dataPath + "/");
                            AssetBundleManager.Instance.BaseDownloadingURL = "file://" + BuildScript.AssetBundleDirectory;
                            break;
                        }
                    case AssetBundlesMenuItems.Mode.LocalServer:
                        {
                            instance.Running = true;
                            AssetBundleManager.Instance.SetSourceAssetBundleURL(instance.URL);
                            break;
                        }
                }
            };
            */
        }
    }
}
