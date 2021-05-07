using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Output
public partial class Output : MonoBehaviour
{
    [SerializeField] public Input input;

    private void Awake()
    {
        GirlDialog_Awake();
    }
}
