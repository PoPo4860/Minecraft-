
using UnityEngine;

public class Player : MonoBehaviour
{
    #region .
    private float walkSpeed = 6f;
    private readonly float jumpPower = 9.8f;
    #endregion
    private Transform cameraTransform;

    #region .
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    #endregion

    

    [SerializeField] private Transform highlightBlock;
    private Vector3 placeBlock = new Vector3();
    private readonly float checkIncrement = 0.1f;
    private readonly float reach = 8.0f;

    public GameObject playerUI;
    private bool activePlayerUI = false;

    public VoxelRigidbody playerRigi;
    public PlayerQuickSlot playerQuickSlot;
    private World world
    {
        get { return World.Instance; }
    }
    void Start()
    {
        Camera camera = GetComponentInChildren<Camera>();
        cameraTransform = camera.transform;
        playerUI.SetActive(false);
    }
    void Update()
    {
        GetPlayerUIInput();
        if (true == activePlayerUI)
            return;

        GetPlayerControlInput();
        PlaceCursorBlocks();
    }

    void SetPlayerUI(bool check)
    {
        playerUI.SetActive(check);
    }
    private void FixedUpdate()
    {
        Vector3 velocityVector = ((transform.forward * vertical) + (transform.right * horizontal));
        playerRigi.SetVelocity(Time.fixedDeltaTime * walkSpeed * velocityVector.normalized);
    }
    private void LateUpdate()
    {
        MoveAndRotate();
    }
    private void GetPlayerUIInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            activePlayerUI = !activePlayerUI;
            SetPlayerUI(activePlayerUI);
            horizontal = 0;
            vertical = 0;
            mouseX = 0;
            mouseY = 0;
        }
    }
    private void GetPlayerControlInput()
    {
        #region 플레이어 움직임 관련
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X") * 5;
        mouseY = Input.GetAxis("Mouse Y") * 5;
        if (Input.GetKey(KeyCode.Space))
            playerRigi.InputJump(jumpPower);

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
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerRigi.AddForce(((transform.forward * vertical) + (transform.right * horizontal)));
        }
        #endregion 

        if (true == highlightBlock.gameObject.activeSelf)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    world.GetChunkFromPos(highlightBlock.position).
            //        ModifyChunkData(Utile.Vector3ToVector3Int(Utile.GetCoordInVoxelPosFromWorldPos(highlightBlock.position).voxelPos), CodeData.BLOCK_AIR);
            //}
            if (Input.GetMouseButtonDown(1))
            {
                ushort itemCode = playerQuickSlot.UseQuickSlotItemCode();
                world.GetChunkFromPos(placeBlock).
                    ModifyChunkData(Utile.Vector3ToVector3Int(Utile.GetCoordInVoxelPosFromWorldPos(placeBlock).voxelPos), itemCode);
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
