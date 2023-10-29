using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class TouchManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float directionThreashold = 0.8f;
    [SerializeField]
    private float camSpeed = 1f;
    [SerializeField, Range(1, 20)]
    private float zoomBy = 2f;
    [SerializeField]
    private float zoomSpeed = 5f;
    [SerializeField]
    private float maxZoomIncrease = 0.1f;
    public bool canMove = true;

    private Vector3 startPosition = Vector3.zero;
    private float targetZoom;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && canMove)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began || touch.phase == UnityEngine.TouchPhase.Ended)
                startPosition = touch.position;

            
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

            if (Input.touchCount >= 2)
            {
                Touch secoundFinger = Input.GetTouch(1);

                if (touch.phase == UnityEngine.TouchPhase.Moved && secoundFinger.phase == UnityEngine.TouchPhase.Moved)
                {
                    //Debug.Log("Distance: " + Vector2.Distance(touch.position, secoundFinger.position));

                    if (Vector2.Distance(touch.position, secoundFinger.position) <= maxZoomIncrease)
                    {
                        targetZoom += 0.01f * zoomBy;
                        targetZoom = Mathf.Clamp(targetZoom, 0.6f, 8);
                    }
                    else
                    {
                        targetZoom -= 0.01f * zoomBy;
                        targetZoom = Mathf.Clamp(targetZoom, 0.6f, 8);
                    }
                }
            }
            else
            {
                if (touch.phase == UnityEngine.TouchPhase.Moved)
                {
                    //Swipping
                    //Debug.Log("Swipe Started");

                    Vector3 endPosition = touch.position;
                    Vector3 tD = endPosition - startPosition;
                    Vector2 direction = new Vector2(tD.x, tD.y).normalized;
                    Debug.Log("direction: " + direction.ToString());

                    if (Vector2.Dot(Vector2.up, direction) > directionThreashold)
                    {
                        //Debug.Log("Swipe Up");
                        Camera.main.transform.position += camSpeed * Time.deltaTime * Vector3.up;
                    }
                    else if (Vector2.Dot(Vector2.down, direction) > directionThreashold)
                    {
                        //Debug.Log("Swipe Down");
                        Camera.main.transform.position += camSpeed * Time.deltaTime * Vector3.down;
                    }

                    if (Vector2.Dot(Vector2.left, direction) > directionThreashold)
                    {
                        //Debug.Log("Swipe Left");
                        Camera.main.transform.position += camSpeed * Time.deltaTime * Vector3.left;
                    }
                    else if (Vector2.Dot(Vector2.right, direction) > directionThreashold)
                    {
                        //Debug.Log("Swipe Right");
                        Camera.main.transform.position += camSpeed * Time.deltaTime * Vector3.right;
                    }
                }
            }
        }
    }

}
