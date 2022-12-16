using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class TCPClient : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    public Socket server;
    IPEndPoint ipep;
    int recv;
    public bool ToCreateclient = false;
    bool clientCreated = false;
    public string inputText;
    public string outputText;
    public bool PrepareToSend = false;
    bool doReceive = true;
    bool doSend = true;
    public string ipToConnect;
    public string username;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    public GameObject player;
    public GameObject levelCanvas;
    public UDP_Client udpClient;
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);

        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse(ipToConnect), 9000);
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientCreated = true;
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server.");
            Debug.Log(e.ToString());
            return;
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
                string tmp = username + ": " + outputText;
                data2 = Encoding.ASCII.GetBytes(tmp);
                server.Send(data2, tmp.Length, SocketFlags.None);
                messageSent = true;
                PrepareToSend = false;
            }

        }
    }

    void receive()
    {
        while (doReceive)
        {
            byte[] data;
            data = new byte[8192];
            recv = server.Receive(data);
            inputText = Encoding.ASCII.GetString(data, 0, recv);
            messageReceived = true;
            Debug.Log(inputText);
        }

    }

    void Update()
    {
        if (messageReceived && inputText.Length > 0)
        {
            string a = "PlayerID:";
            string auxiliar = inputText.Substring(9);
            if (inputText.Substring(0,9) == a)
            {
                GameObject.Find("Canvas").SetActive(false);
                player.SetActive(true);
                levelCanvas.SetActive(true);
                GameObject.Find("UDPClient").GetComponent<UDP_Client>().makeSend = true;            
                udpClient.GetComponent<UDP_Client>().playerID= Int32.Parse(auxiliar);
            }
            AddMessage(inputText);
            messageReceived = false;
            
        }

        if (messageSent && outputText.Length > 0)
        {
            AddMessage(username + ": " + outputText);
            messageSent = false;
        }

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
        if (server != null)
        {
            server.Close();
            Debug.Log("Stopping client");
        }
    }
}
