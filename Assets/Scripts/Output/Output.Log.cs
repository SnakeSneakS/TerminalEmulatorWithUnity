using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//Output_Log(MyHistory)
public partial class Output : MonoBehaviour
{
    [SerializeField] ScrollRect LogOutputAreaScrollView;
    [SerializeField] GameObject LogOutputContent;
    [SerializeField] GameObject LogText_Pref;

    private string LogString_Output="";
    private string LogString_InputField = "";

    [SerializeField] private InputField LogText_Output_InputField; //コピーや選択する用
    [SerializeField] private RectTransform LogText_Output_InputField_RectTransform; //リサイズ用
    [SerializeField] private Text LogText_Output; //表示用
    [SerializeField] private RectTransform LogText_Output_RectTransform; //リサイズ用
    [SerializeField] private Text LogText_Input; //入力表示用

    
    public Original.MyHistory myHistory = new Original.MyHistory(50); //Original History にLogを保存する
    
    //現在はMultipleのみにしている
    public enum LogDisplayLine
    {
        Single, //SingleLine(ただし「文字数>SingleLineStringLengthMax」の場合は改行する)
        Multiple,
    }
    public enum LogDisplayColor
    {
        White,
        Black,
        Red,
        Green,
        Orange,
    }

    //実行
    public void Log_execute(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        myHistory.NewHistLine();
        myHistory.SetCommand(Command.NowReactiveProcessName+"> "+s);
        UnityEngine.Debug.Log("Command: " + s);
        myHistory.setDisplayLineToWriteLine();
        Log_show(myHistory.displayHistLine);

        input.EventWhenExecuteCommand();
    }

    //結果表示
    public void Log_success(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        myHistory.SetResultSuccess(s);
        UnityEngine.Debug.Log("SUCCESS: " + s);
        myHistory.setDisplayLineToWriteLine();
        Log_show(myHistory.displayHistLine);
    }
    //error表示
    public void Log_error(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        myHistory.SetResultError(s);
        UnityEngine.Debug.Log("ERROR: " + s);
        myHistory.setDisplayLineToWriteLine();
        Log_show(myHistory.displayHistLine);
    }
    //終了
    /*
    public void Log_end(string s)
    {
        
    }
    */

    //log表示
    public void Log_show(int displayLogLine)
    {
        if(displayLogLine<0 || displayLogLine >= myHistory.histories.Length)
        {
            UnityEngine.Debug.LogError("outside of histories");
            return;
        }
        myHistory.displayHistLine = displayLogLine;

        //error check
        if (LogText_Output == null || LogText_Output_InputField == null)
        {
            Debug.LogError("NOT FOUND LOGTEXT FOR DISPLAY!!\n");
            return;
        }

        LogString_InputField = myHistory.histories[myHistory.displayHistLine].Command_Uncolored + "\n" + myHistory.histories[myHistory.displayHistLine].Result_Uncolored;
        LogString_Output = myHistory.histories[myHistory.displayHistLine].Command_Colored + "\n" + myHistory.histories[myHistory.displayHistLine].Result_Colored;

        LogText_Update();
    }
    

    //Updateする
    //UIの操作などはMainThreadで行わなければならない(Dispatcherを介す)
    public void LogText_Update()
    {
        TotalManager.dispatcher.Invoke(() => {
            LogText_Output.text = LogString_Output;
            LogText_Output_InputField.text = LogString_InputField;
            ResizeInputField();
            UpdateLogInput("");
            GoToBottomOfLogContent();
        });
    }

    //Logの一番下へ移動する
    private void GoToBottomOfLogContent()
    {
        Canvas.ForceUpdateCanvases();
        LogOutputAreaScrollView.verticalNormalizedPosition = 0.0f;
        Canvas.ForceUpdateCanvases();
    }

    //InputFieldの表示をTextに合わせる
    private void ResizeInputField()
    {
        Canvas.ForceUpdateCanvases();
        LogText_Output_InputField_RectTransform.sizeDelta = LogText_Output_RectTransform.sizeDelta;
        Canvas.ForceUpdateCanvases();
    }

    //inputの内容をLogにもリアルタイムで反映する
    public void UpdateLogInput(string s)
    {
        LogText_Input.text = Command.NowReactiveProcessName + "> " + s;
    }

    //色付きにする
    public static string ColoringLine(string oldLine, Output.LogDisplayColor color)
    {
        if (string.IsNullOrEmpty(oldLine))
        {
            return oldLine;
        }

        string colored = "";

        //ターミナルの色付け https://qiita.com/PruneMazui/items/8a023347772620025ad6
        //とりあえず実装しない
        //foreach (string s in oldLine.Split("\[", '')) ;

        //自分流の色付け
        switch (color)
        {
            case Output.LogDisplayColor.Black:
                colored = "<color=#000000ff>" + oldLine + "</color>";
                break;
            case Output.LogDisplayColor.White:
                colored = "<color=#ffffffff>" + oldLine + "</color>";
                break;
            case Output.LogDisplayColor.Red:
                colored = "<color=#ff0000ff>" + oldLine + "</color>";
                break;
            case Output.LogDisplayColor.Green:
                colored = "<color=#00ff00ff>" + oldLine + "</color>";
                break;
            case Output.LogDisplayColor.Orange:
                colored = "<color=#ffa500ff>" + oldLine + "</color>";
                break;
            default:
                colored = oldLine;
                break;
        }
        return colored;
    }
}
