using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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

    public static StreamWriter SW;
    public static bool _IsReactiveMode;

    //REACTIVEにShを実行する。ログインシェル(リアクティブシェル)を実行
    //ExecuteShReactive: bin/bashなど、ログインシェル(リアクティブシェル)を実行する。
    private void ExecuteLoginShell(string command, DataReceivedEventHandler handler_data, DataReceivedEventHandler handler_error)
    {
        if (_IsExecuting) { UnityEngine.Debug.Log("Error: Executing other process!"); return; }
        if (command == "") { UnityEngine.Debug.Log("Error: No command is found!"); return; }

        Thread t = new Thread(new ThreadStart(() =>
        {
            try
            {
                using (Process proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = ShellFileName; //command; // /bin/zshなどのShellFileName
                    proc.StartInfo.Arguments = "-l"; //"";
                    proc.StartInfo.WorkingDirectory = WorkingDirectory;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;

                    proc.OutputDataReceived += handler_data;
                    proc.ErrorDataReceived += handler_error;


                    //終了(非同期)
                    proc.EnableRaisingEvents = true;
                    proc.Exited += ((sender, e) => {
                        _IsReactiveMode = false;
                        _IsExecuting = false;
                        NowReactiveProcessName = "";
                        output.Log_result("Process end...",Output.LogOption.NewSingleLineWhite() );
                        UnityEngine.Debug.Log("Process end: " + ((Process)sender).Id.ToString() );
                        SW.Close();
                    });

                    _IsExecuting = true;
                    _IsReactiveMode = true;
                    NowReactiveProcessName = proc.StartInfo.FileName;

                    //開始
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    //入力
                    SW = proc.StandardInput;
                    if (SW.BaseStream.CanWrite)
                    {
                        SW.WriteLine("echo \"PROCESS START!! LOGIN SHELL PROCESS...\" ");
                    }

                    proc.WaitForExit();
                    return;
                }
            }
            catch (Exception e)
            {
                if (e == null) return;
                string result = "[Error!!] " + e.Message;
                UnityEngine.Debug.LogError(result);
                output.Log_result(result, Output.LogOption.NewSingleLineRed());
                return;
            }
        }));

        t.Start();
    }

    //Handler
    public Command.Handler NewHandler_LoginShell()
    {
        Action<string, Output> this_func = Execute_LoginShell;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //デフォルトの実行: Zsh or Bash のコマンドとして実行
    private void Execute_LoginShell(string command, Output output)
    {
        ExecuteLoginShell(command, NewDataReceivedEventHandler_LoginShell(output), NewErrorReceivedEventHandler_LoginShell(output));
    }

    //実行後のデータ受け取り
    private static DataReceivedEventHandler NewDataReceivedEventHandler_LoginShell(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data, Output.LogOption.NewSingleLineWhite());
        });
    }

    //実行後のエラー受け取り
    private static DataReceivedEventHandler NewErrorReceivedEventHandler_LoginShell(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data, Output.LogOption.NewSingleLineRed());
        });
    }

}
