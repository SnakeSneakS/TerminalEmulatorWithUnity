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
    private void Execute_Cd(string command, Output output)
    {
        if (string.IsNullOrEmpty(command)) return;

        string[] command_split = command.Split(' ');
        string relativePath = command_split[1];
        UnityEngine.Debug.Log("RELATIVE PATH: "+relativePath);
            
        if (command_split.Length > 1)
        {
            string newPath = Path.GetFullPath( Path.Combine( WorkingDirectory, relativePath) );
            if (Directory.Exists(newPath))
            {
                Command.WorkingDirectory = newPath;
                output.Log_success("Working directory: " + newPath);
            }
            else
            {
                output.Log_warn("Not found path: \"" + newPath + "\"");
                output.Log_error("今後、TabによるPath予測はできない可能性があります");
            }
        }
        else
        {
            output.Log_warn("Error: Command \"cd\" needs argument");
        }
    }

}
