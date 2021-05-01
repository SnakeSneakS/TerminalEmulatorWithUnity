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
        public Action<string, Output> act;
    };

    //Dictionaryによってlsなどのコマンドフラグとhandlerを結びつける。
    private Dictionary<string, Handler> NewCommandHandler(Output output)
    {
        return new Dictionary<string, Handler> ()
        {
            //ls
            { "ls", NewHandler_Ls() },

            //cd
            { "cd", NewHandler_Cd() },

            //sh reactive
            {"LOGIN", NewHandler_LoginShell() },
            
        };
    }
}
