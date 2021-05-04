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
        output.logDisplayLine = Output.LogDisplayLine.Multiple;
    }

}
