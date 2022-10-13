using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

public class UDP_Client : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    Socket server;
    IPEndPoint ipep;
    int recv;
    byte[] data;
    public bool ToCreateClient = false;
    bool clientCreated = false;
    public bool PrepareToSend = false;
    public string inputText;
    public string outputText;
    IPEndPoint sender;
    EndPoint Remote;
    bool doReceive = true;
    bool doSend = true;
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);
        data = new byte[8192];
    }

    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse("10.0.103.33"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Log("hola2");

        string welcome = "Hello, are you there?";
        data = Encoding.ASCII.GetBytes(welcome);
        server.SendTo(data, data.Length, SocketFlags.None, ipep);

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        clientCreated = true;

        //server.Connect(ipep);
        data = new byte[8192];
        //recv = server.ReceiveFrom(data, ref Remote);

        //Debug.Log(Remote.ToString());
        //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend)
            {
                data = new byte[8192];
                data = Encoding.ASCII.GetBytes(outputText);
                server.SendTo(data, data.Length, SocketFlags.None, ipep);
                PrepareToSend = false;
            }
        }
    }

    void receive()
    {
        while (doReceive)
        {
            data = new byte[8192];
            recv = server.ReceiveFrom(data, ref Remote);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(inputText);
        }
    }

    private void Update()
    {
        if (ToCreateClient && !clientCreated)
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
        Debug.Log("Stopping client");
        server.Close();
    }
}