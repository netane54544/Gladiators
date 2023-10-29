using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowConstructionUI : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset;

    private void Start()
    {
        //Stop the teleporting ui bug
        Vector3 pos = Camera.main.WorldToScreenPoint(lookAt.position + offset);

        transform.position = pos;
    }

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(lookAt.position + offset);

        transform.position = pos;
    }

}
