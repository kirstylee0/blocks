using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class ChangeIndicator : SingletonBehaviour<ChangeIndicator>
{
    

    public List<GameObject> indicatorsList = new List<GameObject>();
    

    void Start()
    {
        CycleIndicator(0);
    }

    void Update()
    {

    }

   
    public void CycleIndicator(int id)
    {
        foreach (GameObject indicatorOption in indicatorsList)
        {
            indicatorOption.SetActive(false);
        }

        indicatorsList[id].SetActive(true);

    }

}
