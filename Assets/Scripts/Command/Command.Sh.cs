using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

//Sh
public partial class Command
{
    //Excecute Sh: 一番最後に実行される
    //ExecuteCommandに吸収させた
    /*
    private void ExecuteSh(string command, DataReceivedEventHandler handler_data, DataReceivedEventHandler handler_error)
    {
        if (_IsExecuting) return;

        try
        {
            using (Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = ShFileName;
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = false;
                proc.StartInfo.WorkingDirectory = WorkingDirectory;
                //proc.StartInfo.RedirectStandardError = true;

                proc.OutputDataReceived += handler_data;
                proc.ErrorDataReceived += handler_error;

                _IsExecuting = true;
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                _IsExecuting = false;

                return;
            }
        }
        catch (Exception e)
        {
            string result = "[Error!!] " + e.Message;
            UnityEngine.Debug.Log(result);
            return;
        }
    }
    */
}
