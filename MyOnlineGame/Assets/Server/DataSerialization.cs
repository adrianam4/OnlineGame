using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataSerialization : MonoBehaviour
{
    MemoryStream stream;
    public GameObject player;
    GameObject UDPServer;
    Vector3 newPosition;
    Quaternion newRotation;
    public GameObject Player2;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        newRotation=Quaternion.identity;
        newPosition = new Vector3();
        byte[] data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
    }
    public void Serialize()
    {
        stream= new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player.transform.position.x);
        writer.Write(player.transform.position.y);
        UDPServer.GetComponent<UDP_Server>().sendData= stream.ToArray();
        UDPServer.GetComponent<UDP_Server>().PrepareToSend = true;
    }
    public void Deserialize(byte[] data)
    {
        stream.Write(data);
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        float newPositionX = reader.ReadSingle();
        float newPositionY = reader.ReadSingle();

        newPosition.Set(newPositionX, newPositionY, 0);
        Player2.transform.SetPositionAndRotation(newPosition, newRotation);

    }
    // Update is called once per frame
    void Update()
    {
        
        
    }
}
