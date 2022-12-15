using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class randomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().color = new Color(r: Random.Range(0f, 1f), g: Random.Range(0f, 1f), b: Random.Range(0f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
