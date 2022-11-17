using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int playerPoints = 0;
    public int player1Points = 0;
    public GameObject playerPointsUI;
    public GameObject player1PointsUI;
    public GameObject coinUI;
    private TextMeshProUGUI textPointsUI;
    private TextMeshProUGUI textPoints1UI;
    void Start()
    {
        textPointsUI = playerPointsUI.GetComponent<TextMeshProUGUI>();
        textPoints1UI = player1PointsUI.GetComponent<TextMeshProUGUI>();
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
    }
}
