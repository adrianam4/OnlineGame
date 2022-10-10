using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    Socket client;
    int recv;
    // Start is called before the first frame update
    public bool ToCreateServer = false;
    bool serverCreated=false;
    public bool PrepareToSend = false;
    byte[] data;
    public string text;
    void Start()
    {
        _t1 = new Thread(createServer);
        _t2 = new Thread(sendAndRecibeFunction);
        data = new byte[1024];
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
    void sendAndRecibeFunction()
    {
        while (true)
        {
            
            //recv = client.Receive(data);
            //if (recv == 0)
            //    break;
            //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            if (PrepareToSend)
            {
                data = Encoding.ASCII.GetBytes(text);
                client.Send(data, text.Length, SocketFlags.None);
                PrepareToSend = false;
            }
            
        }
    }
    // Update is called once per frame
    void Update()
    {
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
                _t2.Start();
            }
        }


    }
}
