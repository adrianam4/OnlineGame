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
    private Thread UDPSend;
    private Thread UDPRecieve;
    int recv;
    public bool ToCreateServer = false;
    bool serverCreated = false;
    public bool PrepareToSend = false;
    public string outputText;
    public string inputText;
    public UDP_Client clientUDP;
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
    Socket udp;
    public Dictionary<EndPoint, UDP_Client> clients;
    int port = 9050;
    void Start()
    {
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
        clients = new Dictionary<EndPoint, UDP_Client>();
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                IPEndPoint endPoint = new IPEndPoint((IPAddress)ip, port);
                Debug.Log("Server IP Address: " + ip);
                Debug.Log("Port: " + port);
                udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udp.Bind(endPoint);
                udp.Blocking = false;
                break;
            }
        }
        

        

        serverCreated = true;
    }

    void send()
    {
        while (doSend)
        {
            if (PrepareToSend && doSerialize)
            {

                sendData = dataserialization.Serialize(0);
                foreach (KeyValuePair<EndPoint, UDP_Client> p in clients)
                    udp.SendTo(sendData, sendData.Length, SocketFlags.None, p.Key);
                messageSent = true;
                PrepareToSend = false;
                doSerialize = false;
            }

        }
    }
    void HandleNewClient(EndPoint addr, string data)
    {
        clients.Add(addr, new UDP_Client());

    }
    void receive()
    {
        while (doReceive)
        {
            if (doDeserialize&& udp.Available != 0)
            {
                byte[] data;
                data = new byte[8192];
                EndPoint sender = new IPEndPoint(IPAddress.Any, port);
                int recv = udp.ReceiveFrom(data, ref sender);

                string info = Encoding.Default.GetString(data);
                if (info[0] == 'n' )
                    HandleNewClient(sender, info);
                else if (recv > 0)              
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
                if (enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().path != null){
                    auxiliar.Set(enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().id, enemies.transform.GetChild(a).transform.position.x, enemies.transform.GetChild(a).transform.position.y);
                    dataserialization.rootObjects.Add(auxiliar);
                }
                
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
                createServer();           
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