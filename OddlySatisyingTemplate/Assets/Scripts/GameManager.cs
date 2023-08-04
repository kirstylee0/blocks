using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Framework;

public class GameManager : SingletonBehaviour<GameManager>
{
    public GameObject prefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public float timer;
    public int blockCount;
    public TMP_Text blockCountText;
    
    public string nextScene;

    List<GoalObject> goalObjects = new List<GoalObject>(); 
    void Start()
    {
        
    }

    
    void Update()
    {
        blockCountText.text = "blocks: " + blockCount.ToString();

        timer += 1 * Time.deltaTime;

        if(timer >= 2)
        {
            //Instantiate(prefab, spawnPoint[].position, spawnPoint[].rotation);
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            }
            timer = 0;
        }

        if(AllGoalsAchieved())
        {
            //next level
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);

        }
    }

    public void AssignGoalObject (GoalObject goalObject)
    {
        goalObjects.Add(goalObject);
    }

    bool AllGoalsAchieved()
    {
        for (int i = 0; i < goalObjects.Count; i++)
        {
            if (!goalObjects[i].GoalAcheived())
                return false;
        }

        return true;
    }
}
