using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using Unity.Tutorials.Core.Editor;

public class TCPServer : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread recieve;
    public Socket client;
    int recv;
    // Start is called before the first frame update
    public bool ToCreateServer = false;
    bool serverCreated=false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    bool doReceive = true;
    bool doSend = true;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;

    void Start()
    {
        _t1 = new Thread(createServer);
        _t2 = new Thread(send);
        recieve = new Thread(Recieve);
        //data = new byte[8192];

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
        messageReceived = false;
        messageSent = false;
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);


        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);
        Debug.Log("hola1");
        newsock.Listen(10);
        Debug.Log("Waiting for a client...");
        client = newsock.Accept();
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        serverCreated = true;
        Debug.Log("Connected with {0} at port {1}");
    }
    void send()
    {
        while (doSend)
        {
            if (PrepareToSend)
            {
                byte[] data2;
                data2 = new byte[8192];
                data2 = Encoding.ASCII.GetBytes(outputText);
                client.Send(data2, outputText.Length, SocketFlags.None);
                messageSent = true;
                PrepareToSend = false;
            }

        }
    }
    void Recieve()
    {
        while (doReceive)
        {
            byte[] data;
            data = new byte[8192];
            recv = client.Receive(data);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            messageReceived = true;
            Debug.Log(inputText);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (messageReceived && !inputText.IsNullOrEmpty())
            AddMessage("client: " + inputText);

        if (messageSent && !outputText.IsNullOrEmpty())
            AddMessage("server: " + outputText);

        if (ToCreateServer&& !serverCreated)
        {
            if (!_t1.IsAlive)
            {
                _t1.Start();
            }                    
        }
        if (serverCreated)
        {
            if (!_t2.IsAlive)
            {
                _t2 = new Thread(send);
                _t2.Start();
            }
            if (!recieve.IsAlive)
            {
                recieve = new Thread(Recieve);
                recieve.Start();
            }
        }


    }
}
