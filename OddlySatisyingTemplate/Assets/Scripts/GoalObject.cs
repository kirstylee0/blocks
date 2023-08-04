using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalObject : MonoBehaviour
{

    public TMP_Text ballCountText;

    public int countToPass = 0;
    public int levelGoal = 5;
    void Start()
    {
        GameManager.Instance.AssignGoalObject(this);
    }

    // Update is called once per frame
    void Update()
    {
        

        if(countToPass >= levelGoal)
        {
            ballCountText.text = "5 / 5";
        }
        else
        {
            ballCountText.text = countToPass.ToString() + " / 5";
        }
    }

    public void IncreaseCurrentCount()
    {
        countToPass ++;
    }

    public bool GoalAcheived()
    {
        if (countToPass >= levelGoal)
        {
            return true;
        }
        else
            return false;
    }
}
