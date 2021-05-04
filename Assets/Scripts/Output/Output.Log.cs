using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//Output_Log
public partial class Output : MonoBehaviour
{
    public static int SingleLineStringLengthMax = 10;

    [SerializeField] ScrollRect LogOutputAreaScrollView;
    [SerializeField] GameObject LogOutputContent;
    [SerializeField] GameObject LogText_Pref;

    private string LogString_Output="";
    [SerializeField] private InputField LogText_Output_InputField; //コピーや選択する用
    [SerializeField] private RectTransform LogText_Output_InputField_RectTransform; //リサイズ用
    [SerializeField] private Text LogText_Output; //表示用
    [SerializeField] private RectTransform LogText_Output_RectTransform; //リサイズ用
    [SerializeField] private Text LogText_Input; //入力表示用

    //Original History にLogを保存する
    public Original.MyHistory myHistory = new Original.MyHistory();

    //LogOptionの設定
    public LogType logType = LogType.Success;
    public LogDisplayLine logDisplayLine = LogDisplayLine.Multiple;

    public enum LogType
    {
        Success,
        Error,
    }
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
    }

    //実行
    public void Log_execute(string s)
    {
        myHistory.NewHistLine();
        //AddNewLine(2);
        myHistory.SetCommand(Command.NowReactiveProcessName+"> ");
        UnityEngine.Debug.Log("Command: " + s);
        //AddNewLine(1);
    }

    //結果表示
    public void Log_success(string s)
    {
        myHistory.SetResultSuccess(s);
        UnityEngine.Debug.Log("SUCCESS: " + s);
    }
    //error表示
    public void Log_error(string s)
    {
        myHistory.SetResultError(s);
        UnityEngine.Debug.Log("ERROR: " + s);
    }
    //終了
    public void Log_end(string s)
    {
        
    }

    //log表示
    public void Log_show(int nowLogLine)
    {
        if (s == "" || s==null) return; //出力するものがある時のみ出力する
        UnityEngine.Debug.Log("LOG SHOW: "+s);

        //error check
        if (LogText_Output == null || LogText_Output_InputField == null)
        {
            Debug.LogError("NOT FOUND LOGTEXT FOR DISPLAY!!  \ncommand.... "+s);
            return;
        }

        //色付け
        switch (logDisplayColor)
        {
            case LogDisplayColor.Black:
                LogString_Output += "<color=#000000ff>" + s + "</color>";
                break;
            case LogDisplayColor.White:
                LogString_Output += "<color=#ffffffff>" + s + "</color>";
                break;
            case LogDisplayColor.Red:
                LogString_Output += "<color=#ff0000ff>" + s + "</color>";
                break;
            case LogDisplayColor.Green:
                LogString_Output += "<color=#00ff00ff>" + s + "</color>";
                break;
            default:
                break;
        }

        //１行or複数行
        switch (logDisplayLine)
        {
            case LogDisplayLine.Single:
                if (s.Length > SingleLineStringLengthMax)  LogString_Output += "\n";
                else LogString_Output += " ";
                break;
            case LogDisplayLine.Multiple:
                LogString_Output += "\n";
                break;
            default:
                break;
        }

        LogText_Update();
    }

    //Updateする
    //UIの操作などはMainThreadで行わなければならない(Dispatcherを介す)
    public void LogText_Update()
    {
        TotalManager.dispatcher.Invoke(() => {
            LogText_Output.text = LogString_Output;
            LogText_Output_InputField.text = SubstringTag(LogString_Output);
            ResizeInputField();
            UpdateLogInput("");
            GoToBottomOfLogContent();
        });
    }

    //改行する
    private void AddNewLine(int lineNum=1)
    {
        int n = 0;
        for (int i = LogString_Output.Length-1; i >= 0; i--)
        {
            if (LogString_Output[i]=='\n') n++;
            else if (LogString_Output[i] == ' ' || LogString_Output[i]=='<' || LogString_Output[i]=='/' || LogString_Output[i]=='c' || LogString_Output[i]=='o' || LogString_Output[i]=='l' || LogString_Output[i]=='r' || LogString_Output[i]=='>' ) continue;
            else break;
        }

        string t = "";
        for (int i = 0; i < lineNum-n; i++) t += "\n";

        LogString_Output += t;

        TotalManager.dispatcher.Invoke(() => {
            LogText_Output.text = LogString_Output;
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
        LogText_Output_InputField_RectTransform.sizeDelta = LogText_Output_RectTransform.sizeDelta;
    }

    //inputの内容をLogにもリアルタイムで反映する
    public void UpdateLogInput(string s)
    {
        LogText_Input.text = Command.NowReactiveProcessName + "> " + s;
    }

    //Unity rich textに用いているタグを消す
    private string SubstringTag(string s)
    {
        string t=s;
        t = Regex.Replace(t, "<color=#........>","");
        t = Regex.Replace(t, "</color>", "");
        return t;
    }

    /*
    Prefabを増やすことによって改行する場合
    //ShowLog
    public void Log_Show(string s, LogOption option)
    {
        if (s == "") return; //出力するものがある時のみ出力する
        //Debug.Log(s);

        //UIの操作などはMainThreadで行わなければならない
        //Dispatcherを介すことでMainThreadで行う
        TotalManager.dispatcher.Invoke(() => {
            if (LogText_Inst_Text == null) { Debug.Log("NOT FOUND LOGTEXT INSTANCE"); return; }
            switch (option.type)
            {
                case LogDisplayType.SingleLine:
                    LogText_Inst_Text.text += s + " ";
                    break;
                case LogDisplayType.MultipleLine:
                    LogText_Inst_Text.text += s + "\n";
                    break;
                default:
                    break;
            }
            switch (option.color)
            {
                case LogDisplayColor.Black:
                    LogText_Inst_Text.color = Color.black;
                    break;
                case LogDisplayColor.White:
                    LogText_Inst_Text.color = Color.white;
                    break;
                case LogDisplayColor.Red:
                    LogText_Inst_Text.color = Color.red;
                    break;
                case LogDisplayColor.Green:
                    LogText_Inst_Text.color = Color.green;
                    break;
                default:
                    break;
            }
        });
    }


    //AddNewLogText
    public void AddNewLogText()
    {
        TotalManager.dispatcher.Invoke(() => {
            if (LogText_Inst_Text == null)
            {
                GameObject gm = GameObject.Instantiate(LogText_Pref);
                gm.transform.SetParent(LogOutputContent.transform, false);
                LogText_Inst_Text = gm.GetComponent<Text>();
            };

            LogText_Inst_Text.text += "\n";
        });
    }


    public void AddLogSizeControlEvent()
    {
        //LogSizeControlUI.OnPointerDown.
    }
    */
}
