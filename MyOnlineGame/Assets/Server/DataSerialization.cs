using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class clientStructure
{
    public int clientID;
    public Vector3 clientPosition;
}
public class DataSerialization : MonoBehaviour
{
    GameObject coins;
    static MemoryStream stream;
    public GameObject player;
    GameObject UDPServer;
    GameObject UDPClient;
    Quaternion newRotation;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    bool first = true;
    bool toMake = false;
    byte[] data;
    Vector3 player_position;
    private bool deserialized = false;
    private PointsManager pointsManager;

    bool otherCoinDestroyed = false;
    int othercoinId = -1;
    bool myCoinDestroyed = false;
    int mycoinId = -1;
    public List<Vector3> rootObjects;
    public int type = -1;

    public bool enemyDown = false;
    public int enemyDownId = -1;
    public List<clientStructure> clientList;
    List<clientStructure> clientsToClient;
    int ServerClient;
    public int playerIDEN;
    int number;
    // Start is called before the first frame update
    void Start()
    {
        newRotation = Quaternion.identity;
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
        UDPClient = GameObject.Find("UDPClient");
        rootObjects = new List<Vector3>();
        coins = GameObject.Find("LEVEL/Tokens");
        pointsManager = player.GetComponent<PointsManager>();
        
        
    }

    public byte[] Serialize(int id)
    {
        ServerClient = id;
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        int auxiiar = playerIDEN;
        if (id == 1)//if is client
        {
            if (auxiiar != -1)
            {
                writer.Write(auxiiar);
                writer.Write(player_position.x);
                writer.Write(player_position.y);
                writer.Write(pointsManager.playerPoints);


                writer.Write(myCoinDestroyed);
                writer.Write(mycoinId);
            }
        }
        else
        {
            writer.Write(clientList.Count);
            for(int a=0;a< clientList.Count; a++)
            {
                writer.Write(a);
                writer.Write(clientList[a].clientPosition.x);
                writer.Write(clientList[a].clientPosition.y);
            }
           
            writer.Write(pointsManager.playerPoints);


            writer.Write(myCoinDestroyed);
            writer.Write(mycoinId);
        }
        
       

        return stream.ToArray();
    }

    public void Deserialize(byte[] data,int id)
    {
        ServerClient = id;
        stream = new MemoryStream();
        stream.Write(data, 0, data.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        if (id == 0)//if is server
        {
            int u = reader.ReadInt32();
            float newPositionX = reader.ReadSingle();
            Debug.Log("position x: " + newPositionX);
            float newPositionY = reader.ReadSingle();
            Debug.Log("position y: " + newPositionY);
            pointsManager.player1Points = reader.ReadInt32();
            otherCoinDestroyed = reader.ReadBoolean();

            othercoinId = reader.ReadInt32();
            clientList[u].clientPosition.Set(newPositionX, newPositionY,0);
        }
        else//if is a client
        {
            int numberOfClient = reader.ReadInt32();
            number = numberOfClient;
            for (int a = 0; a < numberOfClient; a++)
            {
                toMake = true;
                
                ///////////////////////////////////////////////////////////////////////////////
                int player= reader.ReadInt32();
                float newPositionX = reader.ReadSingle();
                Debug.Log("position x: " + newPositionX);
                float newPositionY = reader.ReadSingle();
                clientsToClient[a].clientPosition.Set(newPositionX, newPositionY,0);
                clientsToClient[a].clientID = player;
                pointsManager.player1Points = reader.ReadInt32();
                otherCoinDestroyed = reader.ReadBoolean();

                othercoinId = reader.ReadInt32();

            }

        }
        deserialized = true;
    }
    private void Update()
    {
        if (deserialized)
        {
            if (ServerClient == 0)//server
            {
                for (int a = 0; a < clientList.Count; a++)
                {
                    switch (a)
                    {
                        case 0:
                            Player2.transform.SetPositionAndRotation(clientList[0].clientPosition, newRotation);
                            break;
                        case 1:
                            Player3.transform.SetPositionAndRotation(clientList[1].clientPosition, newRotation);
                            break;
                        case 2:
                            Player4.transform.SetPositionAndRotation(clientList[2].clientPosition, newRotation);
                            break;
                    }

                }
            }
            else
            {
                for (int a = 0; a < clientsToClient.Count; a++)
                {
                    if(a!= UDPClient.GetComponent<UDP_Client>().playerID)
                    {
                        switch (a)
                        {
                            case 0:
                                Player2.transform.SetPositionAndRotation(clientList[0].clientPosition, newRotation);
                                break;
                            case 1:
                                Player3.transform.SetPositionAndRotation(clientList[1].clientPosition, newRotation);
                                break;
                            case 2:
                                Player4.transform.SetPositionAndRotation(clientList[2].clientPosition, newRotation);
                                break;
                        }



                    }

                }



            }
            deserialized = false;
            
        }
        if (otherCoinDestroyed)
        {

            DestroyCoin();
        }

        if (toMake)
        {
            if (first)
            {
                clientsToClient = new List<clientStructure>();
                for (int b = 0; b < number; b++)
                {
                    clientsToClient.Add(new clientStructure());
                    clientsToClient[b].clientPosition = new Vector3(0, 0, 0);
                }
                first = false;
            }
            
        }
        player_position.Set(player.transform.position.x, player.transform.position.y, 0);
    }
    public void setDestroiedCoin(int id)
    {
        myCoinDestroyed = true;
        mycoinId = id;
    }
    void DestroyCoin()
    {
        otherCoinDestroyed = false;
        for (int a = 0; a < coins.transform.childCount; a++)
        {
           
            if (othercoinId == coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().id)
            {
                coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().collected = true;
            }
        }

    }
}