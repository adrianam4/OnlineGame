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
    public int type = -1;

    public bool enemyDown = false;
    public int enemyDownId = -1;
    public List<clientStructure> clientList;
    List<clientStructure> clientsToClient;
    public int ServerClient = -1;
    public int playerIDEN;
    public int number;
    Vector3 serverPos;
    public List<int> otherCoinsDestroyed;
    int serverCoinDestroyed = -1;
    GameObject enemies;


    public int yourenemyDownID;
    public List<int> otherenemyDownID;
    int serverenemydoenID = -1;

    public List<Vector3> player2;
    public List<Vector3> player3;
    public List<Vector3> player4;

    int player2Max = 0;
    int player3Max = 0;
    int player4Max = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("LEVEL/Enemies");
        newRotation = Quaternion.identity;
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
        UDPClient = GameObject.Find("UDPClient");
        coins = GameObject.Find("LEVEL/Tokens");
        pointsManager = player.GetComponent<PointsManager>();
        serverPos = new Vector3();
        otherCoinsDestroyed = new List<int>();
    }

    public byte[] Serialize(int id,int client)
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
            //if (client == 2)
            //{
            //    writer.Write(0);
            //    for (int a = 0; a < player2Max; a++)
            //    {
                    
            //        writer.Write(player2[a].x);
            //        writer.Write(player2[a].y);
            //        writer.Write(player2[a].z);
            //    }
            //    player2Max = 0;
            //}
            //else if(client == 3)
            //{
            //    writer.Write(0);
            //    for (int a = 0; a < player3Max; a++)
            //    {
                    
            //        writer.Write(player3[a].x);
            //        writer.Write(player3[a].y);
            //        writer.Write(player3[a].z);
            //    }
            //    player3Max = 0;
            //}
            //else if (client == 4)
            //{
            //    writer.Write(0);
            //    for (int a = 0; a < player4Max; a++)
            //    {
                    
            //        writer.Write(player4[a].x);
            //        writer.Write(player4[a].y);
            //        writer.Write(player4[a].z);
            //    }
            //    player4Max = 0;
            //}
            writer.Write(pointsManager.playerPoints);
            writer.Write(pointsManager.player1Points);
            writer.Write(pointsManager.player2Points);
            writer.Write(pointsManager.player3Points);
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
            switch (u)
            {
                case 0:
                    pointsManager.player1Points = reader.ReadInt32();
                    break;
                case 1:
                    pointsManager.player2Points = reader.ReadInt32();
                    break;
                case 2:
                    pointsManager.player3Points = reader.ReadInt32();
                    break;
            }
            
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
            //int cantOfenemys = reader.ReadInt32();
            //for (int b = 0; b < cantOfenemys; b++)
            //{
            //    float x = reader.ReadSingle();
            //    float y = reader.ReadSingle();
            //    float enemyID = reader.ReadInt32();
            //    player2[b].Set(x,y, enemyID);
            //}
            pointsManager.player1Points = reader.ReadInt32();
            pointsManager.player2Points = reader.ReadInt32();
            pointsManager.player3Points = reader.ReadInt32();
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
                Vector3 y = new Vector3();
                for (int a = 0; a < player2.Count; a++)
                {
                    y.Set(player2[a].x, player2[a].y, 0);
                    int s = (int)player2[a].z;
                    enemies.transform.GetChild(s + 1).gameObject.transform.SetPositionAndRotation(y, newRotation);
                }



            }
            deserialized = false;
            
        }
        if (ServerClient == 0)//server
        {

            
            setEnemyPosition();

        }
        else if(ServerClient == 1)//client
        {
            
            
        }
        
        newEnemyDead();
        DestroyCoin();

        if (toMake)
        {
            if (first)
            {
                for (int b = 0; b < enemies.transform.childCount; b++)
                {
                    if (enemies.transform.GetChild(b).name == "Enemy")
                    {
                        enemies.transform.GetChild(b).GetComponent<Platformer.Mechanics.EnemyController>().path = null;
                    }
                }
                otherenemyDownID = new List<int>();
                clientsToClient = new List<clientStructure>();
                player2=new List<Vector3>();
                for(int a = 0;a < 4; a++)
                {
                    player2.Add(new Vector3());
                }
                for (int b = 0; b < number; b++)
                {
                    otherenemyDownID.Add(-1);
                    otherCoinsDestroyed.Add(-1);
                    clientsToClient.Add(new clientStructure());
                    clientsToClient[b].clientPosition = new Vector3(0, 0, 0);
                }
                for (int b = 0; b < 5; b++)
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
    void setEnemyPosition()
    {

        //Vector3 Postocalc = new Vector3();
        //if (player2Max == 0 && player3Max == 0 && player4Max == 0)
        //{
        //    for (int b = 0; b < enemies.transform.childCount; b++)
        //    {
        //        if (enemies.transform.GetChild(b).name == "Enemy" && enemies.transform.GetChild(b).GetComponent<Platformer.Mechanics.EnemyController>().path != null)
        //        {
        //            Postocalc.Set(enemies.transform.GetChild(b).transform.position.x - Player2.transform.position.x, enemies.transform.GetChild(b).transform.position.y - Player2.transform.position.y, 0);
        //            if (Postocalc.sqrMagnitude < 300)
        //            {
        //                if (player2Max < 4)
        //                {
        //                    player2[player2Max].Set(enemies.transform.GetChild(b).transform.position.x, enemies.transform.GetChild(b).transform.position.y, enemies.transform.GetChild(b).GetComponent<Platformer.Mechanics.EnemyController>().id);
        //                    player2Max++;
        //                }


        //            }
        //            Postocalc.Set(enemies.transform.GetChild(b).transform.position.x - Player3.transform.position.x, enemies.transform.GetChild(b).transform.position.y - Player3.transform.position.y, 0);
        //            if (Postocalc.sqrMagnitude < 300)
        //            {



        //                if (player3Max < 4)
        //                {
        //                    player3[player3Max].Set(enemies.transform.GetChild(b).transform.position.x, enemies.transform.GetChild(b).transform.position.y, enemies.transform.GetChild(b).GetComponent<Platformer.Mechanics.EnemyController>().id);
        //                    player3Max++;
        //                }
        //            }



        //            Postocalc.Set(enemies.transform.GetChild(b).transform.position.x - Player4.transform.position.x, enemies.transform.GetChild(b).transform.position.y - Player4.transform.position.y, 0);
        //            if (Postocalc.sqrMagnitude < 300)

        //            {
        //                if (player4Max < 4)
        //                {
        //                    player4[player4Max].Set(enemies.transform.GetChild(b).transform.position.x, enemies.transform.GetChild(b).transform.position.y, enemies.transform.GetChild(b).GetComponent<Platformer.Mechanics.EnemyController>().id);
        //                    player4Max++;
        //                }


        //            }

        //        }
        //    }


        //}

    }
}