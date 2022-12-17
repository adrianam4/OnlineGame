using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUsername : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TextField;
    public GameObject TCPClient;
    public GameObject UDPClient;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUser()
    {
        Text test = TextField.GetComponent<Text>();
        TCPClient.GetComponent<TCPClient>().username = test.text;
        UDPClient.GetComponent<UDP_Client>().username = test.text;
    }
}