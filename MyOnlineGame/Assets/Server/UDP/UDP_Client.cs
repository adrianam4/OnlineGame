using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using TMPro;

public class UDP_Client : MonoBehaviour
{
    private Thread _t1;
    private Thread _t2;
    private Thread _t3;
    public Socket server;
    IPEndPoint ipep;
    int recv;
    public bool ToCreateClient = false;
    bool clientCreated = false;
    public bool PrepareToSend = false;
    public string inputText;
    public string outputText;
    IPEndPoint sender;
    EndPoint Remote;
    bool doReceive = true;
    bool doSend = true;
    public string ipToConnect;
    public string username;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    public GameObject serializator;
    DataSerialization dataserialization;
    public bool makeSend = false;
    private bool doSerialize = true;
    private bool doDeserialize = true;
    private float time = 0;
    void Start()
    {
        _t1 = new Thread(CreateClient);
        _t2 = new Thread(send);
        _t3 = new Thread(receive);
        dataserialization = serializator.GetComponent<DataSerialization>();
        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void CreateClient()
    {
        ipep = new IPEndPoint(IPAddress.Parse(ipToConnect), 9050);
        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        clientCreated = true;
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend && doSerialize)
            {
                byte[] data2;
                data2 = new byte[8192];
                data2 = dataserialization.Serialize(1);
                //string tmp = username + ": " + outputText;
                //data2 = Encoding.ASCII.GetBytes(tmp);
                server.SendTo(data2, data2.Length, SocketFlags.None, ipep);
                messageSent = true;
                PrepareToSend = false;
                doSerialize = false;
            }
        }
    }

    void receive()
    {
        while (doReceive)
        {
            if (doDeserialize)
            {
                byte[] data;
                data = new byte[8192];

                recv = server.ReceiveFrom(data, ref Remote);

                if (recv > 0)
                {
                    inputText = Encoding.ASCII.GetString(data, 0, recv);
                    messageReceived = true;
                    Debug.Log(inputText);
                    dataserialization.Deserialize(data,1);
                }
                doDeserialize = false;
            }
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.03)
        {
            doSerialize = true;
            
            time = 0;
        }
        doDeserialize = true;

        if (messageReceived && inputText.Length > 0)
        {
            AddMessage(inputText);
            messageReceived = false;
        }

        if (messageSent && outputText.Length > 0)
        {
            AddMessage(username + ": " + outputText);
            messageSent = false;
        }
        if (makeSend)
        {
            PrepareToSend = true;
        }

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
        if (server != null)
        {
            server.Close();
            Debug.Log("Stopping client");
        }
    }
}