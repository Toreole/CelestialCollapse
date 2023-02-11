using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    protected Transform target;
    [SerializeField]
    protected float angle, distance;

    private void LateUpdate()
    {
        Vector3 offset = Quaternion.AngleAxis(angle, Vector3.right) * Vector3.up;
        transform.position = target.position + offset * distance;
    }
}
