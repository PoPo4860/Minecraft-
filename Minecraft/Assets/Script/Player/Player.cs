
using UnityEngine;

public class Player : MonoBehaviour
{
    #region .
    [SerializeField] private World world;
    private float walkSpeed = 6f;

    #endregion
    private Transform cameraTransform;

    #region .
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    #endregion

    //private float boundsTolerance = 0.3f;
    //private float vericalMomentum = 0f;

    //private bool isJumping = false;
    //private bool isRunning = false;
    //private bool jumpRequested = false;

    [SerializeField] private Transform highlightBlock;
    private Vector3 placeBlock = new Vector3();
    private readonly float checkIncrement = 0.1f;
    private readonly float reach = 8.0f;

    public PlayerRigidbody playerRigi;
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
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState != CursorLockMode.None ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private void FixedUpdate()
    {
        playerRigi.velocity = Time.fixedDeltaTime * walkSpeed * ((transform.forward * vertical) + (transform.right * horizontal));
    }
    private void LateUpdate()
    {
        MoveAndRotate();
    }
    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X") * 5;
        mouseY = Input.GetAxis("Mouse Y") * 5;
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Space))
            playerRigi.InputJump();

        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursor();

        if (Input.GetKeyDown(KeyCode.R)) 
            walkSpeed = 10;

        if (Input.GetKeyUp(KeyCode.R)) 
            walkSpeed = 6;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerRigi.InputShift(true);
            walkSpeed = 3;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerRigi.InputShift(false);
            walkSpeed = 6;
        }

        if (true == highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                world.GetChunkFromPos(highlightBlock.position).
                    ModifyChunkData(Utile.Vector3ToVector3Int(Utile.GetCoordInVoxelPosFromWorldPos(highlightBlock.position).VexelPos), CodeData.BLOCK_AIR);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.GetChunkFromPos(placeBlock).
                    ModifyChunkData(Utile.Vector3ToVector3Int(Utile.GetCoordInVoxelPosFromWorldPos(placeBlock).VexelPos), CodeData.BLOCK_BEDROCK);
            }
        }
    }
    private void MoveAndRotate()
    {
        transform.Rotate(Vector3.up * mouseX);
        Vector3 cameroRotate = Vector3.right * -mouseY;
        cameraTransform.Rotate(cameroRotate);
        if (cameraTransform.eulerAngles.z >= 170.0f)
        {
            Vector3 cameraAngle = cameraTransform.eulerAngles;
            if (cameraTransform.eulerAngles.x <= 90)
            {
                cameraAngle.x = 90;
            }
            else
            {
                cameraAngle.x = 270;
            }
            cameraTransform.eulerAngles = cameraAngle;
        }
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
