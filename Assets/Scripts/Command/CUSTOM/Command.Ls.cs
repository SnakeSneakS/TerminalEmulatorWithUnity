using System;
using System.Diagnostics;
using UnityEngine;

//Command.Handler.Ls
public partial class Command
{


    public Command.Handler NewHandler_Ls()
    {
        Action<string, Output> this_func = Execute_Ls;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //実行
    private void Execute_Ls(string command, Output output)
    {
        ExecuteFile (command, NewDataReceivedEventHandler_Ls(output), NewErrorReceivedEventHandler_Ls(output) );
    }

    //実行後のデータ受け取り
    private static DataReceivedEventHandler NewDataReceivedEventHandler_Ls(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data,Output.LogOption.NewSingleLineWhite() );
        });
    }

    //実行後のエラー受け取り
    private static DataReceivedEventHandler NewErrorReceivedEventHandler_Ls(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data,Output.LogOption.NewSingleLineRed() );
        });
    }
}
