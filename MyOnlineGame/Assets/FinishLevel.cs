using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject player1;
    public GameObject resultText;
    //public GameObject fadeObject;

    private PointsManager pointsManager;
    private Animator playerAnimator;
    private Animator player1Animator;
    private bool finishPlay = false;
    private float finishCounter = 0;
    void Start()
    {
        pointsManager = player.GetComponent<PointsManager>();
        playerAnimator = player.GetComponent<Animator>();
        player1Animator = player1.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (finishPlay)
        {
            finishCounter += Time.deltaTime;
            if (finishCounter >= 10 || Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Mario");
                //StartCoroutine(Fading());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //Compare Coins and Set Winner
            if (pointsManager.playerPoints > pointsManager.player1Points)
            {
                //Local Player Wins
                playerAnimator.SetTrigger("victory");
                player1Animator.SetBool("dead",true);
                resultText.GetComponent<TextMeshProUGUI>().text = "PLAYER 1 WINS!" + "\n" + "PRESS 'SPACE' TO RESTART";
            }
            else if (pointsManager.playerPoints < pointsManager.player1Points)
            {
                //Online Player Wins
                player1Animator.SetTrigger("victory");
                playerAnimator.SetBool("dead", true);
                resultText.GetComponent<TextMeshProUGUI>().text = "PLAYER 2 WINS!" + "\n" + "PRESS 'SPACE' TO RESTART";
            }
            else
            {
                //Both Player Wins
                playerAnimator.SetTrigger("victory");
                player1Animator.SetTrigger("victory");
                resultText.GetComponent<TextMeshProUGUI>().text = "TIE!" + "\n" + "PRESS 'SPACE' TO RESTART";
            }
            finishPlay = true;
        }
    }

    //IEnumerator Fading()
    //{
    //    fadeObject.GetComponent<Animator>().SetBool("Fade", true);
    //    yield return new WaitUntil(() => fadeObject.GetComponent<Image>().color.a == 1);
    //    SceneManager.LoadScene("Mario");
    //}
}
