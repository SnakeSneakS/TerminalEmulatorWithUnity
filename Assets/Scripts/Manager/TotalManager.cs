using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalManager : MonoBehaviour
{
    public static Dispatcher dispatcher;

    private void Awake()
    {
        dispatcher = new Dispatcher();
    }

    private void Update()
    {
        dispatcher.Execute();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }
}
