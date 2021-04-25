using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//Output_Log
public partial class Output : MonoBehaviour
{
    [SerializeField] ScrollRect LogOutputAreaScrollView;
    [SerializeField] GameObject LogOutputContent;
    [SerializeField] GameObject LogText_Pref;
    [SerializeField] Button LogSizeControlUI;

    private Text LogText_Inst_Text;

    private string receivedEventHandler_string;

    public enum LogDisplayType
    {
        SingleLine,
        MultipleLine,
    }

    public enum LogDisplayColor
    {
        Black,
        White,
        Red,
        Green,
    }

    //ログのオプション
    public class LogOption
    {
        public LogDisplayType type { get; set; }
        public LogDisplayColor color { get; set; }
        public LogOption(LogDisplayType type, LogDisplayColor color)
        {
            this.type = type;
            this.color = color;
        }
        public static LogOption NewSingleLineBlack()
        {
            return new LogOption(LogDisplayType.SingleLine, LogDisplayColor.Black);
        }
        public static LogOption NewSingleLineWhite()
        {
            return new LogOption(LogDisplayType.SingleLine, LogDisplayColor.White);
        }
        public static LogOption NewSingleLineRed()
        {
            return new LogOption(LogDisplayType.SingleLine, LogDisplayColor.Red);
        }
        public static LogOption NewSingleLineGreen()
        {
            return new LogOption(LogDisplayType.SingleLine, LogDisplayColor.Green);
        }
        public static LogOption NewMultipleLineBlack()
        {
            return new LogOption(LogDisplayType.MultipleLine, LogDisplayColor.Black);
        }
        public static LogOption NewMultipleLineWhite()
        {
            return new LogOption(LogDisplayType.MultipleLine, LogDisplayColor.White);
        }
    }

    //実行
    public void Log_execute(string s, LogOption option)
    {
        AddNewLogText();
        Log_Show("> "+s, option);
        UnityEngine.Debug.Log("Command: " + s);
        AddNewLogText();
    }

    public void Log_result(string s, LogOption option)
    {
        Log_Show(s, option);
    }

    public void Log_Show(string s, LogOption option)
    {
        if (s == "") return; //出力するものがある時のみ出力する
        UnityEngine.Debug.Log(s);

        //UIの操作などはMainThreadで行わなければならない
        //Dispatcherを介すことでMainThreadで行う
        TotalManager.dispatcher.Invoke(() => {
            if (LogText_Inst_Text == null) { Debug.Log("NOT FOUND LOGTEXT INSTANCE"); return; }

            switch (option.color)
            {
                case LogDisplayColor.Black:
                    LogText_Inst_Text.text += "<color=#000000ff>";
                    break;
                case LogDisplayColor.White:
                    LogText_Inst_Text.text += "<color=#ffffffff>";
                    break;
                case LogDisplayColor.Red:
                    LogText_Inst_Text.text += "<color=#ff0000ff>";
                    break;
                case LogDisplayColor.Green:
                    LogText_Inst_Text.text += "<color=#00ff00ff>";
                    break;
                default:
                    break;
            }
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
                case LogDisplayColor.White:
                case LogDisplayColor.Red:
                case LogDisplayColor.Green:
                    LogText_Inst_Text.text += "</color>";
                    break;
                default:
                    break;
            }

            GoToBottomOfLogContent();
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

    //Logの一番下へ移動する
    public void GoToBottomOfLogContent()
    {
        Canvas.ForceUpdateCanvases();
        LogOutputAreaScrollView.verticalNormalizedPosition = 0.0f;
        Canvas.ForceUpdateCanvases();
    }

    public void AddLogSizeControlEvent()
    {
        //LogSizeControlUI.OnPointerDown.
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
            if (LogText_Inst_Text != null && LogText_Inst_Text.text == "") return;

            GameObject gm = GameObject.Instantiate(LogText_Pref);
            gm.transform.SetParent(LogOutputContent.transform, false);
            LogText_Inst_Text = gm.GetComponent<Text>();
        });
    }

    public void AddLogSizeControlEvent()
    {
        //LogSizeControlUI.OnPointerDown.
    }
    */
}
