using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only for the editor for ortographics viewpoint using transportmation T(x)=Ax where A is a y axis rotation matrix
[ExecuteInEditMode]
public class CameraRotation : MonoBehaviour
{
    [SerializeField, Range(0, 360)]
    private uint angle = 45;
    [SerializeField]
    private int oldAngle = -1;

    //Rotation metrix for y axis
    private void Update()
    {
        if(angle != oldAngle)
        {
            Camera.main.transform.Rotate(GetRotation());
            oldAngle = (int)angle;
        }
    }

    internal Vector3 GetRotation()
    {
        Vector3 camPos = Camera.main.transform.position;

        float x = camPos.x * Mathf.Cos(angle) + camPos.z * Mathf.Sin(angle);
        float y = camPos.y;
        float z = camPos.x * -Mathf.Sin(angle) + camPos.z * Mathf.Cos(angle);

        return new Vector3(x, y, z);
    }

}
