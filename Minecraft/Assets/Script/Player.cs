
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
    private Vector3 velocity;
    #endregion


    readonly private float playerWidth = 0.3f;
    readonly private float playerHeight = 1.5f;
    //readonly private float  playerWidthOffSet = 0.3f;
    //private float boundsTolerance = 0.3f;
    //private float vericalMomentum = 0f;

    private bool isGrounded = false;
    //private bool isJumping = false;
    //private bool isRunning = false;
    //private bool jumpRequested = false;

    [SerializeField] private Transform highlightBlock;
    private Vector3 placeBlock = new Vector3();
    private float checkIncrement = 0.1f;
    private float reach = 8.0f;

    void Start()
    {
        Camera camera = GetComponentInChildren<Camera>();
        cameraTransform = camera.transform;
        SetCursor();

    }
    void Update()
    {
        GetPlayerInputs();
        PlaceCursorBlocks();
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

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            gravity = -maxGravity;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursor();
        }
        if(true == highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                world.GetChunkFromPos(highlightBlock.position).
                    ModifyChunkData(Utile.Vector3ToVector3Int(Utile.PosNormalization(highlightBlock.position).VexelPos), 0);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.GetChunkFromPos(placeBlock).
                    ModifyChunkData(Utile.Vector3ToVector3Int(Utile.PosNormalization(placeBlock).VexelPos), 2);
            }
        }
    }
    private void CalculateVelocity()
    {
        velocity = CalculateMove(transform.position);
        velocity += CalculateGravity(transform.position) * Vector3.up; // 중력 적용
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
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z  +playerWidth)) ||
                                                                                                         
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z  -playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z  +playerWidth)) ||
            
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z - playerWidth)) ||
            
            world.CheckBlockSolid(new Vector3(pos.x + playerWidth, pos.y + yVelocity, pos.z + playerWidth)) ||
            world.CheckBlockSolid(new Vector3(pos.x - playerWidth, pos.y + yVelocity, pos.z + playerWidth));
        return isGrounded ? 0 : yVelocity;
    }
    private Vector3 CalculateMove(in Vector3 pos)
    {
        float yVelocity = isGrounded ? 0 : Time.fixedDeltaTime * gravity;
        Vector3 moveVelocity = Time.fixedDeltaTime * walkSpeed * ((transform.forward * vertical) + (transform.right * horizontal));
        Vector3[] collisionVertex = new Vector3[8]
        {
            pos + new Vector3(moveVelocity.x - playerWidth, yVelocity, -playerWidth),
            pos + new Vector3(moveVelocity.x - playerWidth, yVelocity, +playerWidth),
                                                                            
            pos + new Vector3(moveVelocity.x + playerWidth, yVelocity, -playerWidth),
            pos + new Vector3(moveVelocity.x + playerWidth, yVelocity, +playerWidth),

            pos + new Vector3(-playerWidth, yVelocity, moveVelocity.z - playerWidth),
            pos + new Vector3(+playerWidth, yVelocity, moveVelocity.z - playerWidth),
                                            
            pos + new Vector3(-playerWidth, yVelocity, moveVelocity.z + playerWidth),
            pos + new Vector3(+playerWidth, yVelocity, moveVelocity.z + playerWidth)

        };
        if (moveVelocity.x < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[0]) ||
                world.CheckBlockSolid(collisionVertex[1]) ||
                world.CheckBlockSolid(collisionVertex[0] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[1] + (Vector3.up * playerHeight)))
            {
                moveVelocity.x = 0;
            }
        }
        else if (moveVelocity.x > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[2]) ||
                world.CheckBlockSolid(collisionVertex[3]) ||
                world.CheckBlockSolid(collisionVertex[2] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[3] + (Vector3.up * playerHeight))) 
            {
                moveVelocity.x = 0;
            }
        }
        
        if (moveVelocity.z < 0)
        {
            if (world.CheckBlockSolid(collisionVertex[4]) ||
                world.CheckBlockSolid(collisionVertex[5]) ||
                world.CheckBlockSolid(collisionVertex[4] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[5] + (Vector3.up * playerHeight)))
            {
                moveVelocity.z = 0;
            }
        }
        else if (moveVelocity.z > 0)
        {
            if (world.CheckBlockSolid(collisionVertex[6]) ||
                world.CheckBlockSolid(collisionVertex[7]) ||
                world.CheckBlockSolid(collisionVertex[6] + (Vector3.up * playerHeight)) ||
                world.CheckBlockSolid(collisionVertex[7] + (Vector3.up * playerHeight)))
            {
                moveVelocity.z = 0;
            }
        }
        return moveVelocity;
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

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();
        while(step < reach)
        {
            Vector3 pos = cameraTransform.position + (cameraTransform.forward * step);
            if(world.CheckBlockSolid(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock = lastPos;
                highlightBlock.gameObject.SetActive(true);
                return;
            }
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }
        highlightBlock.gameObject.SetActive(false);

    }
}
