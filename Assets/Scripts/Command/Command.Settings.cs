using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Settings
public partial class Command : MonoBehaviour
{
    public enum SettingName
    {
        ShFileName,
        WorkingDirectory,
        PATH
    }

    public static string ShFileName = ""; //"/bin/zsh"; "/bin/bash";
    [SerializeField] public static string WorkingDirectory;
    public static string PATH = "";

    [SerializeField] public static bool _IsExecuting = false;
    [SerializeField] public Output output;
    [SerializeField] public Dictionary<string, Handler> command_handlers;


    private void Awake()
    {
        GetLocalDatas();
        command_handlers = NewCommandHandler(output);
        
        if(ShFileName=="") ShFileName ="/bin/bash"; //"/bin/bash";
        if(PATH=="") PATH = Environment.GetEnvironmentVariable("PATH",EnvironmentVariableTarget.Process); //本当はMachineを取得したいけどNULLが帰ってくる
        if(WorkingDirectory=="") WorkingDirectory = Environment.CurrentDirectory;

        //Debug.Log(Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process));
    }

    private void Start()
    {
        ShowLocalDatas();
    }

    //PlayerPrefsからデータを読み取る
    private void GetLocalDatas()
    {
        ShFileName = PlayerPrefs.GetString( SettingName.ShFileName.ToString() );
        PATH=PlayerPrefs.GetString( SettingName.PATH.ToString() );
        WorkingDirectory = PlayerPrefs.GetString( SettingName.WorkingDirectory.ToString() );
    }

    //ShowLogOfLocalDatas
    private void ShowLocalDatas()
    {
        output.Log_execute("SETTINGS",Output.LogOption.NewSingleLineGreen() );
        output.Log_Show("ShFileName: "+ShFileName, Output.LogOption.NewMultipleLineWhite() );
        output.Log_Show("PATH: " + PATH, Output.LogOption.NewMultipleLineWhite());
        output.Log_Show("WorkingDirectory: " + WorkingDirectory, Output.LogOption.NewMultipleLineWhite());
    }

    //PlayerPrefsにShFileNameを保存する
    public void SetShFileName(string s)
    {
        PlayerPrefs.SetString( SettingName.ShFileName.ToString(), s);
    }
    //PreyerPrefsにPATHを保存する
    public void SetPATH(string s)
    {
        PlayerPrefs.SetString( SettingName.PATH.ToString(), s);
    }
    //PlayerPrefsにWorkingDirectoryを保存する
    public void SetWorkingDirectory(string s)
    {
        PlayerPrefs.SetString( SettingName.WorkingDirectory.ToString(), s);
    }


    
}
