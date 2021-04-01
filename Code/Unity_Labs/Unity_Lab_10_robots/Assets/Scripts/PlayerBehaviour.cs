using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : BaseBehaviour
{
    private Vector3 lastUpdatePosition;

    private float waiting = 0f;

    public bool IsMoving =>
            (waiting > 0f) ||
            (Vector3.Distance(transform.position, lastUpdatePosition) > 0.01f);

    public void StopMoving() => agent.SetDestination(transform.position);

    void Update()
    {
        if (gameController.GameOver) StopMoving();
        if (waiting >= 0f) waiting -= Time.deltaTime;
        if (!gameController.GameOver && !IsMoving)
        {
            Vector3 target = transform.position;

            if (Input.GetKey(KeyCode.T))
            {
                Start();
                target = transform.position;
            }
            else if (Input.GetKey(KeyCode.Q))
                target += Vector3.forward + Vector3.left;
            else if (Input.GetKey(KeyCode.W))
                target += Vector3.forward;
            else if (Input.GetKey(KeyCode.E))
                target += Vector3.forward + Vector3.right;
            else if (Input.GetKey(KeyCode.D))
                target += Vector3.right;
            else if (Input.GetKey(KeyCode.C))
                target += Vector3.back + Vector3.right;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.X))
                target += Vector3.back;
            else if (Input.GetKey(KeyCode.Z))
                target += Vector3.back + Vector3.left;
            else if (Input.GetKey(KeyCode.A))
                target += Vector3.left;
            else if (Input.GetKey(KeyCode.Space))
                waiting = .5f;

            agent.SetDestination(target);
        }
    }

    void Start()
    {
        transform.position = RandomPointHelper.RandomPointOnMesh(
                                          transform.position, 100) + Vector3.up;
    }

    private void FixedUpdate()
    {
        lastUpdatePosition = transform.position;
    }
}

