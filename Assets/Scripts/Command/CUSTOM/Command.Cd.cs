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
        Action<string, Output> this_func = Execute_Cd;
        return new Command.Handler()
        {
            act = this_func,
        };
    }

    //実行
    private void Execute_Cd(string command, Output output)
    {
        if (_IsExecuting) return;
        if (command == "") return;

        _IsExecuting = true;

        string[] command_split = command.Split(' ');
        if (command_split.Length > 1)
        {
            string newPath = this.GetFullPath(command_split[1]);
            if (Directory.Exists(newPath))
            {
                Command.WorkingDirectory = newPath;
                output.Log_result("Working directory: " + newPath, Output.LogOption.NewSingleLineWhite());
            }
            else
            {
                output.Log_result("Not found directory name: \"" + newPath + "\"", Output.LogOption.NewSingleLineRed());
            }
        }
        else
        {
            output.Log_result("Error: Command \"cd\" needs argument", Output.LogOption.NewSingleLineRed());
        }
        
        
        _IsExecuting = false;
    }

    private string GetFullPath(string oldPath)
    {
        string newPath = Path.Combine( WorkingDirectory, oldPath);
        return newPath;
    }

}
