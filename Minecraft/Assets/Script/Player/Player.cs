
using UnityEngine;

public class Player : MonoBehaviour
{
    #region .
    private float walkSpeed = 6f;
    private readonly float jumpPower = 9.8f;
    #endregion

    #region .
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    #endregion

    [HideInInspector] public Transform cameraTransform;
    [SerializeField] private Transform highlightBlock;
    private Vector3 placeBlock = new Vector3();
    private readonly float checkIncrement = 0.1f;
    private readonly float reach = 8.0f;

    private bool activePlayerUI = false;

    public VoxelRigidbody playerRigidbody;
    public PlayerQuickSlot playerQuickSlot;
    public PlayerRightHand playerRightHand;

    private World world
    {
        get { return World.Instance; }
    }

    void Start()
    {
        Camera camera = GetComponentInChildren<Camera>();
        cameraTransform = camera.transform;
        UIManager.Instance.ClearUI();
    }
    void Update()
    {
        GetPlayerUIInput();
        if (true == activePlayerUI)
            return;

        GetPlayerControlInput();
        PlaceCursorBlocks();
    }
    private void FixedUpdate()
    {
        Vector3 velocityVector = ((transform.forward * vertical) + (transform.right * horizontal));
        playerRigidbody.SetVelocity(Time.fixedDeltaTime * walkSpeed * velocityVector.normalized);
    }
    private void LateUpdate()
    {
        MoveAndRotate();
    }

    private void GetPlayerUIInput()
    {
        if (true == Input.GetKeyDown(KeyCode.E))
        {
            if (true == activePlayerUI)
            {
                activePlayerUI = false;
                UIManager.Instance.ClearUI();
            }
            else
            {
                activePlayerUI = true;
                UIManager.Instance.ActiveIventoryUI();
            }
            ClearMouseInput();
        }
    }
    private void ClearMouseInput()
    {
        horizontal = 0;
        vertical = 0;
        mouseX = 0;
        mouseY = 0;
    }
    private void GetPlayerControlInput()
    {
        #region 플레이어 움직임 관련 입력
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X") * 5;
        mouseY = Input.GetAxis("Mouse Y") * 5;
        if (Input.GetKey(KeyCode.Space))
            playerRigidbody.InputJump(jumpPower);

        if (Input.GetKeyDown(KeyCode.R)) 
            walkSpeed = 10;

        if (Input.GetKeyUp(KeyCode.R)) 
            walkSpeed = 6;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerRigidbody.InputShift(true);
            walkSpeed = 3;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerRigidbody.InputShift(false);
            walkSpeed = 6;
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Q))
        {   // 아이템 버리기
            int itemSlot = playerQuickSlot.currentSelectNum + 27;
            if (true == Input.GetKey(KeyCode.LeftShift))
                UIManager.Instance.playerInventory.DropItemFromInventoy(itemSlot, 64);
            else
                UIManager.Instance.playerInventory.DropItemFromInventoy(itemSlot, 1);
        }

        if (Input.GetMouseButton(0))
            playerRightHand.ActiveHandMove();

        if (Input.GetMouseButtonDown(1) && true == highlightBlock.gameObject.activeSelf)
        {
            bool blockInstall = false;
            int targetItemCode = Utile.GetVoxelStateFromWorldPos(GetHighLightBlockPos()).id;
            
            if (true == Input.GetKey(KeyCode.LeftShift))
                blockInstall = true;
            else
            {
                if (CodeData.BLOCK_CraftingTable == targetItemCode)
                {
                    UIManager.Instance.ActiveCraftingUI();
                    activePlayerUI = true;
                    ClearMouseInput();
                }
                else if (CodeData.BLOCK_Furnace == targetItemCode ||
                         CodeData.BLOCK_FurnaceFire == targetItemCode)
                {
                    UIManager.Instance.ActiveFurnaceUI();
                    activePlayerUI = true;
                    ClearMouseInput();
                }
                else
                    blockInstall = true;
            }

            if(true == blockInstall)
            {
                float angle = gameObject.transform.rotation.eulerAngles.y;
                Vector2Int dir = new Vector2Int();
                if (45f < angle && angle <= 135f)
                    ++dir.x; // +X
                else if (225f < angle && angle <= 315f)
                    --dir.x; // -X
                else if (135f < angle && angle <= 225f)
                    --dir.y; // -Z
                else
                    ++dir.y; // -X

                ushort itemCode = playerQuickSlot.UseQuickSlotItemCode();
                if (null != CodeData.GetBlockInfo(itemCode))
                {
                    playerRightHand.ActiveHandMove();
                    Vector3Int voxelPos = Utile.Vector3ToVector3Int(Utile.GetCoordInVoxelPosFromWorldPos(placeBlock).voxelPos);
                    world.GetChunkFromPos(placeBlock).ModifyChunkData(voxelPos, itemCode, dir);
                }   
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
    
    public Vector3 GetHighLightBlockPos()
    {
        return highlightBlock.gameObject.transform.position;
    }
}
