using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Elurnity.Unity.Editor
{
    public class ExecuteWithMono<T> : ScriptableSingleton<T> where T : ExecuteWithMono<T>
    {
        [SerializeField]
        int pid = 0;

        Process _process;

        Process process
        {
            get
            {
                if (_process == null && pid != 0)
                {
                    try
                    {
                        _process = Process.GetProcessById(pid);
                    }
                    catch
                    {
                    }
                }
                return _process;
            }

            set
            {
                _process = value;
            }
        }

        public bool Running
        {
            get
            {
                if (process == null)
                    return false;

                return !process.HasExited;
            }

            set
            {
                if (Running != value)
                {
                    if (value)
                    {
                        Start();
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
        }

        protected virtual string Profile
        {
            get
            {
                return "4.0";
            }
        }

        protected virtual MonoInstallationFinder.MonoInstalation MonoInstalation
        {
            get
            {
                return MonoInstallationFinder.MonoInstalation.MonoBleedingEdge;
            }
        }

        protected ProcessStartInfo MonoStartInfo(string executable, string args)
        {
            var instalation = MonoInstallationFinder.GetMonoInstallation(MonoInstalation);
            return ExecuteInternalMono.GetProfileStartInfoForMono(instalation, Profile, executable, args, true);
        }

        protected virtual ProcessStartInfo StartProcess()
        {
            var startInfo = MonoStartInfo("test.exe", "");
            startInfo.WorkingDirectory = Application.dataPath;
            startInfo.UseShellExecute = false;
            return startInfo;
        }

        public virtual void Start()
        {
            Stop();

            var startInfo = StartProcess();
            process = Process.Start(startInfo);
            if (process == null || process.HasExited == true || process.Id == 0)
            {
                //Unable to start process
                UnityEngine.Debug.LogError("Unable Start AssetBundleServer process");
            }
            else
            {
                //We seem to have launched, let's save the PID
                pid = process.Id;
            }
        }

        public virtual void Stop()
        {
            // Kill the last time we ran
            try
            {
                if (pid == 0)
                {
                    return;
                }

                var lastProcess = Process.GetProcessById(pid);
                lastProcess.Kill();
                pid = 0;
            }
            catch
            {
            }
        }
    }
}