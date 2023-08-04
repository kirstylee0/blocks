using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTile : MonoBehaviour
{
    Animator _anim;

    // Spawning at the end paramters
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] Transform _endOfAnimationSpawnPoint;
    //public Transform midPoint;
    //public BallDirection ballDir;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }
    void Update()
    {
        
    }

    public void StartTileAnimation(BallController ball)
    {
        if (ball != null)
        {

            _anim.SetBool("Triggered", true);
            Destroy(ball.gameObject);
            //Debug.Log("startanim");
        }
    }

    public void SpawnBallAtEnd()
    {
        //Debug.Log("endanim");
        _anim.SetBool("Triggered", false);
        Instantiate(_ballPrefab, _endOfAnimationSpawnPoint.position, Quaternion.identity);
        
    }

}
