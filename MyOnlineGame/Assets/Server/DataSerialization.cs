using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataSerialization : MonoBehaviour
{
    static MemoryStream stream;
    public GameObject player;
    GameObject UDPServer;
    GameObject UDPClient;
    Vector3 newPosition;
    Quaternion newRotation;
    public GameObject Player2;
    byte[] data;
    // Start is called before the first frame update
    void Start()
    {
        newRotation = Quaternion.identity;
        newPosition = new Vector3();
        data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
    }

    public byte[] Serialize()
    {
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player.transform.position.x);
        writer.Write(player.transform.position.y);
        Debug.Log("Player Position Serialized");

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
        double newPositionX = reader.ReadDouble();
        Debug.Log("position x: " + newPositionX);
        double newPositionY = reader.ReadDouble();
        Debug.Log("position y: " + newPositionY);

        newPosition.Set((float)newPositionX, (float)newPositionY, 0);
        Player2.transform.SetPositionAndRotation(newPosition, newRotation);
    }
}
