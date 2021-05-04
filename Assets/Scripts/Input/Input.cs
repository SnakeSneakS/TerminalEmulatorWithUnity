using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Input : MonoBehaviour
{
    [SerializeField] Command command;
    [SerializeField] Output output;

    [SerializeField] InputField inputField;
    [SerializeField] Button commandSubmitButton;

    private void Start()
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

    private void Update()
    {
        //入力にフォーカスしているときに、キーボード入力検知
        if (inputField.isFocused)
        {
            //コマンド実行
            if (UnityEngine.Input.GetKey(KeyCode.Return)) ExecuteCommand();
            //コマンド補完
            else if (UnityEngine.Input.GetKey(KeyCode.Tab)) CompleteCommand();
            //historyのコマンドを使う
            else if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) UseHistoryCommand(-1);
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) UseHistoryCommand(+1);
        }
    }

    //コマンド実行
    private void ExecuteCommand()
    {
        if (Regex.IsMatch(inputField.text, "^( )*$")) { UnityEngine.Debug.Log("Empty value for inputField"); return; }
        command.Execute(inputField.text);
        inputField.text = "";
        output.UpdateLogInput(inputField.text);
    }

    //コマンドの補完
    private void CompleteCommand()
    {
        //コマンドを補完する。もしかしたら、/.zcompdump
    }

    //historyの1つ上のコマンド
    private void UseHistoryCommand(int HistoryPos,bool relative=true)
    {
        if (relative)
        {
            //「現在のHistory行+HistoryPos」 行目のHistoryにあるコマンドをinputfieldにペースト
        }
        else
        {
            //「HistoryPos」 行目のHistoryにあるコマンドをinputfieldにペースト
        }
    }

}