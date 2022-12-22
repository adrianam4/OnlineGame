using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageSprites : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject serverObject;

    public GameObject player1;
    public GameObject player1UI;

    public GameObject player2;
    public GameObject player2UI;

    public GameObject player3;
    public GameObject player3UI;

    private int clientCount;
    // Update is called once per frame
    void Update()
    {
        if (serverObject.GetComponent<DataSerialization>().ServerClient == 0)
            clientCount = serverObject.GetComponent<DataSerialization>().clientList.Count;
        else
            clientCount = serverObject.GetComponent<DataSerialization>().number;

        if (clientCount == 0)
        {
            player1.SetActive(false);
            player1UI.SetActive(false);

            player2.SetActive(false);
            player2UI.SetActive(false);

            player3.SetActive(false);
            player3UI.SetActive(false);
        }
        else if (clientCount == 1)
        {
            player1.SetActive(true);
            player1UI.SetActive(true);

            player2.SetActive(false);
            player2UI.SetActive(false);

            player3.SetActive(false);
            player3UI.SetActive(false);
        }
        else if (clientCount == 2)
        {
            player1.SetActive(true);
            player1UI.SetActive(true);

            player2.SetActive(true);
            player2UI.SetActive(true);

            player3.SetActive(false);
            player3UI.SetActive(false);
        }
        else
        {
            player1.SetActive(true);
            player1UI.SetActive(true);

            player2.SetActive(true);
            player2UI.SetActive(true);

            player3.SetActive(true);
            player3UI.SetActive(true);
        }
    }
}
