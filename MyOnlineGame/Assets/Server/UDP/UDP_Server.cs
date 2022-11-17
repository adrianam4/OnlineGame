using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using TMPro;

public class UDP_Server : MonoBehaviour
{
    private Thread UDPCreateServer;
    private Thread UDPSend;
    private Thread UDPRecieve;
    public Socket client;
    int recv;
    public bool ToCreateServer = false;
    bool serverCreated = false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    public UDP_Client clientUDP;
    IPEndPoint sender;
    EndPoint Remote;
    bool doReceive = true;
    bool doSend = true;
    public GameObject chatObject;
    private TextMeshProUGUI chatText;
    private bool messageReceived = false;
    private bool messageSent = false;
    public byte[] sendData;
    public GameObject serializator;
    DataSerialization dataserialization;
    public bool makeSend = false;
    private bool doSerialize = true;
    private bool doDeserialize = true;
    private float time = 0;
    Vector3 auxiliar;
    void Start()
    {
        UDPCreateServer = new Thread(createServer);
        UDPSend = new Thread(send);
        UDPRecieve = new Thread(receive);
        auxiliar = new Vector3();
        chatText = chatObject.GetComponentInChildren<TextMeshProUGUI>();
        dataserialization = serializator.GetComponent<DataSerialization>();
    }

    void AddMessage(string newMessage)
    {
        chatText.text += (newMessage + "\n");
    }

    void createServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        client.Bind(ipep);
        Debug.Log("Waiting for a client...");

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        serverCreated = true;
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend && doSerialize)
            {

                sendData = dataserialization.Serialize(0);
                client.SendTo(sendData, sendData.Length, SocketFlags.None, Remote);
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

                recv = client.ReceiveFrom(data, ref Remote);

                if (recv > 0)
                {
                    inputText = Encoding.ASCII.GetString(data, 0, recv);
                    messageReceived = true;
                    Debug.Log(inputText);
                    dataserialization.Deserialize(data,0);
                }

                doDeserialize = false;
            }
        }
    }
    void setAllEnemis()
    {
        GameObject enemies = GameObject.Find("LEVEL/Enemies");
        

        for (int a=0;a< enemies.transform.childCount; a++)
        {
            if (enemies.transform.GetChild(a).name == "Enemy")
            {
                auxiliar.Set(enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().id, enemies.transform.GetChild(a).transform.position.x, enemies.transform.GetChild(a).transform.position.y);
                dataserialization.rootObjects.Add(auxiliar);
            }
                       
        }
        
        
    }
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.03)
        {
            setAllEnemis();
            doSerialize = true;
            time = 0;
        }
        doDeserialize = true;

        if (messageReceived && inputText.Length > 0)
        {
            AddMessage(inputText);
            messageReceived = false;
        }

        
        if (makeSend)
        {
            PrepareToSend = true;
        }
        if (messageSent && outputText.Length > 0)
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
            if (!UDPRecieve.IsAlive)
            {
                UDPRecieve = new Thread(receive);
                UDPRecieve.Start();
            }
        }
    }
}