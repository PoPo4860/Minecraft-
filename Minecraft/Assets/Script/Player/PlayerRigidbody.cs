using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidbody : MonoBehaviour
{
    readonly private static float playerWidth = 0.3f;
    readonly private static float playerHeight = 1.5f;
    private bool isGrounded = false;
    private float gravity = -9.8f;
    private float maxGravity = -9.8f;
    private Vector3 moveVelocity;
    [HideInInspector] public Vector3 velocity;

    public World world;

    private Vector3[] collisionVertex = new Vector3[4]
    {
        new Vector3(-playerWidth, 0, +playerWidth),
        new Vector3(-playerWidth, 0, -playerWidth),
        new Vector3(+playerWidth, 0, +playerWidth),
        new Vector3(+playerWidth, 0, -playerWidth)
    };

    private enum Direction { Forward, Back, Left, Right};
    private bool CheckCollision(in Vector3 vel, in Direction di)
    {                                  
        if(di == Direction.Left)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * playerHeight)));
        }
        else if (di == Direction.Right)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * playerHeight)));
        }
        else if (di == Direction.Forward)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * playerHeight)));
        }
        else if (di == Direction.Back)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * playerHeight)));
        }
        return false;
    }
    // Start is called before the first frame update

    private void FixedUpdate()
    {
        CalculateVelocity();
        transform.Translate(moveVelocity, Space.World);
    }
    public void Jump()
    {
        if (isGrounded == true)
        {
            gravity = -maxGravity;
        }
    }
    private void CalculateVelocity()
    {
        moveVelocity = CalculateMove();
        moveVelocity += CalculateGravity() * Vector3.up; // 중력 적용
    }
    private float CalculateGravity()
    {
        if (gravity > maxGravity)
            gravity -= Time.fixedDeltaTime * 35;
        else if (gravity < maxGravity)
            gravity = maxGravity;


        float yVelocity = Time.fixedDeltaTime * gravity;

        // 상단 충돌
        if (gravity > 0)
        {
            if(CheckCollision(new Vector3(0, yVelocity + 0.3f, 0), Direction.Left) ||
               CheckCollision(new Vector3(0, yVelocity + 0.3f, 0), Direction.Right))
            {
                gravity = maxGravity;
            }
        }

        // 하단 충돌
        isGrounded = CheckCollision(new Vector3(0, yVelocity, 0), Direction.Left) ||
                     CheckCollision(new Vector3(0, yVelocity, 0), Direction.Right);
        return isGrounded ? 0 : yVelocity;
    }
    private Vector3 CalculateMove()
    {
        float yVelocity = Time.fixedDeltaTime * gravity * 1.2f;
        if(CheckCollision(new Vector3(0, yVelocity, 0), Direction.Left) ||
           CheckCollision(new Vector3(0, yVelocity, 0), Direction.Right))
            yVelocity = 0;

        if (velocity.x < 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Left))
            velocity.x = 0;
        else if (velocity.x > 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Right))
            velocity.x = 0;

        if (velocity.z < 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Back))
            velocity.z = 0;
        else if (velocity.z > 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Forward))
            velocity.z = 0;

        return velocity;
    }
}
