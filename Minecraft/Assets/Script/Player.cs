using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region .
    [SerializeField] private World world;

    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float maxGravity = -9.8f;

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

    readonly private float  playerWidth = 0.3f;
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
        SetCursor();

    }
    void Update()
    {
        deltaTime = Time.deltaTime;
        GetPlayerInputs();

    }
    void SetCursor()
    {
        // defualt값 True 와 None
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState != CursorLockMode.None ? CursorLockMode.None : CursorLockMode.Locked;
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
        mouseX = Input.GetAxis("Mouse X") * 10;
        mouseY = Input.GetAxis("Mouse Y") * 10;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            gravity = -maxGravity;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursor();
        }
    }

    private void CalculateVelocity()
    {
        Vector3 playerPos = PosNormaliz(transform.position);

        velocity = CalculateMove(playerPos);
        velocity += CalculateGravity(playerPos) * Vector3.up; // 중력 적용
    }

    private float CalculateGravity(Vector3 pos)
    {
        if(gravity > maxGravity)
        {
            gravity -= Time.fixedDeltaTime * 35;
        }
        else if (gravity < maxGravity)
        {
            gravity = maxGravity;
    }
        float yVelocity = Time.fixedDeltaTime* gravity;
        // 0.2 0.8
        isGrounded =
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z + playerWidth));
        return isGrounded ? 0 : yVelocity;
    }

    private Vector3 CalculateMove(in Vector3 pos)
    {
        Vector3 moveVelocity = Time.fixedDeltaTime * walkSpeed * ((transform.forward * vertical) + (transform.right * horizontal));
        Vector3[] collisionVertex = new Vector3[8]
        {
            pos + new Vector3(moveVelocity.x - playerWidth, moveVelocity.y + 0.2f, -playerWidth),
            pos + new Vector3(moveVelocity.x - playerWidth, moveVelocity.y + 0.2f, +playerWidth),
                                                                                   
            pos + new Vector3(moveVelocity.x + playerWidth, moveVelocity.y + 0.2f, -playerWidth),
            pos + new Vector3(moveVelocity.x + playerWidth, moveVelocity.y + 0.2f, +playerWidth),

            pos + new Vector3(-playerWidth, moveVelocity.y + 0.2f, moveVelocity.z - playerWidth),
            pos + new Vector3(+playerWidth, moveVelocity.y + 0.2f, moveVelocity.z - playerWidth),
                              
            pos + new Vector3(-playerWidth, moveVelocity.y + 0.2f, moveVelocity.z + playerWidth),
            pos + new Vector3(+playerWidth, moveVelocity.y + 0.2f, moveVelocity.z + playerWidth)

        };
        if (moveVelocity.x < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[0]) ||
                world.CheckBlockSolid(collisionVertex[1])) 
            {
                moveVelocity.x = 0;
            }
        }
        else if (moveVelocity.x > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[2]) ||
                world.CheckBlockSolid(collisionVertex[3])) 
            {
                moveVelocity.x = 0;
            }
        }
        
        if (moveVelocity.z < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[4]) ||
                world.CheckBlockSolid(collisionVertex[5]))
            {
                moveVelocity.z = 0;
            }
        }
        else if (moveVelocity.z > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[6]) ||
                world.CheckBlockSolid(collisionVertex[7]))
            {
                moveVelocity.z = 0;
            }
        }
        return moveVelocity;
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
        Vector3 cameroRotate = Vector3.right * -mouseY;
        cameraTransform.Rotate(cameroRotate);
        if (cameraTransform.eulerAngles.z >= 170.0f)
        {
            Vector3 asd = cameraTransform.eulerAngles;
            if (cameraTransform.eulerAngles.x <= 90)
            {
                asd.x = 90;
            }
            else
            {
                asd.x = 270;
            }
            cameraTransform.eulerAngles = asd;
        }
        transform.Translate(velocity, Space.World);
    }

}
