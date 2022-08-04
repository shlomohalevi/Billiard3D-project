using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UnitBall : MonoBehaviour
{
    public static readonly float radius = 0.111f;
    public BallType ballType;
    public ballTextureType ballTextureType;
    public Sprite ballImage;
    Rigidbody ballRb;
    Collider ballCollider;


    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<Collider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("CueBall"))
        {
            GameplayStatesHandler.instance.FirstBallThatTochedTheCueBall(this);
        }
        //check if the given layer collision included in the layer mask
       if (collision.gameObject.layer == LayerMask.NameToLayer("Ball")|| collision.gameObject.layer == LayerMask.NameToLayer("WallColliders"))
        {
            float currentMagnitudeVelocity = ballRb.velocity.magnitude;
            GameplayStatesHandler.instance.PlayCollisionHitSound(currentMagnitudeVelocity, transform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhysicalHole"))
        {
            ballRb.velocity = ballRb.velocity * 0.05f;
            ballCollider.isTrigger = true;
        }
       else if (other.CompareTag("ButtomTable"))
        {

            GameplayStatesHandler.instance.AddBallToBallGoesToPocketList(this);
            GameplayStatesHandler.instance.RemoveBallFromActivateBallsINGameList(this);
            GameplayStatesHandler.instance.AssignBallTypeToPlayerAndAi(this);
            UpdateBallsImages.instance.AssignImageOfBall(this);
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.useGravity = false;
            ballRb.gameObject.SetActive(false);
        }

    }
}
    public enum BallType
    {
        non,
        humanPlayerBall,
        computerPlayerBall,
        blackBall,
        cueBall,
    }
    public enum ballTextureType
    {
        non,
        stripedColor,
        fullColor,
    }

  

   


    
