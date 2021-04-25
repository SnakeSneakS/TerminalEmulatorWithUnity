using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

//Path
public partial class Command 
{
    //ExecuteFile: bin/bashなど、ファイルでコマンドを実行する。見つからなかった場合はShFileNameを実行する。
    private void ExecuteFile(string command, DataReceivedEventHandler handler_data, DataReceivedEventHandler handler_error)
    {
        if (_IsExecuting) return;
        if (command == "") return;

        string[] command_split = command.Split(' ');
        string fileName = FindFilesUnderPATH( command_split[0] );
        string arguments = (command_split.Length>1) ? command_split[1]: "";

        if (fileName == null){
            fileName = ShFileName;
            arguments = "-c \" " + command + " \"";
        }
        

        try
        {
            using (Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.RedirectStandardInput = true;
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

    private string FindFilesUnderPATH(string commandFlag)
    {
        foreach (string directory in PATH.Split(':'))
        {
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, commandFlag);
                if (files.Length != 0)
                {
                    UnityEngine.Debug.Log("EXECUTE FILE: " + files[0]);
                    return files[0];
                }
            }
        }
        return null;
    }
}
