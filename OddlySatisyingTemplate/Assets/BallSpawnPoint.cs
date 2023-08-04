using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawnPoint : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.spawnPoints.Add(this.transform);
    }
}
