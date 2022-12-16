using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class TCPServer : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private List<Thread> recieveList;
    private List<Socket> clientsList;
    int recv;
    public int NumOfClientsConnected;
    public bool ToCreateServer = false;
    public bool serverCreated=false;
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
        _t1 = new Thread(createServer);
        _t2 = new Thread(send);
        clientsList = new List<Socket>();
        recieveList = new List<Thread>();

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9000);
        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);
        ToCreateServer = false;
        while (true)
        {
            newsock.Listen(10);
            Debug.Log("Waiting for a client...");

            clientsList.Add(newsock.Accept());
            recieveList.Add(new Thread(Recieve));
            NumOfClientsConnected++;
            Debug.Log(recieveList.Count);
            serverCreated = true;
            Debug.Log("Connected with {0} at port {1}");
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

    void removeClient(int a)
    {
        clientsList.Remove(clientsList[a]);
        NumOfClientsConnected--;
    }

    void sendToAllClients(string message,int a)
    {
        byte[] data2;
        data2 = new byte[8192];
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

    void Recieve(object a)
    {
        while (doReceive)
        {
            byte[] data;
            data = new byte[8192];
            recv = clientsList[(int)a].Receive(data);
            if (recv == 0)
            {
                removeClient((int)a);
                break;
            }
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            sendToAllClients(inputText,(int)a);
            messageReceived = true;
            Debug.Log(inputText);
        }
    }
    public void toStartGame()
    {
        byte[] data2;
        data2 = new byte[100];
        string tmp = "PlayerID:";
        
        for (int b = 0; b < clientsList.Count; b++)
        {
            string auxiliar = tmp + b.ToString();
            data2 = Encoding.ASCII.GetBytes(tmp+b.ToString());
            clientsList[b].Send(data2, auxiliar.Length, SocketFlags.None);
        }
    }
    void Update()
    {
        if (messageReceived && inputText.Length > 0)
        {
            AddMessage(inputText);
            messageReceived = false;
        }

        if (messageSent && outputText.Length > 0)
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
                _t2 = new Thread(send);
                _t2.Start();
            }
            for (int a = 0; a < clientsList.Count; a++)
            {

                if (!recieveList[a].IsAlive)
                {
                    recieveList[a] = new Thread(Recieve);
                    recieveList[a].Start(a);
                }
            }
        }


    }
}
