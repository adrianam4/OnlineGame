using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            Destroy(this.gameObject);
        }
          
    }
}
