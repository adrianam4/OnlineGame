using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UDP_Server : MonoBehaviour
{
    void Start()
    {
        int recv;
        byte[] data = new byte[1024];
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("10.0.103.19"), 1234);

        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        newsock.Bind(ipep);
        Debug.Log("Waiting for a client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);

        recv = newsock.ReceiveFrom(data, ref Remote);

        Debug.Log("Message received from {0}:");
        Debug.Log(Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        newsock.SendTo(data, data.Length, SocketFlags.None, Remote);

        //while (true)
        //{
        //    data = new byte[1024];
        //    recv = newsock.ReceiveFrom(data, ref Remote);

        //    Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        //    newsock.SendTo(data, recv, SocketFlags.None, Remote);
        //}
    }
}