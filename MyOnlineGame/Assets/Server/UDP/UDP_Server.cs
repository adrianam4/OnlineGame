using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using TMPro;

public class UDP_Server : MonoBehaviour
{
    private Thread UDPCreateServer;
    private Thread UDPSend;
    private Thread UDPRecieve;
    public Socket client;
    int recv;
    public bool ToCreateServer = false;
    bool serverCreated = false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    public UDP_Client clientUDP;
    IPEndPoint sender;
    EndPoint Remote;
    bool doReceive = true;
    bool doSend = true;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    public byte[] sendData;
    public GameObject serializator;
    DataSerialization dataserialization;
    public bool makeSend = false;
    void Start()
    {
        UDPCreateServer = new Thread(createServer);
        UDPSend = new Thread(send);
        UDPRecieve = new Thread(receive);

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
        dataserialization = serializator.GetComponent<DataSerialization>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        client.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        serverCreated = true;
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend)
            {

                sendData = dataserialization.Serialize();
                //string tmp = "server: " + outputText;
                //sendData = Encoding.ASCII.GetBytes(tmp);
                client.SendTo(sendData, sendData.Length, SocketFlags.None, Remote);
                messageSent = true;
                PrepareToSend = false;
            }

        }
    }

    void receive()
    {
        while (doReceive)
        {
            byte[] data;
            data = new byte[8192];

            recv = client.ReceiveFrom(data, ref Remote);

            if (recv > 0)
            {
                inputText = Encoding.ASCII.GetString(data, 0, recv);
                messageReceived = true;
                Debug.Log(inputText);
                dataserialization.Deserialize(data);
            }
        }
    }

    void Update()
    {
        if (messageReceived && inputText.Length > 0)
        {
            AddMessage(inputText);
            messageReceived = false;
        }

        
        if (makeSend)
        {
            PrepareToSend = true;
        }
        if (messageSent && outputText.Length > 0)
        {
            AddMessage("server: " + outputText);
            messageSent = false;
        }

        if (ToCreateServer && !serverCreated)
        {
            if (!UDPCreateServer.IsAlive)
            {
                UDPCreateServer.Start();
            }
        }
        if (serverCreated)
        {
            if (!UDPSend.IsAlive)
            {
                UDPSend = new Thread(send);
                UDPSend.Start();
            }
            if (!UDPRecieve.IsAlive)
            {
                UDPRecieve = new Thread(receive);
                UDPRecieve.Start();
            }
        }
    }
}