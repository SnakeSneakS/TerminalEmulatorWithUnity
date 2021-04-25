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
            //UnityEngine.Debug.Log(Command._IsReactiveMode?"Reactive":"Not reactive");
            if (Command._IsReactiveMode == true)
            {
                if (Command.SW.BaseStream.CanWrite)
                {
                    Command.SW.WriteLine(inputField.text);
                    UnityEngine.Debug.Log("stream write: "+inputField.text);
                }
                else
                {
                    UnityEngine.Debug.Log("stream can't write");
                }
            }
            else
            {
                command.Execute(inputField.text);
            }

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
