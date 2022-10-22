using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using Unity.Tutorials.Core.Editor;

public class TCPServer : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private List<Thread> receiveList;
    private List<Socket> clientsList;
    int recv;
    public int NumOfClientsConnected;
    public bool ToCreateServer = false;
    bool serverCreated=false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    bool doReceive = true;
    bool doSend = true;
    public TCPClient clientTCP;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;

    void Start()
    {
        NumOfClientsConnected = 0;
        _t1 = new Thread(CreateServer);
        _t2 = new Thread(Send);
        clientsList = new List<Socket>();
        receiveList = new List<Thread>();

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void CreateServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);
        ToCreateServer = false;
        while (true)
        {
            newsock.Listen(10);
            Debug.Log("Waiting for a client...");

            clientsList.Add(newsock.Accept());
            receiveList.Add(new Thread(Receive));
            NumOfClientsConnected++;
            Debug.Log(receiveList.Count);
            serverCreated = true;
            Debug.Log("Connected with {0} at port {1}");
        }
    }
    void Send()
    {
        while (doSend)
        {
            if (PrepareToSend)
            {
                byte[] data2;
                data2 = new byte[8192];
                string tmp = "server: " + outputText;
                data2 = Encoding.ASCII.GetBytes(tmp);
                Debug.Log(clientsList.Count);
                for (int a = 0; a < clientsList.Count; a++)
                {
                    clientsList[a].Send(data2, tmp.Length, SocketFlags.None);
                }
                messageSent = true;
                PrepareToSend = false;
            }
        }
    }

    void RemoveClient(int a)
    {
        clientsList.Remove(clientsList[a]);
        NumOfClientsConnected--;
    }

    void SendToAllClients(string message,int a)
    {
        byte[] data2 = new byte[8192];
        string tmp = message;
        data2 = Encoding.ASCII.GetBytes(tmp);
        for (int b = 0; b < clientsList.Count; b++)
        {
            if (a != b)
            {
                clientsList[b].Send(data2, tmp.Length, SocketFlags.None);
            }          
        }
    }

    void Receive(object a)
    {
        while (doReceive)
        {
            byte[] data = new byte[8192];
            recv = clientsList[(int)a].Receive(data);
            if (recv == 0)
            {
                RemoveClient((int)a);
                break;
            }
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            SendToAllClients(inputText,(int)a);
            messageReceived = true;
            Debug.Log(inputText);
        }
    }

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
                _t2 = new Thread(Send);
                _t2.Start();
            }
            for (int a = 0; a < clientsList.Count; a++)
            {

                if (!receiveList[a].IsAlive)
                {
                    receiveList[a] = new Thread(Receive);
                    receiveList[a].Start(a);
                }
            }
        }
    }
}
