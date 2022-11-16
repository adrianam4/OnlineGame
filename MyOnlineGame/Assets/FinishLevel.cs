using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject player1;

    private PointsManager pointsManager;
    private Animator playerAnimator;
    private Animator player1Animator;
    void Start()
    {
        pointsManager = player.GetComponent<PointsManager>();
        playerAnimator = player.GetComponent<Animator>();
        player1Animator = player1.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            }
            else if (pointsManager.playerPoints < pointsManager.player1Points)
            {
                //Online Player Wins
                player1Animator.SetTrigger("victory");
                playerAnimator.SetBool("dead", true);
            }
            else
            {
                //Both Player Wins
                playerAnimator.SetTrigger("victory");
                player1Animator.SetTrigger("victory");
            }
        }
    }
}
