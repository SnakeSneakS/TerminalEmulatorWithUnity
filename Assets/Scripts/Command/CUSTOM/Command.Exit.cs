using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Command.Handler.Exit
public partial class Command
{


    public Command.Handler NewHandler_Exit()
    {
        Action<string, Output> this_func = Execute_Exit;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //実行
    private void Execute_Exit(string command, Output output)
    {
        proc.Kill();
    }

}