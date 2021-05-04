using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Command.Handler
public partial class Command
{
    //Handlerによってコマンドを実行する
    public class Handler
    {
        public Action<string,Output> act;
    };

    //Dictionaryによってlsなどのコマンドフラグとhandlerを結びつける。
    private Dictionary<string, Handler> NewCommandHandler(Output output)
    {
        return new Dictionary<string, Handler> ()
        {
            //shell login
            {"LOGIN", NewHandler_LoginShell() },
            //ls
            { "ls", NewHandler_Ls() },
            //cd
            { "cd", NewHandler_Cd() },
            //default
            {"default", NewHandler_Default() }
        };
    }
}
