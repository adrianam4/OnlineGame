using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using TMPro;
using System;

public class buttonsInput : MonoBehaviour
{
    public Text infoButton;
    public Text ipInfo;
    public Button serverbutton;
    public Button clientbutton;
    public Button sendbutton;
    public Button playButton;
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
    public GameObject player;

    void Start()
    {
        change = changeTypeObject.GetComponent<Change_Type>();
        Button SB = serverbutton.GetComponent<Button>();
        Button CS = clientbutton.GetComponent<Button>();
        Button SendButton = sendbutton.GetComponent<Button>();
        SB.onClick.AddListener(ServerTaskOnClick);
        CS.onClick.AddListener(ClientTaskOnClick);
        SendButton.onClick.AddListener(SendTaskOnClick);
        playButton.onClick.AddListener(playTaskOnClick);

        var input = inputBox.GetComponent<InputField>();
        var se = new InputField.EndEditEvent();
        se.AddListener(updatetext);
        input.onEndEdit = se;
    }

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
    void playTaskOnClick()
    {

        if(TCPserver.GetComponent<TCPServer>().serverCreated)
        {
            

            TCPserver.GetComponent<TCPServer>().outputText = "StartGame";
            TCPserver.GetComponent<TCPServer>().PrepareToSend = true;

            UDPserver.GetComponent<UDP_Server>().makeSend = true;

            GameObject.Find("Canvas").SetActive(false);
            player.SetActive(true);
        }
        
    }

    void  updatetext(string arg0)
    {
        inputText = arg0;
    }
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    void ServerTaskOnClick()
    {
        if (conectionType == "-")
        {
            ipInfo.text="Server IP: "+GetLocalIPAddress();
            isServerOrClient = true;
            conectionType = "server";
            GameObject.Find("CHAT/Data").GetComponent<DataSerialization>().type = 0;
            UDPserver.GetComponent<UDP_Server>().ToCreateServer = true;
            TCPserver.GetComponent<TCPServer>().ToCreateServer = true;
            

            changeTypeObject.SetActive(false);
        }
        
    }

    void ClientTaskOnClick()
    {
        if (conectionType == "-")
        {
            isServerOrClient = true;
            conectionType = "client";
            UDPclient.GetComponent<UDP_Client>().ToCreateClient = true;
            GameObject.Find("CHAT/Data").GetComponent<DataSerialization>().type = 1;


            GameObject enemies = GameObject.Find("LEVEL/Enemies");


            TCPclient.GetComponent<TCPClient>().ToCreateclient = true;
            GameObject.Find("CHAT/Canvas/PlayButton").SetActive(false);

            changeTypeObject.SetActive(false);
        }
    }

    void SendTaskOnClick()
    {
        switch (conectionType)
        {
            case "server":  
                TCPserver.GetComponent<TCPServer>().outputText = inputText;
                TCPserver.GetComponent<TCPServer>().PrepareToSend = true;
                break;
            case "client":
                TCPclient.GetComponent<TCPClient>().outputText = inputText;
                TCPclient.GetComponent<TCPClient>().PrepareToSend = true;
                break; 
        }
    }
}
