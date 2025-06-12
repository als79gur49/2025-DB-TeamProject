using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonController : MonoBehaviour
{
    [Header("�г� ����")]
    [SerializeField] 
    private GameObject settingsPanel;
    [SerializeField] 
    private CanvasGroup settingsCanvasGroup; // ���̵� ȿ����

    [Header("��ư ����")]
    [SerializeField] 
    private Button openSettingsButton;
    [SerializeField] 
    private Button closeSettingsButton;

    [Header("�ִϸ��̼� ����")]
    [SerializeField] 
    private float animationDuration = 0.3f;
    [SerializeField] 
    private Ease easeType = Ease.OutBack;
    [SerializeField] 
    private AnimationType animationType = AnimationType.ScaleAndFade;

    private RectTransform panelRectTransform;
    private Vector3 originalScale;
    private bool isAnimating = false;

    public enum AnimationType
    {
        Scale,           // ũ�� ��ȭ
        Fade,           // ���̵� ��/�ƿ�
        ScaleAndFade,   // ũ�� + ���̵�
        SlideFromTop,   // ������ �����̵�
        SlideFromBottom, // �Ʒ��� �����̵�
        SlideFromLeft,  // ���ʿ��� �����̵�
        SlideFromRight  // �����ʿ��� �����̵�
    }

    private void Awake()
    {
        // ������Ʈ ����
        panelRectTransform = settingsPanel.GetComponent<RectTransform>();
        originalScale = panelRectTransform.localScale;

        // CanvasGroup�� ������ �ڵ� �߰�
        if (settingsCanvasGroup == null)
        {
            settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
            if (settingsCanvasGroup == null)
            {
                settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
            }
        }

        // ��ư �̺�Ʈ ����
        openSettingsButton.onClick.AddListener(OpenPanel);
        closeSettingsButton.onClick.AddListener(ClosePanel);

        // �ʱ� ���� ����
        settingsPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (isAnimating || settingsPanel.activeInHierarchy)
        {
            return;
        }

        isAnimating = true;
        settingsPanel.SetActive(true);

        // ��ư ��Ȱ��ȭ (�ִϸ��̼� �� �ߺ� Ŭ�� ����)
        openSettingsButton.interactable = false;

        switch (animationType)
        {
            case AnimationType.Scale:
                OpenWithScale();
                break;
            case AnimationType.Fade:
                OpenWithFade();
                break;
            case AnimationType.ScaleAndFade:
                OpenWithScaleAndFade();
                break;
            case AnimationType.SlideFromTop:
                OpenWithSlide(Vector2.up);
                break;
            case AnimationType.SlideFromBottom:
                OpenWithSlide(Vector2.down);
                break;
            case AnimationType.SlideFromLeft:
                OpenWithSlide(Vector2.left);
                break;
            case AnimationType.SlideFromRight:
                OpenWithSlide(Vector2.right);
                break;
        }
    }

    public void ClosePanel()
    {
        if (isAnimating || !settingsPanel.activeInHierarchy)
        {
            return;
        }

        isAnimating = true;

        // ��ư ��Ȱ��ȭ
        closeSettingsButton.interactable = false;

        switch (animationType)
        {
            case AnimationType.Scale:
                CloseWithScale();
                break;
            case AnimationType.Fade:
                CloseWithFade();
                break;
            case AnimationType.ScaleAndFade:
                CloseWithScaleAndFade();
                break;
            case AnimationType.SlideFromTop:
                CloseWithSlide(Vector2.up);
                break;
            case AnimationType.SlideFromBottom:
                CloseWithSlide(Vector2.down);
                break;
            case AnimationType.SlideFromLeft:
                CloseWithSlide(Vector2.left);
                break;
            case AnimationType.SlideFromRight:
                CloseWithSlide(Vector2.right);
                break;
        }
    }

    private void OpenWithScale()
    {
        panelRectTransform.localScale = Vector3.zero;

        panelRectTransform.DOScale(originalScale, animationDuration)
            .SetEase(easeType)
            .OnComplete(OnOpenComplete);
    }

    private void OpenWithFade()
    {
        settingsCanvasGroup.alpha = 0f;

        settingsCanvasGroup.DOFade(1f, animationDuration)
            .SetEase(easeType)
            .OnComplete(OnOpenComplete);
    }

    private void OpenWithScaleAndFade()
    {
        panelRectTransform.localScale = Vector3.zero;
        settingsCanvasGroup.alpha = 0f;

        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOScale(originalScale, animationDuration).SetEase(easeType));
        sequence.Join(settingsCanvasGroup.DOFade(1f, animationDuration).SetEase(Ease.OutQuart));
        sequence.OnComplete(OnOpenComplete);
    }

    private void OpenWithSlide(Vector2 direction)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 startPosition = panelRectTransform.anchoredPosition + direction * screenSize;
        Vector2 targetPosition = panelRectTransform.anchoredPosition;

        panelRectTransform.anchoredPosition = startPosition;
        settingsCanvasGroup.alpha = 0f;

        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOAnchorPos(targetPosition, animationDuration).SetEase(easeType));
        sequence.Join(settingsCanvasGroup.DOFade(1f, animationDuration * 0.5f).SetEase(Ease.OutQuart));
        sequence.OnComplete(OnOpenComplete);
    }



    private void CloseWithScale()
    {
        panelRectTransform.DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(OnCloseComplete);
    }

    private void CloseWithFade()
    {
        settingsCanvasGroup.DOFade(0f, animationDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(OnCloseComplete);
    }

    private void CloseWithScaleAndFade()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack));
        sequence.Join(settingsCanvasGroup.DOFade(0f, animationDuration).SetEase(Ease.InQuart));
        sequence.OnComplete(OnCloseComplete);
    }

    private void CloseWithSlide(Vector2 direction)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 targetPosition = panelRectTransform.anchoredPosition + direction * screenSize;

        var sequence = DOTween.Sequence();
        sequence.Append(settingsCanvasGroup.DOFade(0f, animationDuration * 0.5f).SetEase(Ease.InQuart));
        sequence.Join(panelRectTransform.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.InBack));
        sequence.OnComplete(OnCloseComplete);
    }



    private void OnOpenComplete()
    {
        isAnimating = false;
        openSettingsButton.interactable = true;
        closeSettingsButton.interactable = true;

        // ���ټ��� ���� ��Ŀ�� ����
        closeSettingsButton.Select();
    }

    private void OnCloseComplete()
    {
        isAnimating = false;
        settingsPanel.SetActive(false);
        openSettingsButton.interactable = true;

        // ���� ���·� ����
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;

        // ��Ŀ���� ���� ��ư���� ����
        openSettingsButton.Select();
    }



    /// <summary>
    /// �ִϸ��̼� Ÿ���� ��Ÿ�ӿ��� ����
    /// </summary>
    public void SetAnimationType(AnimationType newType)
    {
        animationType = newType;
    }

    /// <summary>
    /// �ִϸ��̼� ���ӽð� ����
    /// </summary>
    public void SetAnimationDuration(float duration)
    {
        animationDuration = Mathf.Max(0.1f, duration);
    }

    /// <summary>
    /// ��¡ Ÿ�� ����
    /// </summary>
    public void SetEaseType(Ease ease)
    {
        easeType = ease;
    }

    /// <summary>
    /// ��� �г� ���� (�ִϸ��̼� ����)
    /// </summary>
    public void OpenPanelInstant()
    {
        if (isAnimating) return;

        settingsPanel.SetActive(true);
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;
        closeSettingsButton.Select();
    }

    /// <summary>
    /// ��� �г� �ݱ� (�ִϸ��̼� ����)
    /// </summary>
    public void ClosePanelInstant()
    {
        if (isAnimating) return;

        settingsPanel.SetActive(false);
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;
        openSettingsButton.Select();
    }


    private void OnDestroy()
    {
        // DOTween ����
        panelRectTransform?.DOKill();
        settingsCanvasGroup?.DOKill();
    }
}


/*
 using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // ��� ������ ��Ʈ�ѷ����� ����. 

    [Header("�г� ����")]
    [SerializeField] private GameObject settingsPanel;

    [Header("��ư ����")]
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;

    private void Awake()
    {
        openSettingsButton.onClick.AddListener(OpenPanel);
        closeSettingsButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}

 */