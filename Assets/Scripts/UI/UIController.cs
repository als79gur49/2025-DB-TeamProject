using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameOverUI gameOverPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel?.ShowGameOver();
        }
    }
}
