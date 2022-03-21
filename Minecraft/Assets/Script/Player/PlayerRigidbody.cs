using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidbody : MonoBehaviour
{
    readonly private static float playerWidth = 0.3f;
    readonly private static float playerHeight = 1.5f;
    private bool isGrounded = false;
    public float gravity = -9.8f;
    private float downMAxGravity = -15.8f;
    private float jumpPower = 9.8f;
    private bool crouch = false;
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

    private enum Direction { Forward, Back, Left, Right, Down, Up};
    private bool CheckCollision(in Vector3 vel, in Direction dir)
    {                                  
        if(dir == Direction.Left)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (playerHeight / 2))) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (playerHeight / 2))));
        }
        else if (dir == Direction.Right)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * (playerHeight / 2))) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * (playerHeight / 2))));
        }
        else if (dir == Direction.Forward)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * (playerHeight / 2))) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * (playerHeight / 2))));
        }
        else if (dir == Direction.Back)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (playerHeight / 2))) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * (playerHeight / 2))));
        }
        else if (dir == Direction.Down)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3]));
        }
        else if (dir == Direction.Up)
        {
            return (world.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * playerHeight)) ||
                    world.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * playerHeight)));
        }
        return false;
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        transform.Translate(moveVelocity, Space.World);
    }
    public void InputJump()
    {
        if (isGrounded == true)
        {
            gravity = jumpPower;
        }
    }
    public void InputShift(in bool input)
    {
        crouch  = input;
    }

    private void CalculateVelocity()
    {
        moveVelocity = CalculateMove();
        moveVelocity += CalculateGravity() * Vector3.up;
    }
    private float CalculateGravity()
    {
        if (gravity > downMAxGravity)
            gravity -= Time.fixedDeltaTime * 35;
        else
            gravity = downMAxGravity;


        float yVelocity = Time.fixedDeltaTime * gravity;

        // 상단 충돌
        if (gravity > 0)
        {
            if(CheckCollision(new Vector3(0, yVelocity + 0.3f, 0), Direction.Up))
            {
                gravity = 0;
            }
        }

        // 하단 충돌
        isGrounded = CheckCollision(new Vector3(0, yVelocity, 0), Direction.Down);

        if (true == isGrounded)
            gravity = 0;

        return isGrounded ? 0 : yVelocity;
    }
    private Vector3 CalculateMove()
    {
        float yVelocity = Time.fixedDeltaTime * gravity * 1.2f;
        if(CheckCollision(new Vector3(0, yVelocity, 0), Direction.Down))
            yVelocity = 0;

        if (velocity.x < 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Left))
            velocity.x = 0;
        else if (velocity.x > 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Right))
            velocity.x = 0;

        if (velocity.z < 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Back))
            velocity.z = 0;
        else if (velocity.z > 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Forward))
            velocity.z = 0;

        // 웅크리고 있다면
        if(true == crouch && isGrounded)
        {
            float yVelociety = Time.fixedDeltaTime * -5;

            if (velocity.x < 0 && false == CheckCollision(new Vector3(velocity.x, yVelociety, 0), Direction.Down))
                velocity.x = 0;
            else if (velocity.x > 0 && false == CheckCollision(new Vector3(velocity.x, yVelociety, 0), Direction.Down))
                velocity.x = 0;

            if (velocity.z < 0 && false == CheckCollision(new Vector3(0, yVelociety, velocity.z), Direction.Down))
                velocity.z = 0;
            else if (velocity.z > 0 && false == CheckCollision(new Vector3(0, yVelociety, velocity.z), Direction.Down))
                velocity.z = 0;
        }

        return velocity;
    }
}
