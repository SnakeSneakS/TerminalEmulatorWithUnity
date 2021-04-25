using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Input : MonoBehaviour
{
    [SerializeField] Command command;

    [SerializeField] InputField inputField;
    [SerializeField] Button commandSubmitButton;

    private void Start()
    {
        //buttonクリックでコマンド実行
        commandSubmitButton.onClick.AddListener(() => {
            command.Execute(inputField.text);
            inputField.text = "";
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
