using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

/*
 * ref: https://docs.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process.outputdatareceived?view=net-5.0
 */

public partial class Command: MonoBehaviour
{
    public void Execute(string command)
    {
        string command_flag = command.Split(' ')[0];
        Handler handler;

        //実行コマンドを出力
        output.WhenExecute(command);

        if(Command._IsInteractiveMode) //対話モードの場合、実行中のプロセス(zshなど)のStreamWriterへ書き込む
        {
            if (Command.SW != null && Command.SW.BaseStream.CanWrite)
            {
                
                //コマンドが見つかれば、そのコマンドのhandler実行
                if (command_handlers.TryGetValue(command_flag, out handler))
                {
                    handler.act(command, output);
                }
                //コマンドが見つからなければ、shellFileName(/bin/zshなど)を実行
                else
                {
                    if (command_handlers.TryGetValue("default", out handler)) handler.act(command, output);
                    else UnityEngine.Debug.LogError("Default handler was not found!");
                }

                //Reactiveモードのシェルにコマンドを書き込む
                Command.SW.WriteLine(command);
                UnityEngine.Debug.Log("stream write: " + command);

            }
            else
            {
                UnityEngine.Debug.Log("stream can't write");
            }
        }
        else //対話モードではない場合、LOGIN SHELL のみ可能にする
        {
            if (command == "LOGIN") Execute_LoginShell(command, output);
            else output.WhenError("Enter \"LOGIN\" to Start...");
        }

    }

}

