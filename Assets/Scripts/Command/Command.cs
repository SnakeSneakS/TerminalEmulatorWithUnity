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

        //output.Log_execute(command_flag, new Output.LogOption{} );
        output.Log_execute(command, Output.LogOption.NewSingleLineGreen() );

        //コマンドが見つかれば、そのコマンドのhandler実行
        if ( command_handlers.TryGetValue(command_flag,out handler) )
        {
            handler.act(command, output);
        }
        //コマンドが見つからなければ、shFileName(/bin/zshなど)を実行
        else
        {
            NewHandler_Default(command,output).act(command,output);
        } 
    }

}

