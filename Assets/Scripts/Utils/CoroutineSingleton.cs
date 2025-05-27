using System.Collections;
using UnityEngine;

public class CoroutineSingleton : MonoBehaviour
{
    public static CoroutineSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
