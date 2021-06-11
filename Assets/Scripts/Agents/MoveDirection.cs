using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{
    private int _moveDistance = 1;
    private Rigidbody _rBody;

    private void Awake()
    {
        _rBody = this.GetComponent<Rigidbody>();
                Debug.Assert(_rBody, "Agent does not have a character controller component!");
    }
    public void MoveInDirection(int pDir)
    {
        switch (pDir)
        {
            case (int)DirectionsEnum.directions.Left:
                _rBody.MovePosition(new Vector3 (this.transform.position.x + _moveDistance, this.transform.position.y, this.transform.position.z));
                break;
            case (int)DirectionsEnum.directions.Right:
                _rBody.MovePosition(new Vector3(this.transform.position.x - _moveDistance, this.transform.position.y, this.transform.position.z));
                break;
            case (int)DirectionsEnum.directions.Forward:
                _rBody.MovePosition(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - _moveDistance));
                break;
            case (int)DirectionsEnum.directions.Back:
                _rBody.MovePosition(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + _moveDistance));

                break;
        }
    }
}
