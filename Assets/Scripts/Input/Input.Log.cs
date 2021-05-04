using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Input.Log
public partial class Input : MonoBehaviour
{
    [SerializeField] Button Button_DisplayHistLineBack;
    [SerializeField] Button Button_DisplayHistLineNext;
    [SerializeField] Slider Slider_DisplayHistLine;
    [SerializeField] Text Text_DisplayHistLine;

    [SerializeField] Button Button_ClearLog;

    //Start
    private void LogEvent_Start()
    {
        Button_DisplayHistLineBack.onClick.AddListener(() => {
            output.Log_show( (output.myHistory.displayHistLine <= 0)?0:output.myHistory.displayHistLine-1 );
            UpdateHistLine();
        });
        Button_DisplayHistLineNext.onClick.AddListener(() => {
            output.Log_show( (output.myHistory.displayHistLine >= output.myHistory.writeHistLine) ? output.myHistory.writeHistLine : output.myHistory.displayHistLine+1 );
            UpdateHistLine();
        });
        Slider_DisplayHistLine.onValueChanged.AddListener((value) =>
        {
            output.Log_show((int)value);
            UpdateHistLine();
        });
        Button_ClearLog.onClick.AddListener(() =>
        {
            output.myHistory.Clear();
            UpdateHistLine();
        });
    }

    //Update
    private void LogEvent_Update()
    {

    }

    public void UpdateHistLine()
    {
        Text_DisplayHistLine.text = output.myHistory.displayHistLine.ToString();
        Slider_DisplayHistLine.maxValue = output.myHistory.writeHistLine;
        Slider_DisplayHistLine.value = output.myHistory.displayHistLine;
    }
}
