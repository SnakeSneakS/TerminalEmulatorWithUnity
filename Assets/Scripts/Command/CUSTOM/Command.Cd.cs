using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Command.Handler.Cd
public partial class Command
{
    public Command.Handler NewHandler_Cd()
    {
        Action<string,Output> this_func = Execute_Cd;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //実行
    private void Execute_Cd(string command,Output output)
    {

    }

}
