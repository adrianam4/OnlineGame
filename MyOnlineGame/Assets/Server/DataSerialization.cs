using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataSerialization : MonoBehaviour
{
    GameObject coins;
    static MemoryStream stream;
    public GameObject player;
    GameObject UDPServer;
    GameObject UDPClient;
    Vector3 newPosition;
    Quaternion newRotation;
    public GameObject Player2;
    byte[] data;
    Vector3 player_position;
    private bool deserialized = false;
    private PointsManager pointsManager;

    bool otherCoinDestroyed = false;
    int othercoinId = -1;
    bool myCoinDestroyed = false;
    int mycoinId = -1;
    public List<Vector3> rootObjects;
    Vector3 auxiliar;
    public int type = -1;
    Vector3 enemyAuxiliar;

    public bool enemyDown = false;
    public int enemyDownId = -1;
    // Start is called before the first frame update
    void Start()
    {
        newRotation = Quaternion.identity;
        newPosition = new Vector3();
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
        rootObjects = new List<Vector3>();
        coins = GameObject.Find("LEVEL/Tokens");
        pointsManager = player.GetComponent<PointsManager>();
        auxiliar=new Vector3();
        enemyAuxiliar = new Vector3();
    }

    public byte[] Serialize(int id)
    {
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player_position.x);
        writer.Write(player_position.y);
        writer.Write(pointsManager.playerPoints);


        writer.Write(myCoinDestroyed);
        writer.Write(mycoinId);

        if (id == 0)//server
        {
            writer.Write(rootObjects.Count);
            for(int i = 0; i < rootObjects.Count; i++)
            {

                writer.Write(rootObjects[i].x);
                writer.Write(rootObjects[i].y);
                writer.Write(rootObjects[i].z);
            }
            rootObjects.Clear();
        }
        else//client
        {
            
                writer.Write(enemyDown);
            
                writer.Write(enemyDownId);
            
            
        }
        return stream.ToArray();
    }

    public void Deserialize(byte[] data,int id)
    {
        stream = new MemoryStream();
        stream.Write(data, 0, data.Length);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        float newPositionX = reader.ReadSingle();
        Debug.Log("position x: " + newPositionX);
        float newPositionY = reader.ReadSingle();
        Debug.Log("position y: " + newPositionY);
        pointsManager.player1Points = reader.ReadInt32();
        otherCoinDestroyed = reader.ReadBoolean();

        othercoinId = reader.ReadInt32();
        
        if (id == 1)//client
        {
            int numOfEneemies= reader.ReadInt32();
            for (int i = 0;i < numOfEneemies; i++)
            {
                float ID=reader.ReadSingle();
                float x=reader.ReadSingle();
                float y =reader.ReadSingle();
                auxiliar.Set(ID, x, y);
                rootObjects.Add(auxiliar);
            }
        }
        else//server
        {
            enemyDown = reader.ReadBoolean();

            enemyDownId = reader.ReadInt32();
            
        }
        newPosition.Set(newPositionX, newPositionY, 0);
        deserialized = true;
    }
    private void Update()
    {
        if (deserialized)
        {
            Player2.transform.SetPositionAndRotation(newPosition, newRotation);
            deserialized = false;

            if(type == 1)
            {
                GameObject enemies = GameObject.Find("LEVEL/Enemies");

                int counter = 0;
                for (int a = 0; a < enemies.transform.childCount; a++)
                {
                    if (enemies.transform.GetChild(a).name == "Enemy")
                    {
                        if(rootObjects[counter].x== enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().id)
                        {
                            enemyAuxiliar.Set(rootObjects[counter].y, rootObjects[counter].z, 1);
                            enemies.transform.GetChild(a).transform.SetPositionAndRotation(enemyAuxiliar, newRotation);
                        }
                        counter++;
                    }

                }
                rootObjects.Clear();
            }
            else
            {
                GameObject enemies = GameObject.Find("LEVEL/Enemies");

                for (int a = 0; a < enemies.transform.childCount; a++)
                {
                    if (enemies.transform.GetChild(a).name == "Enemy")
                    {
                        if (enemyDownId == enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().id)
                        {
                            //enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().MakeDead();
                            //enemies.transform.GetChild(a).GetComponent<Platformer.Mechanics.EnemyController>().path = null;
                            enemies.transform.GetChild(a).gameObject.SetActive(false);
                            

                        }
                    }
                    
                }
            }
            
        }
        if (otherCoinDestroyed)
        {

            DestroyCoin();
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