using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamTown : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float cameraSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            cam.transform.position += Vector3.right * Time.deltaTime * cameraSpeed;
        else if(Input.GetKey(KeyCode.D))
            cam.transform.position += Vector3.left * Time.deltaTime * cameraSpeed;

        if (Input.GetKey(KeyCode.W))
            cam.transform.position += Vector3.down * Time.deltaTime * cameraSpeed;
        else if (Input.GetKey(KeyCode.S))
            cam.transform.position += Vector3.up * Time.deltaTime * cameraSpeed;
    }
}
