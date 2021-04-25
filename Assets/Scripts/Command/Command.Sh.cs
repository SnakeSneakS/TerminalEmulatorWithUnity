using System;
using System.IO;
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

    public static StreamWriter SW;
    public static bool _IsReactiveMode;

    //REACTIVEにShを実行する。リアクティブシェルを実行
    //ExecuteShReactive: bin/bashなど、リアクティブシェルを実行する。
    private void ExecuteShReactive(string command, DataReceivedEventHandler handler_data, DataReceivedEventHandler handler_error)
    {
        if (_IsExecuting) return;
        if (command == "") return;

        try
        {
            using (Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = ShFileName;
                proc.StartInfo.Arguments = "";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.WorkingDirectory = WorkingDirectory;

                proc.OutputDataReceived += handler_data;
                proc.ErrorDataReceived += handler_error;

                //非同期?
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler((sender,e) => {
                    _IsReactiveMode = false;
                    _IsExecuting = false;
                    UnityEngine.Debug.Log("Sh process end!!");
                    SW.Close();
                });

                _IsExecuting = true;
                _IsReactiveMode = true;

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                SW = proc.StandardInput;
                if (SW.BaseStream.CanWrite)
                {
                    SW.WriteLine("pwd");
                    SW.WriteLine("ls ./");
                }
                /* Inputの例
                StreamWriter streamWriter = proc.StandardInput;
                if (streamWriter.BaseStream.CanWrite)
                {
                    streamWriter.WriteLine("pwd");
                    streamWriter.WriteLine("ls ./");
                    streamWriter.WriteLine("exit");
                }
                streamWriter.Close();
                */

                /* 同期だった場合に使っていた
                proc.WaitForExit(); => EnableRaisingEvents
                _IsReactiveMode = false;
                _IsExecuting = false;
                UnityEngine.Debug.Log("Sh process end!!");
                */

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

    //Handler
    public Command.Handler NewHandler_ShReactive()
    {
        Action<string, Output> this_func = Execute_ShReactive;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //デフォルトの実行: Zsh or Bash のコマンドとして実行
    private void Execute_ShReactive(string command, Output output)
    {
        ExecuteShReactive(command, NewDataReceivedEventHandler_ShReactive(output), NewErrorReceivedEventHandler_ShReactive(output));
    }

    //実行後のデータ受け取り
    private static DataReceivedEventHandler NewDataReceivedEventHandler_ShReactive(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data, Output.LogOption.NewSingleLineWhite());
        });
    }

    //実行後のエラー受け取り
    private static DataReceivedEventHandler NewErrorReceivedEventHandler_ShReactive(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data, Output.LogOption.NewSingleLineRed());
        });
    }

}
