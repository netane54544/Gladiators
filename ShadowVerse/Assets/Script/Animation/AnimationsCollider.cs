using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsCollider : MonoBehaviour
{
    public Animator animator;

    private void Update()
    {
        BoxCollider[] components = gameObject.GetComponentsInChildren<BoxCollider>();

        if (animator.GetBool("isCrouching") == true)
        {
            if (components[0].enabled == false) //By checking if the first one we can know if it was changed already
            {
                foreach (BoxCollider boxCollider in components)
                {
                    boxCollider.enabled = true;
                }
            } 
        }
        else
        {
            if (components[0].enabled == true) //By checking if the first one we can know if it was changed already
            {
                foreach (BoxCollider boxCollider in components)
                {
                    boxCollider.enabled = false;
                }
            }
        }
    }
}
