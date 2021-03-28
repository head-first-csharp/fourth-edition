using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log($"Sleeping: {rigidBody.IsSleeping()}  Time: {Time.time:0.00}");
    }

    void OnMouseDown()
    {
        rigidBody.WakeUp();
    }
}
