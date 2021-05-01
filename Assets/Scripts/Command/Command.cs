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
        output.Log_execute(command, Output.LogOption.NewSingleLineGreen() );

        if(Command._IsReactiveMode) //対話モードの場合、実行中のプロセス(zshなど)にシェル実行などを任せる。
        {
            if (Command.SW != null && Command.SW.BaseStream.CanWrite)
            {
                Command.SW.WriteLine(command);
                UnityEngine.Debug.Log("stream write: " + command);
            }
            else
            {
                UnityEngine.Debug.Log("stream can't write");
            }
        }
        else //対話モードではない場合、自力で探す
        {
            //コマンドが見つかれば、そのコマンドのhandler実行
            if (command_handlers.TryGetValue(command_flag, out handler))
            {
                handler.act(command, output);
            }
            //コマンドが見つからなければ、shellFileName(/bin/zshなど)を実行
            else
            {
                NewHandler_Default(command, output).act(command, output);
            }
        }

    }

}

