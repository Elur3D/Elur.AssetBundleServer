using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

public static class GitUtility
{
    public static string commit
    {
        get
        {
            return RunGit("rev-parse HEAD");
        }
    }

    public static string branch
    {
        get
        {
            return RunGit("rev-parse --abbrev-ref HEAD");
        }
    }

    public static string RunGit(string argument)
    {
        string gitCMD = Application.platform == RuntimePlatform.WindowsEditor ? "git.exe" : "git";

        var cmd = Process.Start(new ProcessStartInfo
        {
            Arguments = argument,
            CreateNoWindow = true,
            FileName = gitCMD,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            WorkingDirectory = Application.dataPath,
            UseShellExecute = false,
        });
        
        cmd.WaitForExit();

        if (cmd.ExitCode == 0)
        {
            return cmd.StandardOutput.ReadToEnd().Trim();
        }
        return null;
    }

    [MenuItem ("Elurnity/Git/Pull")]
    public static void RunPull()
    {
        UnityEngine.Debug.Log(RunGit("pull"));
    }

    [MenuItem ("Elurnity/Git/Status")]
    public static void RunStatus()
    {
        UnityEngine.Debug.Log(RunGit("status"));
    }

    [MenuItem ("Elurnity/Git/Branch name")]
    public static void ShowBranchName()
    {
        UnityEngine.Debug.Log(branch);
    }

    [MenuItem ("Elurnity/Git/Commit Id")]
    public static void ShowCommitId()
    {
        UnityEngine.Debug.Log(commit);
    }
}