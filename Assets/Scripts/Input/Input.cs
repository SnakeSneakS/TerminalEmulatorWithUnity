using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public partial class Input : MonoBehaviour
{
    [SerializeField] Command command;
    [SerializeField] Output output;

    [SerializeField] InputField inputField;
    [SerializeField] Button commandSubmitButton;

    private void Start()
    {
        CommandEvent_Start();
        LogEvent_Start();
    }

    private void Update()
    {
        CommandEvent_Update();
    }

    
    public void EventWhenExecuteCommand()
    {
        UpdateHistLine();
        WhenExecuteCommand_InputCommand();
    }
}