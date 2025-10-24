using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [Header("Tooltip Customization")]
    [SerializeField]
    private RectTransform _canvasTransform;
    [SerializeField]
    private RectTransform _background;
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private float _xPadding;
    [SerializeField]
    private float _yPadding;

    private RectTransform _tooltipTransform;

    private void Awake()
    {
        _tooltipTransform = GetComponent<RectTransform>();
    }

    private void SetTooltipText(string TooltipText)
    {
        _text.SetText(TooltipText);
        // Just forcing the mesh to update so it doesn't break stuff
        _text.ForceMeshUpdate();

        Vector2 _textSize = _text.GetRenderedValues(false);
        Vector2 _padding = new Vector2(_xPadding, _yPadding);
        _background.sizeDelta = _textSize + _padding;
    }

    public void ShowTooltip(string TooltipText)
    {
        gameObject.SetActive(true);
        SetTooltipText(TooltipText);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        Vector2 _tooltipPosition = Input.mousePosition / _canvasTransform.localScale.x;

        if (_tooltipPosition.x + _background.rect.width > _canvasTransform.rect.width)
        {
            //stops tooltip from going out of the screen on the right
            _tooltipPosition.x = _canvasTransform.rect.width - _background.rect.width;
        }
        if (_tooltipPosition.x < 0)
        {
            //stops tooltip from going out of the screen on the left
            _tooltipPosition.x = 0;
        }
        if (_tooltipPosition.y + _background.rect.height > _canvasTransform.rect.height)
        {
            //stops tooltip from going out of the screen on the top
            _tooltipPosition.y = _canvasTransform.rect.height - _background.rect.height;
        }
        if (_tooltipPosition.y < 0)
        {
            //stops tooltip from going out of the screen on the bottom
            _tooltipPosition.y = 0;
        }

        _tooltipTransform.anchoredPosition = _tooltipPosition;
    }
}
