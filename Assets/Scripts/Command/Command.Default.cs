using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//Handler_Default
public partial class Command: MonoBehaviour
{
    //Handler
    public Command.Handler NewHandler_Default(string command, Output output)
    {
        Action<string, Output> this_func = Execute_Default;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //デフォルトの実行: Zsh or Bash のコマンドとして実行
    private void Execute_Default(string command, Output output)
    {
        ExecuteFile(command, NewDataReceivedEventHandler_Default(output), NewErrorReceivedEventHandler_Default(output));
    }

    //実行後のデータ受け取り
    private static DataReceivedEventHandler NewDataReceivedEventHandler_Default(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data, Output.LogOption.NewSingleLineWhite());
        });
    }

    //実行後のエラー受け取り
    private static DataReceivedEventHandler NewErrorReceivedEventHandler_Default(Output output)
    {
        return new DataReceivedEventHandler((s, e) =>
        {
            output.Log_result(e.Data,Output.LogOption.NewSingleLineRed() );
        });
    }

}
