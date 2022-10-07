using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCP : MonoBehaviour
{
    // Start is called before the first frame update

    bool conectionCreated = false;
    Socket server;
    IPEndPoint ipep;
    string input, stringData;
    string conectionType;
    byte[] data;
    Socket client;

    void Start()
    {
        data = new byte[1024];
    }
    void setClientStartConection()
    {
        //meter IP del otro
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);


        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Unable to connect to server.");
            Console.WriteLine(e.ToString());
            return;
        }


        int recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);

    }
    void setServerStartConection()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);


        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);

        newsock.Listen(10);
        Console.WriteLine("Waiting for a client...");
        client = newsock.Accept();
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

        Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        client.Send(data, data.Length, SocketFlags.None);

    }
    // Update is called once per frame
    void Update()
    {
        int recv;
        switch (conectionType)
        {
            case "server":
                if (!conectionCreated)
                {
                    setServerStartConection();
                }
                recv = client.Receive(data);
                if (recv == 0)
                    break;

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

                client.Send(data, recv, SocketFlags.None);

                break;
            case "client":

                if (!conectionCreated)
                {
                    setClientStartConection();
                }
                input = Console.ReadLine();
                if (input == "exit")
                    break;
                server.Send(Encoding.ASCII.GetBytes(input));

                recv = server.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(stringData);
                break;
        }



        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Unable to connect to server.");
            Console.WriteLine(e.ToString());
            return;
        }

        recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);
    }
}
