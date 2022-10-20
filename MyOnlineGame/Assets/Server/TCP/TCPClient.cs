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

public class TCPClient : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    public Socket server;
    IPEndPoint ipep;
    int recv;
    public bool ToCreateclient = false;
    bool clientCreated = false;
    public string inputText;
    public string outputText;
    public bool PrepareToSend = false;
    bool doReceive = true;
    bool doSend = true;
    public string ipToConnect;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    // Start is called before the first frame update
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);
        //data = new byte[8192];

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
        messageReceived = false;
        messageSent = false;
    }

    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse(ipToConnect), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("hola2");

        try
        {
            clientCreated = true;
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server.");
            Debug.Log(e.ToString());
            return;
        }
        
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
                server.Send(data2, outputText.Length, SocketFlags.None);
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
            recv = server.Receive(data);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            messageReceived = true;
            Debug.Log(inputText);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (messageReceived && !inputText.IsNullOrEmpty())
            AddMessage("server: " + inputText);

        if (messageSent && !outputText.IsNullOrEmpty())
            AddMessage("client: " + outputText);

        if (ToCreateclient && !clientCreated)
        {
            if (!_t1.IsAlive)
            {
                _t1.Start();
            }
        }
        if (clientCreated)
        {
            if (!_t2.IsAlive)
            {
                _t2 = new Thread(send);
                _t2.Start();
            }
            if (!_t3.IsAlive)
            {
                _t3 = new Thread(receive);
                _t3.Start();
            }
        }
    }

    private void OnDisable()
    {
        server.Close();
        Debug.Log("Stopping client");
    }

    public void EndTCPConnection()
    {
        server.Close();
    }
}
