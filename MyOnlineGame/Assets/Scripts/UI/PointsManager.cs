using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int playerPoints = 0;
    public GameObject playerPointsUI;
    private TextMeshProUGUI textPointsUI;
    void Start()
    {
        textPointsUI = playerPointsUI.GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coins")
        {
            playerPoints++;
            textPointsUI.text = "x" + playerPoints;
        }
    }
}
