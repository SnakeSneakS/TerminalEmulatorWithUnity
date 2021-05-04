using System;
using UnityEngine;
using System.Text.RegularExpressions;

public partial class Login : MonoBehaviour
{
    
    /*
    private string[] GetSerialPortNames()
    {
        string[] ports = System.IO.Ports.SerialPort.GetPortNames();
        string[] ports_tty=new string[] { };
        foreach (string port in ports)
        { 
            if (port.Contains("tty") )
            { 
                //Debug.Log("PORT: "+port);
                string[] new_ports_tty = new string[ports_tty.Length+1];
                Array.Copy(new_ports_tty, ports_tty, ports_tty.Length);
                new_ports_tty[ports_tty.Length] = port;
                ports_tty = new_ports_tty;
            }
        }
        Debug.Log("ports_tty[0]: "+ports_tty[0]);
        return ports_tty;
    }
    */
}
