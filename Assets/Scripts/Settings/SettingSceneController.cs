using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSceneController : MonoBehaviour
{

    [SerializeField] InputField ShFileName;
    [SerializeField] InputField WorkingDirectory;

    private void Awake()
    {
        ShFileName.text = PlayerPrefs.GetString( Command.SettingName.ShFileName.ToString() );
        WorkingDirectory.text = PlayerPrefs.GetString(Command.SettingName.WorkingDirectory.ToString());

        SetInputEvents();
    }

    private void SetInputEvents()
    {
        ShFileName.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.ShFileName.ToString(), ShFileName.text); });
        WorkingDirectory.onEndEdit.AddListener(delegate { PlayerPrefs.SetString(Command.SettingName.WorkingDirectory.ToString(), WorkingDirectory.text); });
    }

}
