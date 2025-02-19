
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class SalonPanelSwitcher : MonoBehaviour
{
    [Header("----- Panel Container Elements -----")] 
    [SerializeField] private RectTransform panelContainer;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;

    [Header("----- Panel Container Settings -----")]
    [SerializeField] private float tweenDuration;
    private readonly float[] _targetPositions = new float[] { 1400f, 0f, -1400f };
    private readonly int _totalPanels = 3;
    private int _currentPanelIndex = 1;
    private Tween _currentTween;

    private void Start()
    {
        if (panelContainer)
            panelContainer.anchoredPosition = new Vector2(_targetPositions[_currentPanelIndex], panelContainer.anchoredPosition.y);
        
        leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
        rightArrowButton.onClick.AddListener(OnRightArrowClicked);
    }

    private void OnRightArrowClicked()
    {
        if (_currentPanelIndex < _totalPanels - 1)
        {
            _currentPanelIndex++;
            MoveToCurrentPanel();
        }
    }

    private void OnLeftArrowClicked()
    {
        if (_currentPanelIndex > 0)
        {
            _currentPanelIndex--;
            MoveToCurrentPanel();
        }
    }

    private void MoveToCurrentPanel()
    {
        float targetX = _targetPositions[_currentPanelIndex];

        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill();

        _currentTween = panelContainer.DOAnchorPosX(targetX, tweenDuration).SetEase(Ease.OutCubic);
    }
    
    public int GetCurrentPanelIndex()
    {
        return _currentPanelIndex;
    }
}
