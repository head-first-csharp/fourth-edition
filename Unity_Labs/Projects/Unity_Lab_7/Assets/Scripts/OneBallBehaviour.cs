using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBallBehaviour : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Vector3 forceAdded;
    public float Multiplier = 100f;
    public float MaxMultiplier = 3000f;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Wait one second, then call the MoveMe method every 1.5seconds
        InvokeRepeating("MoveMe", 1f, 1.5f);
    }

    private void MoveMe()
    {
        /*
         * The first iteration of MoveMe
         * 
        // Remove the force MoveMe added the last time it was invoked
        rigidBody.velocity = Vector3.zero;

        // Add a force in a random direction
        forceAdded = Random.insideUnitSphere * 500f;
        rigidBody.AddForce(forceAdded);
        */

        /*
         * The second iteration of MoveMe
         * 
        if (forceAdded == Vector3.left * 500f)
        {
            forceAdded = Vector3.forward * 500f;
        }
        else if (forceAdded == Vector3.forward * 500f)
        {
            forceAdded = Vector3.right * 500f;
        }
        else if (forceAdded == Vector3.right * 500f)
        {
            forceAdded = Vector3.back * 500f;
        }
        else
        {
            forceAdded = Vector3.left * 500f;
        }

        rigidBody.velocity = Vector3.zero;
        rigidBody.AddForce(forceAdded);
        */

        forceAdded = new Vector3(Multiplier - Random.value * Multiplier * 2,
               0, Multiplier - Random.value * Multiplier * 2);
        rigidBody.AddForce(forceAdded);
        Multiplier += 100f;
        if (Multiplier > MaxMultiplier) Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, forceAdded, Color.white);
    }
}
