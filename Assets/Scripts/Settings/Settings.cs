using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField] InputField ShFileName;
    [SerializeField] InputField WorkingDirectory;
    [SerializeField] InputField PATH;

    private void Awake()
    {
        ShFileName.text = PlayerPrefs.GetString( Command.SettingName.ShFileName.ToString() );
        WorkingDirectory.text = PlayerPrefs.GetString(Command.SettingName.WorkingDirectory.ToString());
        PATH.text= PlayerPrefs.GetString(Command.SettingName.PATH.ToString());

        SetInputEvents();
    }

    private void SetInputEvents()
    {
        ShFileName.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.ShFileName.ToString(), ShFileName.text); });
        WorkingDirectory.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.WorkingDirectory.ToString(), WorkingDirectory.text); });
        PATH.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.PATH.ToString(), PATH.text); });
    }

}
