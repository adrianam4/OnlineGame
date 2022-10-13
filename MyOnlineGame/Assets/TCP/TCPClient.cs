using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    Socket server;
    IPEndPoint ipep;
    int recv;
    byte[] data;
    public bool ToCreateclient = false;
    bool clientCreated = false;
    public string inputText;
    public string outputText;
    public bool PrepareToSend = false;
    bool doReceive = true;
    bool doSend = true;
    // Start is called before the first frame update
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);
        data = new byte[1024];

    }
    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);


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
                data = Encoding.ASCII.GetBytes(outputText);
                server.Send(data, outputText.Length, SocketFlags.None);
                PrepareToSend = false;
            }

        }
    }
    void receive()
    {
        while (doReceive)
        {
            recv = server.Receive(data);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(inputText);
        }

    }
    // Update is called once per frame
    void Update()
    {
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
}
