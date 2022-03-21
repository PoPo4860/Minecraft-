using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidbody : MonoBehaviour
{
    readonly private float playerWidth = 0.3f;
    readonly private float playerHeight = 1.5f;
    private bool isGrounded = false;
    private float gravity = -9.8f;
    private float maxGravity = -9.8f;
    private Vector3 moveVelocity;
    [HideInInspector] public Vector3 velocity;

    public World world;

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
        moveVelocity = CalculateMove(transform.position);
        moveVelocity += CalculateGravity(transform.position) * Vector3.up; // 중력 적용
    }
    private float CalculateGravity(Vector3 pos)
    {
        if (gravity > maxGravity)
        {
            gravity -= Time.fixedDeltaTime * 35;
        }
        else if (gravity < maxGravity)
        {
            gravity = maxGravity;
        }
        float yVelocity = Time.fixedDeltaTime * gravity;
        // 0.2 0.8
        isGrounded =
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||

            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||

            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||

            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z + playerWidth));
        return isGrounded ? 0 : yVelocity;
    }
    private Vector3 CalculateMove(in Vector3 pos)
    {
        float minJumpValue = (int)gameObject.transform.position.y - gameObject.transform.position.y;
        float yVelocity = (false == isGrounded && minJumpValue >= 0.3f) ? Time.fixedDeltaTime * gravity : 0;
        Vector3[] collisionVertex = new Vector3[8]
        {
            pos + new Vector3(velocity.x - playerWidth, yVelocity, -playerWidth),
            pos + new Vector3(velocity.x - playerWidth, yVelocity, +playerWidth),

            pos + new Vector3(velocity.x + playerWidth, yVelocity, -playerWidth),
            pos + new Vector3(velocity.x + playerWidth, yVelocity, +playerWidth),

            pos + new Vector3(-playerWidth, yVelocity, velocity.z - playerWidth),
            pos + new Vector3(+playerWidth, yVelocity, velocity.z - playerWidth),

            pos + new Vector3(-playerWidth, yVelocity, velocity.z + playerWidth),
            pos + new Vector3(+playerWidth, yVelocity, velocity.z + playerWidth)

        };
        if (velocity.x < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[0]) ||
                world.CheckBlockSolid(collisionVertex[1]) ||
                world.CheckBlockSolid(collisionVertex[0] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[1] + (Vector3.up * playerHeight)))
            {
                velocity.x = 0;
            }
        }
        else if (velocity.x > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[2]) ||
                world.CheckBlockSolid(collisionVertex[3]) ||
                world.CheckBlockSolid(collisionVertex[2] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[3] + (Vector3.up * playerHeight)))
            {
                velocity.x = 0;
            }
        }

        if (velocity.z < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[4]) ||
                world.CheckBlockSolid(collisionVertex[5]) ||
                world.CheckBlockSolid(collisionVertex[4] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[5] + (Vector3.up * playerHeight)))
            {
                velocity.z = 0;
            }
        }
        else if (velocity.z > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[6]) ||
                world.CheckBlockSolid(collisionVertex[7]) ||
                world.CheckBlockSolid(collisionVertex[6] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[7] + (Vector3.up * playerHeight)))
            {
                velocity.z = 0;
            }
        }
        return velocity;
    }
}
