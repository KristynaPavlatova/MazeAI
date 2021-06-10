using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySidesAndDown : MonoBehaviour
{
    public const int RAY_LENGTH = 3;
    public float minDistanceFromWall = 1.6f;
    private RaycastHit hitInfo;
    public bool CanGoLeft()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left) * RAY_LENGTH, out hitInfo))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * RAY_LENGTH, Color.yellow);
            if (hitInfo.distance <= minDistanceFromWall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }        
    }
    public bool CanGoRight()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right) * RAY_LENGTH, out hitInfo))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * RAY_LENGTH, Color.yellow);
            if (hitInfo.distance <= minDistanceFromWall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    public bool CanGoForward()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * RAY_LENGTH, out hitInfo))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * RAY_LENGTH, Color.yellow);
            if (hitInfo.distance <= minDistanceFromWall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    public bool CanGoBack()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back) * RAY_LENGTH, out hitInfo))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * RAY_LENGTH, Color.yellow);
            if (hitInfo.distance <= minDistanceFromWall)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool StandingOnGoal()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down) * RAY_LENGTH, out hitInfo))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * RAY_LENGTH, Color.yellow);
            if (hitInfo.collider.gameObject.tag == "goal")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
