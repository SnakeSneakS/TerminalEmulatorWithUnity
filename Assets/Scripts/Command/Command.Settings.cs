using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Command.Settings
public partial class Command : MonoBehaviour
{
    public enum SettingName
    {
        ShellFileName,
        ShellArguments,
        WorkingDirectory,
        //PATH,
    }

    public static string ShellFileName = ""; //"/bin/zsh"; "/bin/bash";
    public static string ShellArguments = ""; //"-l" "-i" "-c"; Set default in Awake()
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

        if ( string.IsNullOrEmpty(ShellFileName) )
        {
            ShellFileName = Environment.GetEnvironmentVariable("SHELL", EnvironmentVariableTarget.Process); //"/bin/bash";
            SetShellFileName(ShellFileName);
        }
        if ( string.IsNullOrEmpty(WorkingDirectory) )
        {
            WorkingDirectory = Environment.CurrentDirectory;
            SetWorkingDirectory(WorkingDirectory);
        }
        if (string.IsNullOrEmpty(ShellArguments))
        {
            ShellArguments = "-l";
            SetShellArgumentsy(ShellArguments);
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
        ShellFileName = PlayerPrefs.GetString( SettingName.ShellFileName.ToString() );
        ShellArguments= PlayerPrefs.GetString(SettingName.ShellArguments.ToString());
        WorkingDirectory = PlayerPrefs.GetString( SettingName.WorkingDirectory.ToString() );
    }

    //ShowLogOfLocalDatas
    private void ShowLocalDatas()
    {
        output.Log_execute("SETTINGS" );
        output.Log_success("ShellFileName: "+ShellFileName );
        output.Log_success("ShellArguments: " + ShellArguments);
        output.Log_success("WorkingDirectory: " + WorkingDirectory );
    }

    //PlayerPrefsにShellFileNameを保存する
    public void SetShellFileName(string s)
    {
        PlayerPrefs.SetString( SettingName.ShellFileName.ToString(), s);
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
    //PlayerPrefsにShellArgumentsを保存する
    public void SetShellArgumentsy(string s)
    {
        PlayerPrefs.SetString(SettingName.ShellArguments.ToString(), s);
    }



}
