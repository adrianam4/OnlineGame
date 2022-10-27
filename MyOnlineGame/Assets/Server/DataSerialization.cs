using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataSerialization : MonoBehaviour
{
    MemoryStream stream;
    public GameObject player;
    GameObject UDPServer;
    // Start is called before the first frame update
    void Start()
    {
        
        byte[] data = new byte[100];
        UDPServer = GameObject.Find("UDPServer");
    }
    void Serialize()
    {
        stream= new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(player.transform.position.x);
        writer.Write(player.transform.position.y);
        UDPServer.GetComponent<UDP_Server>().sendData= stream.ToArray();
        UDPServer.GetComponent<UDP_Server>().PrepareToSend = true;
    }
    void Deserialize()
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        reader.ReadDouble();

    }
    // Update is called once per frame
    void Update()
    {
        
        
    }
}
