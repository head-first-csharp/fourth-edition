using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private bool clicking = false;
    private float multiplier = 100f;
    private Rigidbody rigidBody;
    private Camera mainCamera;
    private Vector3 startingPosition;
    private GameController gameController;

    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                    .GetComponent<GameController>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        startingPosition = transform.position;
    }

    private bool hitTarget;

    void Update()
    {
        if (!gameController.GameOver)
        {
            HitBallMechanic();
            OutOfBoundsCheck();
        }
    }

    private void OutOfBoundsCheck()
    {
        if (transform.position.y < -10f)
        {
            transform.position = startingPosition;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            if (!hitTarget)
                gameController.BallLost();
        }
    }

    private void HitBallMechanic()
    {
        if (!clicking && Input.GetMouseButtonDown(0))
        {
            hitTarget = false;
            clicking = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.tag == "Player")
                {
                    Rigidbody rigidBody = hitObject.GetComponent<Rigidbody>();
                    rigidBody.AddForce(ray.direction * hit.distance * multiplier);
                }
            }
        }
        else if (clicking && Input.GetMouseButtonUp(0)) clicking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
            hitTarget = true;
    }
}
