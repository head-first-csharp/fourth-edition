using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform Player;
    public float Angle = 3F;
    public float ZoomSpeed = 0.25F;

    void Update()
    {
        var scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelValue != 0)
        {
            transform.position *= (1F + scrollWheelValue * ZoomSpeed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(Player.position, Vector3.up, Angle);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(Player.position, Vector3.down, Angle);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.RotateAround(Player.position, Vector3.right, Angle);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.RotateAround(Player.position, Vector3.left, Angle);
        }

        transform.LookAt(Player.position);
    }
}
