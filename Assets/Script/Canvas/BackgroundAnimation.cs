using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private GameObject background_1;
    [SerializeField] private GameObject background_2;

    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float resetX = -3000f;
    [SerializeField] private float offsetX = 6000f; // reset sonrası taşınacağı mesafe

    void Update()
    { 
        MoveBackground(background_1);
        MoveBackground(background_2);
        
    }

    private void MoveBackground(GameObject bg)
    {
        bg.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        //Debug.Log(bg.transform.position.x);
        if (bg.transform.position.x <= resetX)
        {
            bg.transform.position += new Vector3(offsetX, 0f, 0f);
        }
    }
    
}
