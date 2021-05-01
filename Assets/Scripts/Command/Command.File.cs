using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//Command Execute {File}
public partial class Command 
{
    //ExecuteFile: bin/bashなど、ファイルでコマンドを実行する。見つからなかった場合はShFileNameを実行する。これはリアクティブ（対話）モードではない
    private void ExecuteFile(string command, DataReceivedEventHandler handler_data, DataReceivedEventHandler handler_error)
    {
        if (_IsExecuting) { UnityEngine.Debug.Log("Error: Executing other process!"); return; }
        if (command == "") { UnityEngine.Debug.Log("Error: No command is found!"); return; }
        

        string[] command_split = command.Split(' ');
        string fileName = FindFilesUnderPATH( command_split[0] );
        string arguments = (command_split.Length>1) ? command_split[1]: "";

        if (fileName == null){
            fileName = ShellFileName;
            arguments = "-c \" " + command + " \"";
        }

        Thread t = new Thread(new ThreadStart(() =>
        {
            try
            {
                using (Process proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = fileName;
                    proc.StartInfo.Arguments = arguments;
                    proc.StartInfo.WorkingDirectory = WorkingDirectory;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;


                    proc.OutputDataReceived += handler_data;
                    proc.ErrorDataReceived += handler_error;

                    proc.EnableRaisingEvents = true;
                    proc.Exited += new EventHandler((sender, e) => {
                        _IsExecuting = false;
                        UnityEngine.Debug.Log("Process end: " + ((Process)sender).Id.ToString());
                    });

                    _IsExecuting = true;
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit(); //<- コレが非同期じゃない。そのため別スレッドを作成して実行すると良い?

                    return;
                }
            }
            catch (Exception e)
            {
                string result = "[Error!!] " + e.Message;
                UnityEngine.Debug.Log(result);
                return;
            }
        }));

        t.Start();
        
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
