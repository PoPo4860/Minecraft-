using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelRigidbody : MonoBehaviour
{
    private float objectWidth = 0.3f;
    private float objectHeight = 1.5f;
    private bool isGrounded = false;
    private float _gravity = -9.8f;
    private Vector3 deceleration;
    private float gravity
    {
        get { return _gravity + velocity.y; }
        set
        {
            if (0 == value)
            {
                _gravity = 0;
                velocityY = 0;
                return;
            }
            _gravity = value;
        }
    }
    private readonly float downMaxGravity = -30f;
    private bool crouch = false;
    private Vector3 moveVelocity;
    private Vector3 addForce;
    private Vector3 _velocity;
    private Vector3 velocity
    {
        get { return _velocity + addForce; }
        set { _velocity = value; addForce = value; }
    }
    private float velocityX
    {
        set { _velocity.x = value; addForce.x = value; }
    }
    private float velocityY
    {
        set { _velocity.y = value; addForce.y = value; }
    }
    private float velocityZ
    {
        set { _velocity.z = value; addForce.z = value; }
    }

    private readonly Vector3[] collisionVertex = new Vector3[4];

    private void Start()
    {
        objectWidth *= gameObject.transform.localScale.x;
        objectHeight *= gameObject.transform.localScale.x;
        collisionVertex[0] = new Vector3(-objectWidth, 0, +objectWidth);
        collisionVertex[1] = new Vector3(-objectWidth, 0, -objectWidth);
        collisionVertex[2] = new Vector3(+objectWidth, 0, +objectWidth);
        collisionVertex[3] = new Vector3(+objectWidth, 0, -objectWidth);
    }

    private void FixedUpdate()
    {
        if (addForce.x != 0 || addForce.y != 0 || addForce.z != 0)
        {
            addForce += (-deceleration) * Time.fixedDeltaTime;
            if (Mathf.Abs(addForce.x) < 0.01f)
            {
                deceleration.x = 0;
                addForce.x = 0;
            }
            if (Mathf.Abs(addForce.y) < 0.01f)
            {
                deceleration.y = 0;
                addForce.y = 0;
            }
            if (Mathf.Abs(addForce.z) < 0.01f)
            {
                deceleration.z = 0;
                addForce.z = 0;
            }
        }
        CalculateVelocity();
        transform.Translate(moveVelocity, Space.World);
    }
    private enum Direction { Forward, Back, Left, Right, Down, Up };
    public void InputJump(float jumpPower)
    {
        if (isGrounded == true)
            gravity = jumpPower;
    }
    public void SetVelocity(in Vector3 vec)
    {
        _velocity = vec;
    }
    public void AddForce(in Vector3 vec)
    {
        addForce += vec / 2;
        deceleration = addForce.normalized;
    }
    public void SetForce(in Vector3 vec)
    {
        addForce = vec;
        deceleration = addForce.normalized;
    }
    public void InputShift(in bool input)
    {
        crouch = input;
    }

    private bool CheckCollision(in Vector3 vel, in Direction dir)
    {
        if (dir == Direction.Left)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (objectHeight / 2))) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (objectHeight / 2))));
        }
        else if (dir == Direction.Right)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * (objectHeight / 2))) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * (objectHeight / 2))));
        }
        else if (dir == Direction.Forward)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * (objectHeight / 2))) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * (objectHeight / 2))));
        }
        else if (dir == Direction.Back)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * (objectHeight / 2))) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * (objectHeight / 2))));
        }
        else if (dir == Direction.Down)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2]) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3]));
        }
        else if (dir == Direction.Up)
        {
            return (World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[0] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[1] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[2] + (Vector3.up * objectHeight)) ||
                    World.Instance.CheckBlockSolid(transform.position + vel + collisionVertex[3] + (Vector3.up * objectHeight)));
        }
        return false;
    }
    private void CalculateVelocity()
    {
        moveVelocity = CalculateMove();
        moveVelocity += CalculateGravity() * Vector3.up;
    }
    private Vector3 CalculateMove()
    {
        float yVelocity = Time.fixedDeltaTime * gravity * 1.2f;
        if (CheckCollision(new Vector3(0, yVelocity, 0), Direction.Down))
            yVelocity = 0;

        if (velocity.x < 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Left))
            velocityX = 0;
        else if (velocity.x > 0 && CheckCollision(new Vector3(velocity.x, yVelocity, 0), Direction.Right))
            velocityX = 0;

        if (velocity.z < 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Back))
            velocityZ = 0;
        else if (velocity.z > 0 && CheckCollision(new Vector3(0, yVelocity, velocity.z), Direction.Forward))
            velocityZ = 0;

        // 웅크리고 있다면
        if (true == crouch && true == isGrounded)
        {
            float yVelociety = Time.fixedDeltaTime * -5;

            if (velocity.x < 0 && false == CheckCollision(new Vector3(velocity.x, yVelociety, 0), Direction.Down))
                velocityX = 0;
            else if (velocity.x > 0 && false == CheckCollision(new Vector3(velocity.x, yVelociety, 0), Direction.Down))
                velocityX = 0;

            if (velocity.z < 0 && false == CheckCollision(new Vector3(0, yVelociety, velocity.z), Direction.Down))
                velocityZ = 0;
            else if (velocity.z > 0 && false == CheckCollision(new Vector3(0, yVelociety, velocity.z), Direction.Down))
                velocityZ = 0;
        }

        return velocity;
    }
    private float CalculateGravity()
    {
        if (gravity > downMaxGravity)
            gravity -= Time.fixedDeltaTime * 35;
        else
            gravity = downMaxGravity;


        float yVelocity = Time.fixedDeltaTime * gravity;

        // 상단 충돌
        if (0 < yVelocity || 0 < velocity.y)
        {
            if (CheckCollision(new Vector3(0, yVelocity + 0.3f, 0), Direction.Up))
            {
                yVelocity = 0;
                gravity = 0;
            }
        }

        // 하단 충돌
        isGrounded = CheckCollision(new Vector3(0, yVelocity, 0), Direction.Down);

        if (true == isGrounded)
        {
            yVelocity = 0;
            gravity = 0;
        }

        return isGrounded ? 0 : yVelocity;
    }
}
