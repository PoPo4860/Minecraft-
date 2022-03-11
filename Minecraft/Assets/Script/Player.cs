using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region .
    [SerializeField] private World world;

    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private float gravity = 9.8f;

    #endregion
    private Transform cameraTransform;

    #region .
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    private float deltaTime;

    private Vector3 velocity;

    #endregion

    private float playerWidth = 0.3f;
    //private float boundsTolerance = 0.3f;
    //private float vericalMomentum = 0f;

    private bool isGrounded = false;
    //private bool isJumping = false;
    //private bool isRunning = false;
    //private bool jumpRequested = false;


    void Start()
    {
        Camera camera = GetComponentInChildren<Camera>();
        cameraTransform = camera.transform;
    }

    void Update()
    {
        deltaTime = Time.deltaTime;
        GetPlayerInputs();

    }
    private void FixedUpdate()
    {
        CalculateVelocity();
        MoveAndRotate();
    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X") * 8;
        mouseY = Input.GetAxis("Mouse Y") * 8;
    }

    private void CalculateVelocity()
    {
        velocity = Time.fixedDeltaTime * walkSpeed * ((transform.forward * vertical) + (transform.right * horizontal));
        velocity += CalculateDownSpeedAndSetGroundState(Time.fixedDeltaTime * gravity) * Vector3.down; // 중력 적용
    }

    private float CalculateDownSpeedAndSetGroundState(float yVelocity)
    {
        Vector3 pos = PosNormaliz(transform.position);
        
        isGrounded =
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z + playerWidth));
        if(false == isGrounded)
        {
            Debug.Log(isGrounded);
        }
        return isGrounded ? 0 : yVelocity;
    }
    private Vector3 PosNormaliz(Vector3 pos)
    {
        bool xCheck = (pos.x < 0);
        bool zCheck = (pos.z < 0);
        pos.x %= VoxelData.ChunkWidth;
        pos.y %= VoxelData.ChunkHeight;
        pos.z %= VoxelData.ChunkWidth;
        if (xCheck) pos.x += (VoxelData.ChunkWidth);
        if (zCheck) pos.z += (VoxelData.ChunkWidth);
        return pos;
    }
    private void MoveAndRotate()
    {
        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.Rotate(Vector3.right * -mouseY);
        transform.Translate(velocity, Space.World);
    }

}
