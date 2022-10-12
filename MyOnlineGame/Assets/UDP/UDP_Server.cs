using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class UDP_Server : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    Socket client;
    int recv;
    // Start is called before the first frame update
    public bool ToCreateServer = false;
    bool serverCreated = false;
    public bool PrepareToSend = false;
    byte[] data;
    public string outputText;
    public string inputText;
    IPEndPoint sender;
    EndPoint Remote;
    void Start()
    {
        _t1 = new Thread(createServer);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);
        data = new byte[1024];
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        newsock.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        recv = newsock.ReceiveFrom(data, ref Remote);
        Debug.Log(Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data,0,recv));
        //client = newsock.Accept();

        serverCreated = true;
    }

    void send()
    {
        while (true)
        {
            if (PrepareToSend)
            {
                data = Encoding.ASCII.GetBytes(outputText);
                client.SendTo(data,outputText.Length,SocketFlags.None, Remote);
                PrepareToSend = false;
            }

        }
    }

    void receive()
    {
        while (true)
        {
            recv = client.ReceiveFrom(data, ref Remote);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(inputText);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ToCreateServer && !serverCreated)
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
                _t2.Start();
            }
            if (!_t3.IsAlive)
            {
                _t3.Start();
            }
        }
    }
}