using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setIP : MonoBehaviour
{
    public GameObject textField;
    public GameObject TCPClient;
    public GameObject UDPClient;
    public void SetIP()
    {
        Text test = textField.GetComponent<Text>(); 
        TCPClient.GetComponent<TCPClient>().ipToConnect = test.text;
        UDPClient.GetComponent<UDP_Client>().ipToConnect = test.text;
    }
}