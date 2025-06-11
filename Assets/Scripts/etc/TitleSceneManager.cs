using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Timeline.Actions;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField] 
    private Button startGameButton;
    [SerializeField]
    private string mainSceneName;
    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGame);
    }

    private void OnStartGame()
    {
        string playerName = playerNameInput.text.Trim();
        Debug.Log(playerName);
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("�÷��̾� �̸��� �Է����ּ���!");
            return;
        }

        // PlayerDataManager�� ����
        PlayerDataManager.Instance.SetPlayerName(playerName);

        // ���� ������ �̵�
        SceneManager.LoadScene(mainSceneName);
    }
}