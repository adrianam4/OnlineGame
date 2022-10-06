using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UDP_Client
{
    public static void Start()
    {
        byte[] data = new byte[1024];
        string input, stringData;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("10.0.103.33"), 1234);

        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string welcome = "Hello, how you doing...";
        data = Encoding.ASCII.GetBytes(welcome);
        server.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;

        data = new byte[1024];
        int recv = server.ReceiveFrom(data, ref Remote);

        Debug.Log("Message received from:");
        Debug.Log(Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

        //while (true)
        //{
        //    input = Console.ReadLine();
        //    if (input == "exit")
        //        break;
        //    server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
        //    data = new byte[1024];
        //    recv = server.ReceiveFrom(data, ref Remote);
        //    stringData = Encoding.ASCII.GetString(data, 0, recv);
        //    Console.WriteLine(stringData);
        //}
        Console.WriteLine("Stopping client");
        server.Close();
    }
}