using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Net.Sockets;
using TMPro;
using System;
using Unity.Tutorials.Core.Editor;

public class buttonsInput : MonoBehaviour
{
    public Text infoButton;
    public Button serverButton;
    public Button clientButton;
    public Button sendButton;
    public GameObject inputBox;
    public GameObject TCPserver;
    public GameObject TCPclient;
    public GameObject UDPserver;
    public GameObject UDPclient;
    public GameObject changeTypeObject;
    Change_Type change;
    string inputText;
    string conectionType="-";
    bool isServerOrClient = false;
    // Start is called before the first frame update
    void Start()
    {
        change = changeTypeObject.GetComponent<Change_Type>();
        Button SB = serverButton.GetComponent<Button>();
        Button CS = clientButton.GetComponent<Button>();
        Button SendButton = sendButton.GetComponent<Button>();
        SB.onClick.AddListener(ServerTaskOnClick);
        CS.onClick.AddListener(ClientTaskOnClick);
        SendButton.onClick.AddListener(SendTaskOnClick);

        var input = inputBox.GetComponent<InputField>();
        var se = new InputField.EndEditEvent();
        se.AddListener(UpdateText);
        input.onEndEdit = se;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServerOrClient)
        {
            switch (conectionType)
            {
                case "server":

                    if (change.isTCP)
                    {
                        
                        if (TCPserver.GetComponent<TCPServer>().NumOfClientsConnected>0)
                        {
                            infoButton.text = TCPserver.GetComponent<TCPServer>().NumOfClientsConnected+ " Clients Connected";
                        }
                        else
                        {
                            infoButton.text = "No client Connected";
                        }
                    }
                    else
                    {
                        if (UDPserver.GetComponent<UDP_Server>().client != null)
                        {
                            infoButton.text = "Client Connected";
                        }
                        else
                        {
                            infoButton.text = "No client Connected";
                        }
                    }
                    break;
                case "client":

                    if (change.isTCP)
                    {
                        if (TCPclient.GetComponent<TCPClient>().server != null)
                        {
                            infoButton.text = "Connected to server";
                        }
                        else
                        {
                            infoButton.text = "No connected to server";
                        }
                    }
                    else
                    {
                        if (UDPclient.GetComponent<UDP_Client>().server != null)
                        {
                            infoButton.text = "Connected to server";
                        }
                        else
                        {
                            infoButton.text = "No connected to server";
                        }
                    }
                    break;
            }
        }
        else
        {
            infoButton.text = "No Connection Enabled";
        }
    }

    void  UpdateText(string arg0)
    {
        inputText = arg0;
    }

    void ServerTaskOnClick()
    {
        if (conectionType == "-")
        {
            isServerOrClient = true;
            conectionType = "server";
            
            if (change.isTCP) TCPserver.GetComponent<TCPServer>().ToCreateServer = true;
            else UDPserver.GetComponent<UDP_Server>().toCreateServer = true;

            changeTypeObject.SetActive(false);
        }
    }

    void ClientTaskOnClick()
    {
        if (conectionType == "-")
        {
            isServerOrClient = true;
            conectionType = "client";
            if (change.isTCP) TCPclient.GetComponent<TCPClient>().toCreateclient = true;
            else UDPclient.GetComponent<UDP_Client>().toCreateClient = true;

            changeTypeObject.SetActive(false);
        }
    }

    void SendTaskOnClick()
    {
        switch (conectionType)
        {
            case "server":
                
                if (change.isTCP)
                {
                    TCPserver.GetComponent<TCPServer>().outputText = inputText;
                    TCPserver.GetComponent<TCPServer>().PrepareToSend = true;
                }
                else
                {
                    UDPserver.GetComponent<UDP_Server>().outputText = inputText;
                    UDPserver.GetComponent<UDP_Server>().prepareToSend = true;
                }
                break;
            case "client":
                
                if (change.isTCP)
                {
                    TCPclient.GetComponent<TCPClient>().outputText = inputText;
                    TCPclient.GetComponent<TCPClient>().prepareToSend = true;
                }
                else
                {
                    UDPclient.GetComponent<UDP_Client>().outputText = inputText;
                    UDPclient.GetComponent<UDP_Client>().prepareToSend = true;
                }
                break; 
        }
    }
}