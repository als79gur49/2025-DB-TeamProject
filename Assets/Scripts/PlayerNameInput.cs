using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    private static PlayerNameInput instance;

    private string playerName;
    public string PlayerName 
    { 
        get 
        { 
            return playerName; 
        } 
        set
        {
            playerName = value;
        }
           
    }

    public static PlayerNameInput Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬 내에서 GameManager를 찾음
                instance = FindObjectOfType<PlayerNameInput>();

                if (instance == null)
                {
                    // 없다면 새 GameObject를 만들어 추가
                    GameObject singletonObject = new GameObject("PlayerNameInput");
                    instance = singletonObject.AddComponent<PlayerNameInput>();
                }
            }

            // 매번 접근할 때 초기화 메서드 호출
            return instance;
        }
    }

    private bool isInitialized = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}