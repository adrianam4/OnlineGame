using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int playerPoints = 0;
    public int player1Points = 0;
    public int player2Points = 0;
    public int player3Points = 0;

    public GameObject playerPointsUI;
    public GameObject player1PointsUI;
    public GameObject player2PointsUI;
    public GameObject player3PointsUI;
    public GameObject coinUI;

    private TextMeshProUGUI textPointsUI;
    private TextMeshProUGUI textPoints1UI;
    private TextMeshProUGUI textPoints2UI;
    private TextMeshProUGUI textPoints3UI;
    void Start()
    {
        textPointsUI = playerPointsUI.GetComponent<TextMeshProUGUI>();
        textPoints1UI = player1PointsUI.GetComponent<TextMeshProUGUI>();
        textPoints2UI = player2PointsUI.GetComponent<TextMeshProUGUI>();
        textPoints3UI = player3PointsUI.GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coins")
        {
            playerPoints++;
            textPointsUI.text = "x" + playerPoints;
            Instantiate(coinUI, Vector3.zero, Quaternion.identity);
        }
    }
    private void Update()
    {
        textPoints1UI.text = "x" + player1Points;
        textPoints2UI.text = "x" + player2Points;
        textPoints3UI.text = "x" + player3Points;
    }
}
