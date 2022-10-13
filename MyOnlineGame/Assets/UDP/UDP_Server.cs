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
    private Thread UDPCreateServer;
    private Thread UDPSend;
    private Thread UDPRecieve;
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
    bool doReceive = true;
    bool doSend = true;
    void Start()
    {
        UDPCreateServer = new Thread(createServer);
        UDPSend = new Thread(send);
        UDPRecieve = new Thread(receive);
        data = new byte[8192];
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        client.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        //recv = newsock.ReceiveFrom(data, ref Remote);
        //Debug.Log(Remote.ToString());
        //Debug.Log(Encoding.ASCII.GetString(data,0,recv));
        //client = newsock.Accept();

        serverCreated = true;
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend)
            {
                data = new byte[8192];
                data = Encoding.ASCII.GetBytes(outputText);
                client.SendTo(data, data.Length,SocketFlags.None, Remote);
                PrepareToSend = false;
            }

        }
    }

    void receive()
    {
        while (doReceive)
        {
            data = new byte[8192];
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