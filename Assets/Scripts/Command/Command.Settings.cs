using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Command.Settings
public partial class Command : MonoBehaviour
{
    public enum SettingName
    {
        ShFileName,
        WorkingDirectory,
        //PATH,
    }

    public static string ShellFileName = ""; //"/bin/zsh"; "/bin/bash";
    public static string WorkingDirectory="";
    //public static string PATH = "";

    public static bool _IsExecuting = false;
    public static string NowReactiveProcessName = "";
    
    [SerializeField] public Output output;
    [SerializeField] public Dictionary<string, Handler> command_handlers;


    private void Awake()
    {
        GetLocalDatas();
        command_handlers = NewCommandHandler(output);

        if (ShellFileName == "")
        {
            ShellFileName = Environment.GetEnvironmentVariable("SHELL", EnvironmentVariableTarget.Process); //"/bin/bash";
            SetShellFileName(ShellFileName);
        }
        if (WorkingDirectory == "")
        {
            WorkingDirectory = Environment.CurrentDirectory;
            SetWorkingDirectory(WorkingDirectory);
        }

    }

    private void Start()
    {
        ShowLocalDatas();
        //Execute_LoginShell(ShellFileName, output);
    }

    //PlayerPrefsからデータを読み取る
    private void GetLocalDatas()
    {
        ShellFileName = PlayerPrefs.GetString( SettingName.ShFileName.ToString() );
        WorkingDirectory = PlayerPrefs.GetString( SettingName.WorkingDirectory.ToString() );
    }

    //ShowLogOfLocalDatas
    private void ShowLocalDatas()
    {
        output.Log_execute("SETTINGS" );
        output.Log_success("ShellFileName: "+ShellFileName );
        output.Log_success("WorkingDirectory: " + WorkingDirectory );
    }

    //PlayerPrefsにShellFileNameを保存する
    public void SetShellFileName(string s)
    {
        PlayerPrefs.SetString( SettingName.ShFileName.ToString(), s);
    }

    /*
    //PreyerPrefsにPATHを保存する
    public void SetPATH(string s)
    {
        PlayerPrefs.SetString( SettingName.PATH.ToString(), s);
    }
    */

    //PlayerPrefsにWorkingDirectoryを保存する
    public void SetWorkingDirectory(string s)
    {
        PlayerPrefs.SetString( SettingName.WorkingDirectory.ToString(), s);
    }


    
}
