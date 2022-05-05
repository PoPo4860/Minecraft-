using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public ItemManager itemManager;
    public UIManager uiManager;
    public MouseItemSlot mouseItemSlot;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if(null == _instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
