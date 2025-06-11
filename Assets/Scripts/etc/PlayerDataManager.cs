using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    // TitleScene���� ���õ� �����͵� ���ξ����� �����뵵

    public static PlayerDataManager Instance { get; private set; }

    public string PlayerName { get; private set; } = "";
    public int SelectedSkinIndex { get; private set; } = 0;

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        Debug.Log($"�÷��̾� �̸� ����: {PlayerName}");
    }

    public void SetPlayerSkin(int skinIndex)
    {
        SelectedSkinIndex = skinIndex;
    }


    public void ResetGameData()
    {
        PlayerName = "";
        SelectedSkinIndex = 0;
    }
}