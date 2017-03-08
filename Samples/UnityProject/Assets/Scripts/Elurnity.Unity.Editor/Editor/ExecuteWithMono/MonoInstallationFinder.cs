using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Elurnity.Unity.Editor
{
    public static class MonoInstallationFinder
    {
        public enum MonoInstalation
        {
            Mono,
            MonoBleedingEdge

        }

        public static string GetFrameWorksFolder()
        {
            var editorAppPath = EditorApplication.applicationPath;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                return Path.Combine(Path.GetDirectoryName(editorAppPath), "Data");
            else if (Application.platform == RuntimePlatform.OSXEditor)
                #if UNITY_5_4_OR_NEWER
                return Path.Combine(editorAppPath, "Contents");
                #else
                return Path.Combine(editorAppPath, Path.Combine("Contents", "Frameworks"));
                #endif
            else // Linux...?
            return Path.Combine(Path.GetDirectoryName(editorAppPath), "Data");
        }

        public static string GetProfileDirectory(BuildTarget target, string profile)
        {
            var monoprefix = GetMonoInstallation();
            return Path.Combine(monoprefix, Path.Combine("lib", Path.Combine("mono", profile)));
        }

        public static string GetMonoInstallation()
        {
            #if INCLUDE_MONO_2_12
            return GetMonoInstallation(MonoInstalation.MonoBleedingEdge);
            #else
            return GetMonoInstallation(MonoInstalation.Mono);
            #endif
        }

        public static string GetMonoInstallation(MonoInstalation instalation)
        {
            return Path.Combine(GetFrameWorksFolder(), instalation.ToString());
        }

        /*
        static string GetMonoProfileVersion()
        {
            string path = Path.Combine(Path.Combine(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "lib"), "mono");

            string[] folders = Directory.GetDirectories(path);
            string[] foldersWithApi = folders.Where(f => f.Contains("-api")).ToArray();
            float profileVersion = 1.0f;

            for (int i = 0; i < foldersWithApi.Length; i++)
            {
                foldersWithApi[i] = foldersWithApi[i].Split('\\').Last();
                foldersWithApi[i] = foldersWithApi[i].Split('-').First();

                if (float.Parse(foldersWithApi[i]) > profileVersion)
                {
                    profileVersion = float.Parse(foldersWithApi[i]);
                }
            }

            return profileVersion.ToString();
        }
        */
    }
}
