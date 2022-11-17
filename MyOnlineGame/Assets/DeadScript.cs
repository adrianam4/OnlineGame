using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics;

public class DeadScript : MonoBehaviour
{
    public GameObject player;
    private Health playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
            playerHealth.currentHP--;
    }
}
