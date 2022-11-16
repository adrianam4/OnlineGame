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

    bool coinDestroyed = false;
    int coinId = -1;
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
    }

    public byte[] Serialize()
    {
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player_position.x);
        writer.Write(player_position.y);
        Debug.Log("Player Position Serialized" + player_position.x + "/// " + player_position.y);
        writer.Write(coinDestroyed);
        writer.Write(coinId);

        return stream.ToArray();
        //UDPServer.GetComponent<UDP_Server>().sendData = stream.ToArray();
        //UDPServer.GetComponent<UDP_Server>().PrepareToSend = true;
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
        coinDestroyed = reader.ReadBoolean();
        if (coinDestroyed)
        {
            coinId = reader.ReadInt16();
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
            if (coinDestroyed)
            {
                coinDestroyed = false;
                DestroyCoin();
            }
        }

        player_position.Set(player.transform.position.x, player.transform.position.y, 0);
    }
    public void setDestroiedCoin(int id)
    {
        coinDestroyed = true;
        coinId = id;
    }
    void DestroyCoin()
    {
        coins.transform.GetChild(coinId).gameObject.SetActive(false);

    }
}