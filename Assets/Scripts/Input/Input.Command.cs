using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class Input: MonoBehaviour
{
    int HistPosRelative = 0;

    //Start
    private void CommandEvent_Start()
    {
        //inputFieldの値が変わったときに実行
        inputField.onValueChanged.AddListener((s) =>
        {
            output.UpdateLogInput(s);
        });

        //buttonクリックでコマンド実行
        commandSubmitButton.onClick.AddListener(() =>
        {
            ExecuteCommand();
        });
    }
    //Update
    private void CommandEvent_Update()
    {
        UpdateEventWhenFocus();
    }

    //入力にフォーカスしているときに、キーボード入力検知
    private void UpdateEventWhenFocus()
    {
        if (!inputField.isFocused) return;

        //コマンド実行
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            inputField.text = inputField.text.Replace("\n", ""); inputField.text = inputField.text.Replace("\r", ""); //改行コード削除
            ExecuteCommand();
        }
        //コマンド補完
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
        {
            inputField.text = inputField.text.Replace("\t", ""); //tab 消去
            CompleteCommand();
        }
        //historyのコマンドを使う
        else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)) UseHistoryCommand(--HistPosRelative);
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) UseHistoryCommand(++HistPosRelative);
        //Ctrl+C いまいちわからんため、、、う〜ん、、、
        /*
        else if(UnityEngine.Input.GetKey(KeyCode.LeftCommand) || UnityEngine.Input.GetKey(KeyCode.RightControl))
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C)) { inputField.text = "kill" + command.proc.Id.ToString(); }
        }
        */
    }


    //Commandが実行された時に、HistPosRelativeを変更する
    private void UpdateHistPosRelative()
    {
        HistPosRelative = 0;
    }

    //コマンド実行
    private void ExecuteCommand()
    {
        if (Regex.IsMatch(inputField.text, "^( )*$")) { UnityEngine.Debug.Log("Empty value for inputField"); return; }
        
        
        command.Execute(inputField.text);
        inputField.text = "";
        output.UpdateLogInput(inputField.text);
    }

    //コマンドの補完 とりあえずはファイルのみ
    private void CompleteCommand()
    {
        //コマンドを補完する。もしかしたら、/.zcompdump
        string[] pathes = SuggestPathes(inputField.text);
        output.myHistory.SetMyWarning("PATHの予想先");
        foreach (var p in pathes)
        {
            output.myHistory.SetMyWarning("- "+p);
        }
        output.myHistory.setDisplayLineToWriteLine();
        output.Log_show(output.myHistory.displayHistLine);
        UnityEngine.Debug.Log("ALL FILES END");
    }
    //inputのpathを書き換える
    private void UpdatePathOfInputField(string path)
    {
        while (inputField.text.Length > 0 && inputField.text[inputField.text.Length - 1] == ' ') inputField.text = inputField.text.Substring(0, inputField.text.Length - 2);

        int last = inputField.text.LastIndexOf(' ');
        if (last == -1) inputField.text = path;
        else inputField.text = inputField.text.Substring(0, last) + path;
    }
    //inputからpathesを予測する。Tabで実行
    private string[] SuggestPathes(string input)
    {
        while (input.Length > 0 && input[input.Length - 1] == ' ') input = input.Substring(0, input.Length - 2);
        
        int last = input.LastIndexOf(' ');
        string input_path = input.Substring(last + 1);
        last = input_path.LastIndexOf('/');

        string[] pathes;
        if (last == -1)
        {
            //if (string.IsNullOrEmpty(input_path)) { return new string[0]; }
            string[] files = Directory.GetFiles(Command.WorkingDirectory, input_path + "*");
            string[] directories = Directory.GetDirectories(Command.WorkingDirectory, input_path + "*");
            pathes = new string[files.Length + directories.Length];
            files.CopyTo(pathes, 0);
            directories.CopyTo(pathes, files.Length);
            return pathes;
        }
        else
        {
            string path = Path.Combine( Command.WorkingDirectory, input_path.Substring(0, last) );
            string[] files = Directory.GetFiles(path, (last == input_path.Length - 1) ? "*" : path.Substring(last + 1) + "*");
            string[] directories = Directory.GetDirectories(path, (last == input_path.Length - 1) ? "*" : path.Substring(last + 1) + "*");
            pathes = new string[files.Length + directories.Length];
            files.CopyTo(pathes, 0);
            directories.CopyTo(pathes, files.Length);
            return pathes;
        }
    }

    //historyの1つ上のコマンド
    private void UseHistoryCommand(int HistoryPos, bool relative = true)
    {
        if (relative)
        {
            //「現在のHistory行+HistoryPos」 行目のHistoryにあるコマンドをinputfieldにペースト
            int i = output.myHistory.writeHistLine + HistoryPos;
            if (i < 0 || i >= output.myHistory.HISTSIZE)
            {
                UnityEngine.Debug.LogError("Out of range: "+i);
                return;
            }
            inputField.text = output.myHistory.histories[i].Command_Uncolored;
        }
        else
        {
            //「HistoryPos」 行目のHistoryにあるコマンドをinputfieldにペースト
            int i = HistoryPos;
            if (i < 0 || i >= output.myHistory.HISTSIZE)
            {
                UnityEngine.Debug.LogError("Out of range: " + i);
                return;
            }
            inputField.text = output.myHistory.histories[i].Command_Uncolored;
        }
    }
}
