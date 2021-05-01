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
        inputField.onValueChanged.AddListener((s) =>{
            output.UpdateLogInput(s);
        });

        //buttonクリックでコマンド実行
        commandSubmitButton.onClick.AddListener(() => {
            if (Regex.IsMatch(inputField.text, "^( )*$")) { UnityEngine.Debug.Log("Empty value for inputField"); return; }

            command.Execute(inputField.text); 
            inputField.text = "";
            output.UpdateLogInput(inputField.text);
        });

        /*
        //enterでコマンド実行
        inputField.onEndEdit.AddListener((string text) => {
            command.Execute(text);
            inputField.text = "";
        });
        */       
    }

    
}
