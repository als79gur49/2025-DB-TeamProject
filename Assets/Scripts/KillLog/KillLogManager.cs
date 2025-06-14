using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Pair
{
    public string name;
    public Sprite sprite;
}


public class KillLogManager : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] 
    private KillLogPanel killLogPrefab;
    [SerializeField] 
    private Transform killLogParent;

    [Header("���� ��������Ʈ")]
    [SerializeField] 
    private List<Pair> weaponSprites;

    [Header("�α� ����")]
    [SerializeField] 
    private float removeLogTime = 5f;
    [SerializeField] 
    private int maxLogCount = 5;

    [Header("�ִϸ��̼� ����")]
    [SerializeField] 
    private float animationDuration = 0.5f;
    [SerializeField] 
    private Ease addEase = Ease.OutBack;
    [SerializeField] 
    private Ease removeEase = Ease.InBack;
    [SerializeField] 
    private bool useSlideAnimation = true;
    [SerializeField] 
    private Vector2 slideOffset = new Vector2(300f, 0f);

    // ���� Ȱ�� �α׵��� �����ϴ� ť
    private Queue<KillLogPanel> activeLogs = new Queue<KillLogPanel>();
    private RectTransform parentRect;

    void Start()
    {
        parentRect = killLogParent as RectTransform;

        if (parentRect == null)
        {
            Debug.LogError("killLogParent�� RectTransform�� �ƴմϴ�!");
        }
    }

    /// <summary>
    /// ų �α� �߰� (�ִϸ��̼� ����)
    /// </summary>
    public void AddLog(KillLog log)
    {
        // �ִ� �α� �� �ʰ� �� ���� ������ �α� ����
        if (activeLogs.Count >= maxLogCount)
        {
            RemoveOldestLogAnimated();
        }

        // ���� ��������Ʈ ã��
        Pair resultPair = weaponSprites.FirstOrDefault();

        foreach (Pair pair in weaponSprites)
        {
            if (log.Weapon.Contains(pair.name))
            {
                resultPair = pair;
                break;
            }
        }

        // �α� �г� ����
        KillLogPanel clone = Instantiate(killLogPrefab, killLogParent);
        clone.Setup(log.Enemy, resultPair.sprite, log.Self);

        // ť�� �߰�
        activeLogs.Enqueue(clone);

        // �ִϸ��̼ǰ� �Բ� �߰�
        AddLogAnimated(clone);

        // ���� �ð� �� �ڵ� ����
        StartCoroutine(RemoveLogAfterDelay(clone, removeLogTime));
    }

    /// <summary>
    /// �α� �߰� �ִϸ��̼�
    /// </summary>
    private void AddLogAnimated(KillLogPanel logPanel)
    {
        RectTransform logRect = logPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = logPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = logPanel.gameObject.AddComponent<CanvasGroup>();
        }

        if (useSlideAnimation)
        {

            // ���� ���̾ƿ� ������Ʈ�Ͽ� ��Ȯ�� ��ġ ���
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            // Layout�� ����� ���� ��Ȯ�� ��ġ ����
            Vector2 layoutPosition = logRect.anchoredPosition;

            // ���� ��ġ�� �����¸�ŭ �̵�
            Vector2 startPosition = layoutPosition + slideOffset;
            logRect.anchoredPosition = startPosition;
            canvasGroup.alpha = 0f;

            // ��Ȯ�� Layout ��ġ�� �ִϸ��̼�
            Sequence addSequence = DOTween.Sequence();
            addSequence.Append(logRect.DOAnchorPos(layoutPosition, animationDuration).SetEase(addEase))
                      .Join(canvasGroup.DOFade(1f, animationDuration * 0.7f))
                      .OnComplete(() => {
                      });
        }
        else
        {
            // ������ �ִϸ��̼��� ��ġ ������ �����Ƿ� ��������
            logRect.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            // ���̾ƿ� ������Ʈ
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            // ������ + ���̵� �ִϸ��̼�
            Sequence addSequence = DOTween.Sequence();
            addSequence.Append(logRect.DOScale(Vector3.one, animationDuration).SetEase(addEase))
                      .Join(canvasGroup.DOFade(1f, animationDuration * 0.7f))
                      .OnComplete(() => {
                          Debug.Log($"ų �α� �߰� �Ϸ�: {logPanel.name}");
                      });
        }
    }

    /// <summary>
    /// �α� ���� �ִϸ��̼�
    /// </summary>
    private void RemoveLogAnimated(KillLogPanel logPanel, System.Action onComplete = null)
    {
        if (logPanel == null)
        {
            onComplete?.Invoke();
            return;
        }

        RectTransform logRect = logPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = logPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = logPanel.gameObject.AddComponent<CanvasGroup>();
        }

        if (useSlideAnimation)
        {
            // �����̵� �ƿ� �ִϸ��̼�: �������� �����
            Vector2 exitPos = logRect.anchoredPosition - slideOffset;

            Sequence removeSequence = DOTween.Sequence();
            removeSequence.Append(logRect.DOAnchorPos(exitPos, animationDuration).SetEase(removeEase))
                         .Join(canvasGroup.DOFade(0f, animationDuration * 0.5f))
                         .OnComplete(() => {
                             if (logPanel != null)
                             {
                                 Destroy(logPanel.gameObject);
                             }

                             // ���̾ƿ� ������Ʈ
                             if (parentRect != null)
                             {
                                 Canvas.ForceUpdateCanvases();
                                 LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                             }

                             onComplete?.Invoke();
                         });
        }
        else
        {
            // ������ ��� �ִϸ��̼�
            Sequence removeSequence = DOTween.Sequence();
            removeSequence.Append(logRect.DOScale(Vector3.zero, animationDuration).SetEase(removeEase))
                         .Join(canvasGroup.DOFade(0f, animationDuration * 0.5f))
                         .OnComplete(() => {
                             if (logPanel != null)
                             {
                                 Destroy(logPanel.gameObject);
                             }

                             // ���̾ƿ� ������Ʈ
                             if (parentRect != null)
                             {
                                 Canvas.ForceUpdateCanvases();
                                 LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                             }

                             onComplete?.Invoke();
                             Debug.Log("ų �α� ���� �Ϸ�");
                         });
        }
    }

    /// <summary>
    /// ���� ������ �α� ���� (�ִϸ��̼� ����)
    /// </summary>
    private void RemoveOldestLogAnimated()
    {
        if (activeLogs.Count > 0)
        {
            KillLogPanel oldestLog = activeLogs.Dequeue();
            RemoveLogAnimated(oldestLog);
        }
    }

    /// <summary>
    /// ���� �ð� �� �α� ����
    /// </summary>
    private IEnumerator RemoveLogAfterDelay(KillLogPanel logPanel, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���� ť�� �ְ� ��ȿ�� �α����� Ȯ��
        if (logPanel != null && activeLogs.Contains(logPanel))
        {
            // ť���� ����
            var tempList = activeLogs.ToList();
            tempList.Remove(logPanel);
            activeLogs.Clear();
            foreach (var log in tempList)
            {
                if (log != null)
                {
                    activeLogs.Enqueue(log);
                }
            }

            // �ִϸ��̼����� ����
            RemoveLogAnimated(logPanel);
        }
    }
}

/*
 using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pair
{
    public string name;
    public Sprite sprite;
}

public class KillLogManager : MonoBehaviour
{
    [SerializeField]
    private KillLogPanel killLogPrefab;

    [SerializeField]
    private Transform killLogParent;

    // ���� �̸�, ��������Ʈ ����
    [SerializeField]
    private List<Pair> weaponSprites;

    private float removeLogTime = 5f;

    public void AddLog(KillLog log)
    {
        Pair resultPair = weaponSprites.FirstOrDefault();
     
        //log.Weapon���� ���� �̸��� �����ϰ� �ش� ��������Ʈ ��ȯ
        foreach (Pair pair in weaponSprites)
        {
            if (log.Weapon.Contains(pair.name))
            {
                resultPair = pair;
                break;
            }
        }

        KillLogPanel clone = Instantiate(killLogPrefab);
        clone.Setup(log.Enemy, resultPair.sprite, log.Self);
        clone.transform.SetParent(killLogParent);
        Destroy(clone.gameObject, removeLogTime);
    }
}

 
 */