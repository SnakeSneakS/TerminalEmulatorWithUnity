using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Output
public partial class Output : MonoBehaviour
{
    [SerializeField] public Input input;

    public Original.MyHistory myHistory = new Original.MyHistory(50); //Original History にLogを保存する


    private void Awake()
    {
        GirlDialog_Awake();
    }

    public void WhenExecute(string s)
    {
        Log_execute(s);
        GirlDialog_ExecuteCommand();
        input.EventWhenExecuteCommand();
    }

    public void WhenSuccess(string s)
    {
        Log_success(s);
        GirlDialog_Success();
    }

    public void WhenError(string s)
    {
        Log_error(s);
        GirlDialog_Error();
    }
    public void WhenWarn(string s)
    {
        Log_warn(s);
    }
}
