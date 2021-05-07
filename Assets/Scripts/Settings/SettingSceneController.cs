using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSceneController : MonoBehaviour
{

    [SerializeField] InputField ShellFileName;
    [SerializeField] InputField ShellArguments;
    [SerializeField] InputField WorkingDirectory;
    

    private void Awake()
    {
        ShellFileName.text = PlayerPrefs.GetString( Command.SettingName.ShellFileName.ToString() );
        ShellArguments.text = PlayerPrefs.GetString(Command.SettingName.ShellArguments.ToString());
        WorkingDirectory.text = PlayerPrefs.GetString(Command.SettingName.WorkingDirectory.ToString());
        
        SetInputEvents();
    }

    private void SetInputEvents()
    {
        ShellFileName.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.ShellFileName.ToString(), ShellFileName.text); });
        ShellArguments.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.ShellArguments.ToString(), ShellArguments.text); });
        WorkingDirectory.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.WorkingDirectory.ToString(), WorkingDirectory.text); });
    }

}
