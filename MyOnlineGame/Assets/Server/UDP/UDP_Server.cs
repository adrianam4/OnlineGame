using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using TMPro;
using Unity.Tutorials.Core.Editor;

public class UDP_Server : MonoBehaviour
{
    private Thread UDPCreateServer;
    private Thread UDPSend;
    private Thread UDPRecieve;
    public Socket client;
    int recv;
    public bool toCreateServer = false;
    bool serverCreated = false;
    public bool prepareToSend = false;
    public string outputText;
    public string inputText;
    public UDP_Client clientUDP;
    IPEndPoint sender;
    EndPoint remote;
    bool doReceive = true;
    bool doSend = true;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;

    void Start()
    {
        UDPCreateServer = new Thread(CreateServer);
        UDPSend = new Thread(Send);
        UDPRecieve = new Thread(Receive);
        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void CreateServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        client.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        remote = (EndPoint)(sender);
        serverCreated = true;
        Debug.Log("Connected with {0} at port {1}");
    }

    void Send()
    {
        while (doSend)
        {
            if (prepareToSend)
            {
                byte[] data2 = new byte[8192];
                string tmp = "server: " + outputText;
                data2 = Encoding.ASCII.GetBytes(tmp);
                client.SendTo(data2, data2.Length,SocketFlags.None, remote);
                messageSent = true;
                prepareToSend = false;
            }

        }
    }

    void Receive()
    {
        while (doReceive)
        {
            byte[] data = new byte[8192];
            recv = client.ReceiveFrom(data, ref remote);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            messageReceived = true;
            Debug.Log(inputText);
        }
    }

    void Update()
    {
        if (messageReceived && !inputText.IsNullOrEmpty())
        {
            AddMessage(inputText);
            messageReceived = false;
        }

        if (messageSent && !outputText.IsNullOrEmpty())
        {
            AddMessage("server: " + outputText);
            messageSent = false;
        }

        if (toCreateServer && !serverCreated)
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
                UDPSend = new Thread(Send);
                UDPSend.Start();
            }
            if (!UDPRecieve.IsAlive)
            {
                UDPRecieve = new Thread(Receive);
                UDPRecieve.Start();
            }
        }
    }
}