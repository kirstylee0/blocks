using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualTile : MonoBehaviour
{
    public BallDirection ballDir;
    public Transform midPoint;
    public float timer = 0;
    public GameObject[] blocks;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += 1 * Time.deltaTime;
        
        if (timer <= 4)
        {
            ballDir = BallDirection.Front;
            blocks[0].SetActive(true);
            blocks[1].SetActive(false);
        }
        if(timer >= 4)
        {
            ballDir = BallDirection.Back;
            blocks[1].SetActive(true);
            blocks[0].SetActive(false);
        }
        if(timer >= 8)
        {
            timer = 0; 
        }
    }
}
