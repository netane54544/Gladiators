using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

enum AnimationState
{
    Defualt,
    Crouching,
    Right,
    Left,
    Jumping,
    NotJumping,
    LeftOff,
    RightOff,
    LeftAndRightOff,
    CrouchingOff
}

public class MoveFromScroll : NetworkBehaviour
{
    private Rigidbody playerRgd;
    private Animator animator;
    private double floorHeight = -0.0008643903;
    private bool onGround = true;
    private const float JUMP_SPEED = 250f;
    private const int OFFSET = 15;
    public float movementSpeed = 1f;
    private GameObject scrollContainer;
    private GameObject scroller;
    public bool mirrorSide = false;

    private NetworkVariable<AnimationState> states = new();

    void Awake()
    {
        playerRgd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        floorHeight = 0.04;
        scrollContainer = GameObject.Find("ContainerCircle");
        scroller = GameObject.Find("MovementCircle");

        if (mirrorSide)
        {
            animator.SetBool("Mirror", true);
        }
    }

    public override void OnNetworkSpawn()
    {
        /*
        if (!IsOwner) //Makes sure only the owner can control the charcter
        {
            Destroy(this);
            //Destroy(transform.Find("Main Camera").gameObject);
        } 
        */
    }

    private void Update()
    {
        AppliedAnimation();
    }

    void FixedUpdate()
    {
        //Slider Movement
        Vector3 positionToMove = scroller.GetComponent<RectTransform>().position;
        Vector3 scrollUiPosition = scrollContainer.GetComponent<RectTransform>().position;

        if (scrollUiPosition != Vector3.zero && IsOwner && IsClient)
        {
            if (positionToMove.x > scrollUiPosition.x + OFFSET)
            {
                UpdateAnimationServerRpc(AnimationState.Right);
                animator.SetBool("Left", false);
                animator.SetBool("Right", true);
                transform.position += Vector3.forward * movementSpeed;
            }
            else if (positionToMove.x < scrollUiPosition.x - OFFSET)
            {
                UpdateAnimationServerRpc(AnimationState.Left);
                animator.SetBool("Left", true);
                animator.SetBool("Right", false);
                transform.position += Vector3.back * movementSpeed;
            }
            else if (!(animator.GetBool("Left") == false && animator.GetBool("Right") == false))
            {
                UpdateAnimationServerRpc(AnimationState.LeftAndRightOff);
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);
            }

            //Turn off the jumping animation before landing

            if (transform.position.y <= floorHeight && !onGround)
            {
                UpdateAnimationServerRpc(AnimationState.NotJumping);
                onGround = true;
            }

            if (positionToMove.y > scrollUiPosition.y + OFFSET && onGround)
            {
                if(animator.GetBool("isCrouching") == true)
                    animator.SetBool("isCrouching", false);

                UpdateAnimationServerRpc(AnimationState.Jumping);

                playerRgd.AddForce(Vector3.up * JUMP_SPEED, ForceMode.Impulse);
                onGround = false;
            }

            if (positionToMove.y < scrollUiPosition.y - OFFSET && onGround)
            {
                if(animator.GetBool("isCrouching") == false)
                    UpdateAnimationServerRpc(AnimationState.Crouching);

                animator.SetBool("isCrouching", true);
            }
            else if(animator.GetBool("isCrouching") == true)
            {
                if(animator.GetBool("isCrouching") == true)
                    UpdateAnimationServerRpc(AnimationState.CrouchingOff);

                animator.SetBool("isCrouching", false);
            }
        }
    }

    private void AppliedAnimation()
    {
        switch (states.Value)
        {
            case AnimationState.Crouching:
                animator.SetBool("isCrouching", true);
                break;
            case AnimationState.CrouchingOff:
                animator.SetBool("isCrouching", false);
                break;
            case AnimationState.Jumping:
                animator.SetBool("Jumping", true);
                animator.Play("Jumping Up", 0, 0.0f);
                break;
            case AnimationState.Left:
                animator.SetBool("Left", true);
                break;
            case AnimationState.Right:
                animator.SetBool("Right", true);
                break;
            case AnimationState.LeftOff:
                animator.SetBool("Left", false);
                break;
            case AnimationState.RightOff:
                animator.SetBool("Right", false);
                break;
            case AnimationState.LeftAndRightOff:
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);
                break;
            case AnimationState.NotJumping:
                animator.SetBool("Jumping", false);
                animator.Play("Idle", 0, 0.0f);
                break;
        }
    }

    [ServerRpc]
    private void UpdateAnimationServerRpc(AnimationState state)
    {
        states.Value = state;
    }
}
