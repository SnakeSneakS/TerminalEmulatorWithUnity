using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Runtime.InteropServices;

public partial class Input: MonoBehaviour
{
    int HistPosRelative = 1;

    private bool SuggestedPathBool=false;
    private string[] SuggestedPathes;
    private int SuggestedPathCompleteIndex=-1;

    //Start
    private void CommandEvent_Start()
    {
        //inputFieldの値が変わったときに実行
        inputField.onValueChanged.AddListener((s) =>
        {
            output.UpdateLogInput(s);

            if (SuggestedPathBool && !UnityEngine.Input.GetKey(KeyCode.Tab)) SuggestedPathBool = false; //Path予測リセット
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
        //Ctrl+C いまいちわからんため、、、う〜ん、、、DEKINAI...
        /*
        else if(UnityEngine.Input.GetKey(KeyCode.LeftCommand) || UnityEngine.Input.GetKey(KeyCode.RightControl))
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C)) { inputField.text = "kill" + command.proc.Id.ToString(); }
        }
        */
    }

    
    private void WhenExecuteCommand_InputCommand()
    {
        HistPosRelative = 1; //Commandが実行された時に、HistPosRelativeを変更する
    }

    //コマンド実行
    private void ExecuteCommand()
    {
        if (Regex.IsMatch(inputField.text, "^( )*$")) { UnityEngine.Debug.Log("Empty value for inputField"); return; }
        
        
        command.Execute(inputField.text);
        inputField.text = "";
        output.UpdateLogInput(inputField.text);
    }

    //コマンドの補完 とりあえずはpathのみ
    private void CompleteCommand()
    {
        string input_path = inputField.text.Substring(inputField.text.LastIndexOf(' ') + 1);

        //コマンドを補完する。もしかしたら、/.zcompdumpだけど、タブシグナルの送り方がわからないため自作する
        if (!SuggestedPathBool) //まだ検索していないパスの場合、
        {
            SuggestedPathBool = true; SuggestedPathCompleteIndex = -1;

            SuggestedPathes = SuggestPathes(input_path);
            output.myHistory.SetMyWarning("~ PATHの予想先 ~");
            for (int i = 0; i < SuggestedPathes.Length; i++) output.myHistory.SetMyWarning(" " + SuggestedPathes[i]);
            output.myHistory.SetMyWarning("\n");

            //1つだけの場合、それを用いる+予測変換をリセット
            if (SuggestedPathes.Length == 1)
            {
                SuggestedPathBool = false;
                inputField.text = inputField.text.Substring(0, inputField.text.LastIndexOf(' ') + 1) + SuggestedPathes[0];
            }

            output.myHistory.setDisplayLineToWriteLine();
            output.Log_show(output.myHistory.displayHistLine);
        }
        else //検索したことのあるパスの場合、
        {
            SuggestedPathCompleteIndex++;
            if (SuggestedPathCompleteIndex >= SuggestedPathes.Length) SuggestedPathCompleteIndex = 0;
            string newPath = SuggestedPathes[SuggestedPathCompleteIndex];
            if (newPath[newPath.Length-1] == '/') newPath = newPath.Substring(0, newPath.Length - 1); //pathの最後の"/"を取り除く
            inputField.text = inputField.text.Substring(0, inputField.text.LastIndexOf(' ') + 1) + newPath;
        }
        
    }
    
    //inputからpathesを予測する。Tabで実行
    private string[] SuggestPathes(string input_path)
    {
        //while (input.Length > 0 && input[input.Length - 1] == ' ') input = input.Substring(0, input.Length - 2);
        if (string.IsNullOrEmpty(input_path)) return new string[]{ };
        
        int last = input_path.LastIndexOf('/');

        string[] pathes;
        if (last == -1)
        {
            string[] files = Directory.GetFiles(Command.WorkingDirectory, input_path + "*");
            files = GetRelativePathes((Command.WorkingDirectory[Command.WorkingDirectory.Length - 1] == '/') ? Command.WorkingDirectory : Command.WorkingDirectory + "/", files);
            string[] directories = Directory.GetDirectories(Command.WorkingDirectory, input_path + "*");
            directories = GetRelativePathes((Command.WorkingDirectory[Command.WorkingDirectory.Length - 1] == '/') ? Command.WorkingDirectory : Command.WorkingDirectory + "/", directories);
            for (int i = 0; i < directories.Length; i++) directories[i] += "/";

            pathes = new string[files.Length + directories.Length];
            files.CopyTo(pathes, 0);
            directories.CopyTo(pathes, files.Length);
            Array.Sort(pathes);

            return pathes;
        }
        else
        {
            string path = Path.GetFullPath( Path.Combine( Command.WorkingDirectory , input_path.Substring(0, last+1) ) );
            //UnityEngine.Debug.Log(path);
            //UnityEngine.Debug.Log(input_path.Substring(last+1) + "*");
            if (!Directory.Exists(path)) return new string[] { };
            string[] files = Directory.GetFiles(path, input_path.Substring(last + 1) + "*");
            files = GetRelativePathes((Command.WorkingDirectory[Command.WorkingDirectory.Length - 1] == '/') ? Command.WorkingDirectory : Command.WorkingDirectory + "/", files);
            string[] directories = Directory.GetDirectories(path, input_path.Substring(last + 1) + "*");
            directories = GetRelativePathes((Command.WorkingDirectory[Command.WorkingDirectory.Length - 1] == '/') ? Command.WorkingDirectory : Command.WorkingDirectory + "/", directories);
            for (int i = 0; i < directories.Length; i++) directories[i] += "/";

            pathes = new string[files.Length + directories.Length];
            files.CopyTo(pathes, 0);
            directories.CopyTo(pathes, files.Length);
            Array.Sort(pathes);

            return pathes;
        }
    }
    private string[] GetRelativePathes(string from,string[] to)
    {
        string[] relative = new string[to.Length];

        Uri fromUri = new Uri(from);
        for (int i = 0; i < to.Length; i++)
        {
            Uri toUri = new Uri(to[i]);
            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            relative[i] = relativeUri.ToString();
            //UnityEngine.Debug.Log(from + " + " + to[i] + " => " + relative[i]);
        }
        return relative; 
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
