using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//Handler_Default
public partial class Command
{
    //Handler
    public Command.Handler NewHandler_Default()
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
        output.logDisplayLine = Output.LogDisplayLine.Multiple;
    }

}
