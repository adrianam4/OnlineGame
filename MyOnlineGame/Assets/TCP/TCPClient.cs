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
    Socket server;
    IPEndPoint ipep;
    int recv;
    byte[] data;
    public bool ToCreateclient = false;
    bool clientCreated = false;
    string text;
    // Start is called before the first frame update
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(sendAndRecibeFunction);
        data = new byte[1024];

    }
    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);


        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("hola2");

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server.");
            Debug.Log(e.ToString());
            return;
        }
        clientCreated = true;
    }
    void sendAndRecibeFunction()
    {
        while (true)
        {

            recv = server.Receive(data);
            text = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(text);
            //if (PrepareToSend)
            //{
            //    data = Encoding.ASCII.GetBytes(text);
            //    client.Send(data, recv, SocketFlags.None);
            //    PrepareToSend = false;
            //}

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
                _t2.Start();
            }
        }
    }
}
