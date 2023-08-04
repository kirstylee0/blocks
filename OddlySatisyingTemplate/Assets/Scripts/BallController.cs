using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   public enum BallDirection
    {
        Front,
        Back,
        Right,
        Left,
        Up,
        Down,
        Still,
        End,
        Jump,
        Die
    }
public class BallController : MonoBehaviour
{
    public float speed;
 
    public BallDirection currentBallDirection;

    public bool hasColided;
    public Transform newCenter;
    public BallDirection newBallDir;
    GameManager gameManager; 

    

    void Start()
    {
        //currentBallDirection = ballDirection.Front;
        gameManager = FindObjectOfType<GameManager>();
        
    }


    void FixedUpdate()
    {
        switch (currentBallDirection)
        {
            case BallDirection.Front:
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                break;

            case BallDirection.Back:
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                break;

            case BallDirection.Right:
                transform.Translate(Vector3.back * speed * Time.deltaTime);
                break;

            case BallDirection.Left:
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;

            case BallDirection.Up:
                transform.Translate(Vector3.up * speed * 4 * Time.deltaTime);
                break;

            case BallDirection.Down:
                transform.Translate(Vector3.down * speed * 4 * Time.deltaTime);
                break;

            case BallDirection.Still:

                break;

            case BallDirection.End:
                Destroy(this.gameObject);
                break;

            case BallDirection.Jump:

                break;

            case BallDirection.Die:
                Destroy(this.gameObject);
                break;
        }
    }


    void Update() { 
        if (hasColided)
        {
            if (newCenter != null)
            {
                float distanceToMid = Vector3.Distance(newCenter.position, transform.position);
                //print(distanceToMid);

                if (distanceToMid <= 0.42)
                {
                    currentBallDirection = newBallDir;
                    hasColided = false;
                }
            }
        }

    }
   

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Ball")
        {
            Destroy(this.gameObject);
        }

        AnimationTile animationTile = collision.gameObject.GetComponentInParent<AnimationTile>();
        if (animationTile != null)
        {
            animationTile.StartTileAnimation(this);
        }


        MovementTile movementTileScript = collision.gameObject.GetComponentInParent<MovementTile>();
        if (movementTileScript != null)
        {

            newCenter = movementTileScript.midPoint;
            newBallDir = movementTileScript.ballDir;
            hasColided = true;
        }

        NonMoveObject nonMoveObjectScript = collision.gameObject.GetComponentInParent<NonMoveObject>();
        if(nonMoveObjectScript != null)
        {
            newCenter = nonMoveObjectScript.midPoint;
            newBallDir = nonMoveObjectScript.ballDir;
            hasColided = true;

        }

        DualTile dualTileScript = collision.gameObject.GetComponentInParent<DualTile>();
        if (dualTileScript != null)
        {
            newCenter = dualTileScript.midPoint;
            newBallDir = dualTileScript.ballDir;
            hasColided = true;

        }

        GoalObject goalObject = collision.gameObject.GetComponentInParent<GoalObject>();
        if(goalObject != null)
        {
            goalObject.IncreaseCurrentCount();
        }


    }

}
