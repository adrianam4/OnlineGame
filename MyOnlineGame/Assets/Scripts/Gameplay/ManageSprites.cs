using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageSprites : MonoBehaviour
{
    // Start is called before the first frame update
    public TCPServer server;

    public GameObject player1;
    public GameObject player1UI;

    public GameObject player2;
    public GameObject player2UI;

    public GameObject player3;
    public GameObject player3UI;

    // Update is called once per frame
    void Update()
    {
        switch (server.clientsList.Count)
        {
            case 1:
                player1.SetActive(true);
                player1UI.SetActive(true);

                player2.SetActive(false);
                player2UI.SetActive(false);

                player3.SetActive(false);
                player3UI.SetActive(false);
                break;
            case 2:
                player1.SetActive(true);
                player1UI.SetActive(true);

                player2.SetActive(true);
                player2UI.SetActive(true);

                player3.SetActive(false);
                player3UI.SetActive(false);
                break;
            case 3:
                player1.SetActive(true);
                player1UI.SetActive(true);

                player2.SetActive(true);
                player2UI.SetActive(true);

                player3.SetActive(true);
                player3UI.SetActive(true);
                break;
        }
    }
}
