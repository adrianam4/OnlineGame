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
    public GameObject udpServer;
    public GameObject udpClient;
    public GameObject tcpServer;
    public GameObject tcpClient;

    void Start()
    {
        isTCP = true;
        Button _button = thisButton.GetComponent<Button>();
        _button.onClick.AddListener(ChangeType);
    }

    public void ChangeType()
    {
        if (isTCP)
        {
            udpClient.SetActive(true);
            udpServer.SetActive(true);
            tcpClient.SetActive(false);
            tcpServer.SetActive(false);
            SetText("Change to TCP");
        }
        else if (!isTCP)
        {
            tcpClient.SetActive(true);
            tcpServer.SetActive(true);
            udpClient.SetActive(false);
            udpServer.SetActive(false);
            SetText("Change to UDP");
        }
        isTCP = !isTCP;
    }

    void SetText(string _text)
    {
        thisButton.GetComponentInChildren<TextMeshProUGUI>().text = _text;
    }
}
