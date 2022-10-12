using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Net.Sockets;

public class buttonsInput : MonoBehaviour
{
    public Button serverbutton;
    public Button clientbutton;
    public Button sendbutton;
    public GameObject inputBox;
    public GameObject TCPserver;
    public GameObject TCPclient;
    public GameObject UDPserver;
    public GameObject UDPclient;
    public GameObject changeTypeObject;
    Change_Type change;
    string inputText;
    string conectionType="-";
    // Start is called before the first frame update
    void Start()
    {
        change = changeTypeObject.GetComponent<Change_Type>();
        Button SB = serverbutton.GetComponent<Button>();
        Button CS = clientbutton.GetComponent<Button>();
        Button SendButton = sendbutton.GetComponent<Button>();
        SB.onClick.AddListener(ServerTaskOnClick);
        CS.onClick.AddListener(ClientTaskOnClick);
        SendButton.onClick.AddListener(SendTaskOnClick);


        var input = inputBox.GetComponent<InputField>();
        var se = new InputField.EndEditEvent();
        se.AddListener(updatetext);
        input.onEndEdit = se;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void  updatetext(string arg0)
    {
        inputText = arg0;
    }
    void ServerTaskOnClick()
    {
        
        if (conectionType == "-")
        {
            conectionType = "server";
            if (change.isTCP) TCPserver.GetComponent<TCPServer>().ToCreateServer = true;
            //else UDPserver.GetComponent<UDP_Server>().ToCreateServer = true;

            changeTypeObject.SetActive(false);
        }
        
    }
    void ClientTaskOnClick()
    {
        
        if (conectionType == "-")
        {
            conectionType = "client";
            if (change.isTCP) TCPclient.GetComponent<TCPClient>().ToCreateclient = true;
            else UDPclient.GetComponent<UDP_Client>().ToCreateClient = true;

            changeTypeObject.SetActive(false);
        }
    }
    void SendTaskOnClick()
    {
        switch (conectionType)
        {
            case "server":
                inputText = "server: " + inputText;
                if (change.isTCP)
                {
                    TCPserver.GetComponent<TCPServer>().outputText = inputText;
                    TCPserver.GetComponent<TCPServer>().PrepareToSend = true;
                }
                else
                {
                    //UDPserver.GetComponent<UDP_Server>().outputText = inputText;
                    //UDPserver.GetComponent<UDP_Server>().PrepareToSend = true;
                }
                break;
            case "client":
                inputText = "cliente: " + inputText;
                if (change.isTCP)
                {
                    TCPclient.GetComponent<TCPClient>().outputText = inputText;
                    TCPclient.GetComponent<TCPClient>().PrepareToSend = true;
                }
                else
                {
                    UDPclient.GetComponent<UDP_Client>().outputText = inputText;
                    UDPclient.GetComponent<UDP_Client>().PrepareToSend = true;
                }
                break;
        }
        
    }
}
