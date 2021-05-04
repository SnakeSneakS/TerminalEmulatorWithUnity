using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//Sh
public partial class Command
{
    public static StreamWriter SW;
    public static bool _IsInteractiveMode;
    public Process proc;

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
                using (proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = ShellFileName; //command; // /bin/zshなどのShellFileName
                    proc.StartInfo.Arguments = "-l"; //"-l -i -s";
                    proc.StartInfo.WorkingDirectory = WorkingDirectory;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;


                    
                    //正常output
                    proc.OutputDataReceived += handler_data;
                    //エラーoutput
                    proc.ErrorDataReceived += handler_error;

                    //終了(非同期)
                    proc.EnableRaisingEvents = true;
                    proc.Exited += ((sender, e) => {
                        _IsInteractiveMode = false;
                        _IsExecuting = false;
                        NowReactiveProcessName = "";
                        output.Log_success("Process end..." + ((Process)sender).Id.ToString());
                        SW.WriteLine("exit");
                        SW.Close();
                    });

                    _IsExecuting = true;
                    _IsInteractiveMode = true;
                    NowReactiveProcessName = proc.StartInfo.FileName;

                    //開始
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    //入力
                    SW = proc.StandardInput;
                    if (SW.BaseStream.CanWrite)
                    {
                        SW.WriteLine("echo \"PROCESS START...\" ");
                    }
                    proc.WaitForExit();



                    /*
                    //改行されていない途中までのやつを読み込みたい
                    proc.Start();
                    _IsExecuting = true;
                    _IsInteractiveMode = true;
                    NowReactiveProcessName = proc.StartInfo.FileName;
                    SW = proc.StandardInput;
                    if (SW.BaseStream.CanWrite) SW.WriteLine("echo \"PROCESS START...\" ");

                    StreamReader SRO = proc.StandardOutput;
                    StreamReader SRE = proc.StandardError;
                    output.logDisplayLine = Output.LogDisplayLine.Single;
                    while (true)
                    {
                        string so = "";
                        while (SRO.Peek() != -1 )
                        {
                            so += (char)SRO.Read();
                        }
                        if (!String.IsNullOrEmpty(so))
                        {
                            output.Log_show("SO: " + so, Output.LogDisplayColor.White, output.logDisplayLine);
                        }
                        
                        string se = "";
                        while (SRE.Peek() != -1)
                        {
                            se += (char)SRE.Read();
                            output.Log_show("XXXXXXXXXXXXXXXXXXXXXXXXX", Output.LogDisplayColor.White, output.logDisplayLine);
                        }
                        if (!String.IsNullOrEmpty(se))
                        {
                            output.Log_show("SE: " + se, Output.LogDisplayColor.White, output.logDisplayLine);
                        }

                        if (proc.HasExited) break;
                    }

                    proc.WaitForExit();
                    _IsInteractiveMode = false;
                    _IsExecuting = false;
                    NowReactiveProcessName = "";
                    output.Log_success("Process end...");
                    SW.WriteLine("exit");
                    SW.Close();
                    */

                    return;
                }
            }
            catch (Exception e)
            {
                if (e == null) return;
                string result = "[Error!!] " + e.Message;
                UnityEngine.Debug.LogError(result);
                output.Log_error(result);
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
            output.Log_success(e.Data);
        });
    }

    //実行後のエラー受け取り
    private static DataReceivedEventHandler NewErrorReceivedEventHandler_LoginShell(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_error(e.Data);
        });
    }

}
