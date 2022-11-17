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
    List<GameObject> rootObjects;
    // Start is called before the first frame update
    void Start()
    {
        newRotation = Quaternion.identity;
        newPosition = new Vector3();
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
        rootObjects = new List<GameObject>();
        coins = GameObject.Find("LEVEL/Tokens");
        pointsManager = player.GetComponent<PointsManager>();
    }

    public byte[] Serialize()
    {
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player_position.x);
        writer.Write(player_position.y);
        writer.Write(pointsManager.playerPoints);


        writer.Write(myCoinDestroyed);
        writer.Write(mycoinId);

        return stream.ToArray();
    }

    public void Deserialize(byte[] data)
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
        if (otherCoinDestroyed)
        {
            othercoinId = reader.ReadInt32();
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