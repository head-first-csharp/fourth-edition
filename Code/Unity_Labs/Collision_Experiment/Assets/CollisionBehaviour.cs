using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{this.name} collided with {collision.gameObject.name} at time {Time.time}");
    }
}
