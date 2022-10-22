using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Change_Type : MonoBehaviour
{
    public bool isTCP;
    public GameObject thisButton;
    public GameObject UDPServer;
    public GameObject UDPClient;
    public GameObject TCPServer;
    public GameObject TCPClient;

    void Start()
    {
        isTCP = true;
        Button button = thisButton.GetComponent<Button>();
        button.onClick.AddListener(ChangeType);
    }

    public void ChangeType()
    {
        if (isTCP)
        {
            UDPClient.SetActive(true);
            UDPServer.SetActive(true);
            TCPClient.SetActive(false);
            TCPServer.SetActive(false);
            SetText("Change to TCP");
        }
        else if (!isTCP)
        {
            TCPClient.SetActive(true);
            TCPServer.SetActive(true);
            UDPClient.SetActive(false);
            UDPServer.SetActive(false);
            SetText("Change to UDP");
        }
        isTCP = !isTCP;
    }

    void SetText(string text)
    {
        thisButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
