using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Elurnity.Unity.Editor
{
    public class ExecuteInternalMono
    {
        public static ProcessStartInfo GetProfileStartInfoForMono(string monodistribution, string profile, string executable, string arguments, bool setMonoEnvironmentVariables)
        {
            var monoexe = PathExtensions.PathCombine(monodistribution, "bin", "mono");
            var profileAbspath = PathExtensions.PathCombine(monodistribution, "lib", "mono", profile);
            if (Application.platform == RuntimePlatform.WindowsEditor)
                monoexe = PrepareFileName(monoexe + ".exe");

            var startInfo = new ProcessStartInfo
            {
                Arguments = PrepareFileName(executable) + " " + arguments,
                CreateNoWindow = true,
                FileName = monoexe,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Application.dataPath + "/..",
                UseShellExecute = false
            };

            if (setMonoEnvironmentVariables)
            {
                startInfo.EnvironmentVariables["MONO_PATH"] = profileAbspath;
                startInfo.EnvironmentVariables["MONO_CFG_DIR"] = PathExtensions.PathCombine(monodistribution, "etc");
            }
            return startInfo;
        }

        private static readonly Regex UnsafeCharsWindows = new Regex("[^A-Za-z0-9\\_\\-\\.\\:\\,\\/\\@\\\\]");
        private static readonly Regex UnescapeableChars = new Regex("[\\x00-\\x08\\x10-\\x1a\\x1c-\\x1f\\x7f\\xff]");
        private static readonly Regex Quotes = new Regex("\"");

        private static string PrepareFileName(string input)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EscapeCharsQuote(input);
            }
            return EscapeCharsWindows(input);
        }

        private static string EscapeCharsQuote(string input)
        {
            if (input.IndexOf('\'') == -1)
            {
                return "'" + input + "'";
            }
            if (input.IndexOf('"') == -1)
            {
                return "\"" + input + "\"";
            }
            return null;
        }

        private static string EscapeCharsWindows(string input)
        {
            if (input.Length == 0)
            {
                return "\"\"";
            }
            if (UnescapeableChars.IsMatch(input))
            {
                UnityEngine.Debug.LogWarning("Cannot escape control characters in string");
                return "\"\"";
            }
            if (UnsafeCharsWindows.IsMatch(input))
            {
                return "\"" + Quotes.Replace(input, "\"\"") + "\"";
            }
            return input;
        }
    }
}
