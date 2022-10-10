using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class buttonsInput : MonoBehaviour
{
    public Button serverbutton;
    public Button clientbutton;
    public Button sendbutton;
    public GameObject inputBox;
    public GameObject TCPserver;
    public GameObject TCPclient;
    string inputText;
    // Start is called before the first frame update
    void Start()
    {
        Button SB = serverbutton.GetComponent<Button>();
        Button CS = clientbutton.GetComponent<Button>();
        Button SendButton = sendbutton.GetComponent<Button>();
        SB.onClick.AddListener(ServerTaskOnClick);
        CS.onClick.AddListener(ClientTaskOnClick);
        SendButton.onClick.AddListener(SendTaskOnClick);


        var input = inputBox.GetComponent<InputField>();
        var se = new InputField.EndEditEvent();
        se.AddListener(updatetext);
        input.onEndEdit = se;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void  updatetext(string arg0)
    {
        inputText = arg0;
    }
    void ServerTaskOnClick()
    {
        TCPserver.GetComponent<TCPServer>().ToCreateServer = true;
    }
    void ClientTaskOnClick()
    {
        TCPclient.GetComponent<TCPClient>().ToCreateclient = true;
    }
    void SendTaskOnClick()
    {
        TCPserver.GetComponent<TCPServer>().text= inputText;
        TCPserver.GetComponent<TCPServer>().PrepareToSend = true;
    }
}
