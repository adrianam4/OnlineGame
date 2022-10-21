using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using TMPro;
using Unity.Tutorials.Core.Editor;

public class UDP_Server : MonoBehaviour
{
    private Thread UDPCreateServer;
    private Thread UDPSend;
    private Thread UDPRecieve;
    public Socket client;
    private List<Thread> recieveList;
    private List<EndPoint> endPoints;
    int recv;
    public int NumOfClientsConnected;
    // Start is called before the first frame update
    public bool ToCreateServer = false;
    bool serverCreated = false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    public UDP_Client clientUDP;
    IPEndPoint sender;
    bool doReceive = true;
    bool doSend = true;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    void Start()
    {
        UDPCreateServer = new Thread(createServer);
        UDPSend = new Thread(send);
        UDPRecieve = new Thread(receive);
        //data = new byte[8192];
        endPoints=new List<EndPoint>();
        recieveList = new List<Thread>();
        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
        NumOfClientsConnected = 0;
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void createServer()
    {
        serverCreated = true;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        client.Bind(ipep);
        while (true)
        {
            string aux;

        
            Debug.Log("Waiting for a client...");
        
            sender = new IPEndPoint(IPAddress.Any, 0);
            endPoints.Add((EndPoint)(sender));
            recieveList.Add(new Thread(receive));
            byte[] data;
            data = new byte[8192];
            EndPoint auxiliar = endPoints[NumOfClientsConnected];
            recv = client.ReceiveFrom(data, ref auxiliar);
            aux = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(aux);
            NumOfClientsConnected++;
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
                string tmp = "server: " + outputText;
                data2 = Encoding.ASCII.GetBytes(tmp);
                for (int a = 0; a < NumOfClientsConnected; a++)
                {
                    client.SendTo(data2, data2.Length,SocketFlags.None, endPoints[a]);
                }
                messageSent = true;
                PrepareToSend = false;
            }

        }
    }
    void removeClient(int a)
    {
        endPoints.Remove(endPoints[a]);
        NumOfClientsConnected--;
    }
    void sendToAllClients(string message, int a)
    {
        byte[] data2;
        data2 = new byte[8192];
        string tmp = message;
        data2 = Encoding.ASCII.GetBytes(tmp);
        for (int b = 0; b < NumOfClientsConnected; b++)
        {
            if (a != b)
            {
                client.SendTo(data2, data2.Length, SocketFlags.None, endPoints[a]);
            }
        }
    }
    void receive(object a)
    {
        while (doReceive)
        {
            byte[] data;
            data = new byte[8192];
            EndPoint ep= endPoints[(int)a];
            recv = client.ReceiveFrom(data, ref ep);
            if (recv == 0)
            {
                removeClient((int)a);
                break;
            }
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            sendToAllClients(inputText, (int)a);
            messageReceived = true;
            Debug.Log(inputText);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (messageReceived && !inputText.IsNullOrEmpty())
        {
            AddMessage(inputText);
            messageReceived = false;
        }


        if (messageSent && !outputText.IsNullOrEmpty())
        {
            AddMessage("server: " + outputText);
            messageSent = false;
        }

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
            for (int a = 0; a < NumOfClientsConnected; a++)
            {

                if (!recieveList[a].IsAlive)
                {
                    recieveList[a] = new Thread(receive);
                    recieveList[a].Start(a);
                }
            }
        }
    }
}