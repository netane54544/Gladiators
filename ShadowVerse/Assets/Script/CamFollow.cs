using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFallow : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float smoothSpeed = 0.125f;
    [SerializeField]
    private Vector3 offset;

    private void Awake()
    {
        cam = Camera.main;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = transform.position + offset;
        Vector3 veclocity = Vector3.zero;
        Vector3 smoothPosition = Vector3.SmoothDamp(cam.transform.position, desiredPosition, ref veclocity, smoothSpeed);
        cam.transform.position = new Vector3(smoothPosition.x, cam.transform.position.y, smoothPosition.z);
    }

}
