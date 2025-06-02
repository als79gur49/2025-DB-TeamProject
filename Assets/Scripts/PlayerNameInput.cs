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
                // �� ������ GameManager�� ã��
                instance = FindObjectOfType<PlayerNameInput>();

                if (instance == null)
                {
                    // ���ٸ� �� GameObject�� ����� �߰�
                    GameObject singletonObject = new GameObject("PlayerNameInput");
                    instance = singletonObject.AddComponent<PlayerNameInput>();
                }
            }

            // �Ź� ������ �� �ʱ�ȭ �޼��� ȣ��
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