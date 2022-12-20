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
    Vector3 serverPos;
    public List<int> otherCoinsDestroyed;
    int serverCoinDestroyed=-1;
    GameObject enemies;


    public int yourenemyDownID;
    public List<int> otherenemyDownID;
    int serverenemydoenID=-1;
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("LEVEL/Enemies");
        newRotation = Quaternion.identity;
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
        UDPClient = GameObject.Find("UDPClient");
        rootObjects = new List<Vector3>();
        coins = GameObject.Find("LEVEL/Tokens");
        pointsManager = player.GetComponent<PointsManager>();
        serverPos = new Vector3();
        otherCoinsDestroyed = new List<int>();
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


                writer.Write(mycoinId);
                writer.Write(yourenemyDownID);
            }
        }
        else//server
        {
            writer.Write(player_position.x);
            writer.Write(player_position.y);
            Debug.Log("position x: " + player_position.x);
            Debug.Log("position y: " + player_position.y);
            writer.Write(clientList.Count);
            for(int a=0;a< clientList.Count; a++)
            {
                writer.Write(a);
                writer.Write(clientList[a].clientPosition.x);
                writer.Write(clientList[a].clientPosition.y);
            }
           
            writer.Write(pointsManager.playerPoints);

            int coinID= mycoinId;
            writer.Write(coinID);
            for (int a = 0; a < clientList.Count; a++)
            {
                writer.Write(otherCoinsDestroyed[a]);

            }
            writer.Write(yourenemyDownID);
            for (int a = 0; a < clientList.Count; a++)
            {
                writer.Write(otherenemyDownID[a]);

            }
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
            //Debug.Log("position x: " + newPositionX);
            float newPositionY = reader.ReadSingle();
            //Debug.Log("position y: " + newPositionY);
            pointsManager.player1Points = reader.ReadInt32();

            
            clientList[u].clientPosition.Set(newPositionX, newPositionY,0);

            otherCoinsDestroyed[u] = reader.ReadInt32();
            otherenemyDownID[u]= reader.ReadInt32();
        }
        else//if is a client
        {

            float newPositionServerX = reader.ReadSingle();
            float newPositionServerY = reader.ReadSingle();
            
            serverPos.Set(newPositionServerX, newPositionServerY, 0);

            Debug.Log("position x: " + newPositionServerX);
            Debug.Log("position y: " + newPositionServerY);

            int numberOfClient = reader.ReadInt32();
            number = numberOfClient;
            toMake = true;
            for (int a = 0; a < numberOfClient; a++)
            {
                
                
                ///////////////////////////////////////////////////////////////////////////////
                int player= reader.ReadInt32();
                float newPositionX = reader.ReadSingle();
                //Debug.Log("position x: " + newPositionX);
                float newPositionY = reader.ReadSingle();
                clientsToClient[a].clientPosition.Set(newPositionX, newPositionY,0);
                clientsToClient[a].clientID = player;
                

            }
            pointsManager.player1Points = reader.ReadInt32();

            serverCoinDestroyed = reader.ReadInt32();

            for (int b = 0; b < numberOfClient; b++)
            {
                otherCoinsDestroyed[b] = reader.ReadInt32();
            }
            serverenemydoenID=reader.ReadInt32();
            for (int b = 0; b < numberOfClient; b++)
            {
                otherenemyDownID[b] = reader.ReadInt32();
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
            else if(ServerClient == 1)//client
            {
                int b = 0;
                Player2.transform.SetPositionAndRotation(serverPos, newRotation);
                for (int a = 0; a < clientsToClient.Count; a++)
                {
                    
                    if (a!= UDPClient.GetComponent<UDP_Client>().playerID)
                    {
                        if (b == 0)
                        {
                            Player3.transform.SetPositionAndRotation(clientsToClient[a].clientPosition, newRotation);
                        }else if (b == 1)
                        {
                            Player4.transform.SetPositionAndRotation(clientsToClient[a].clientPosition, newRotation);
                        }
                        b++;
                        
                    }

                }



            }
            deserialized = false;
            
        }
        newEnemyDead();
        DestroyCoin();

        if (toMake)
        {
            if (first)
            {
                otherenemyDownID = new List<int>();
                clientsToClient = new List<clientStructure>();
                for (int b = 0; b < number; b++)
                {
                    otherenemyDownID.Add(-1);
                    otherCoinsDestroyed.Add(-1);
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
        mycoinId = id;
    }
    void DestroyCoin()
    {
        if (serverCoinDestroyed!=-1)
        {
            for (int b = 0; b < otherCoinsDestroyed.Count; b++)
            {
                for (int a = 0; a < coins.transform.childCount; a++)
                {

                    if (serverCoinDestroyed == coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().id)
                    {
                        coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().collected = true;
                    }
                }


            }


        }

        for (int b = 0; b < otherCoinsDestroyed.Count; b++) {
            for (int a = 0; a < coins.transform.childCount; a++)
            {

                if (otherCoinsDestroyed[b] == coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().id)
                {
                    coins.transform.GetChild(a).GetComponent<Platformer.Mechanics.TokenInstance>().collected = true;
                }
            }


        }
        

    }
    public void newEnemyDead()
    {
        for(int a = 0; a < otherenemyDownID.Count; a++)
        {
            for(int b = 0;b< enemies.transform.childCount; b++)
            {
                if (enemies.transform.GetChild(b).name == "Enemy")
                {
                    if (enemies.transform.GetChild(b).gameObject.GetComponent<Platformer.Mechanics.EnemyController>().isDying == false) {
                        if (enemies.transform.GetChild(b).gameObject.GetComponent<Platformer.Mechanics.EnemyController>().id == otherenemyDownID[a]|| enemies.transform.GetChild(b).gameObject.GetComponent<Platformer.Mechanics.EnemyController>().id == serverenemydoenID)
                        {
                            enemies.transform.GetChild(b).gameObject.GetComponent<Platformer.Mechanics.EnemyController>().isDying = true;
                        }
                    }
                }
            }
        }
        //enemies.transform.GetChild(id+1).gameObject.SetActive(false);
    }
}