using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MovementTile : MonoBehaviour
{
    public Transform TileVisual => _tileVisuals;
    [SerializeField] Transform _tileVisuals;
    [SerializeField] MMF_Player _destroyFeedback;
    [SerializeField] ParticleSystemRenderer _destroyPSRenderer;
    bool _isBeingDestroyed;
    public int ID;
    public Transform midPoint;
    public BallDirection ballDir;
    void Start()
    {
        
    }

    void Update()
    {
        if(ID == 0)
        {
            ballDir = BallDirection.Left;
        }

        if (ID == 90)
        {
            ballDir = BallDirection.Front;
        }

        if (ID == 180)
        {
            ballDir = BallDirection.Right;
        }

        if (ID == 270)
        {
            ballDir = BallDirection.Back;
        }
    }

    public void PlayDestroyFeedback()
    {
        if (!_isBeingDestroyed)
        {
            Renderer childRenderer = GetComponentInChildren<Renderer>();
             
            _destroyPSRenderer.material = childRenderer.material;
            _destroyFeedback.PlayFeedbacks();
            _isBeingDestroyed = true;
            Destroy(this);
        }
    }

    public void RotateVisual(int yRotation)
    {
        _tileVisuals.eulerAngles = new Vector3(0, yRotation, 0);
        ID = yRotation; 
    }
}
