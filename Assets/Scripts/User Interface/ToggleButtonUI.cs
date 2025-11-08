using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField]
    private Button _toggleButton;
    [SerializeField]
    private Image _toggleOn;
    [SerializeField]
    private Image _toggleOff;
    [SerializeField]
    private bool _isToggledOn = true;

    private void Start()
    {
        _toggleButton.onClick.AddListener(ToggleImageState);

        if (_isToggledOn)
        {
            _toggleOn.color = Color.white;
            _toggleOff.color = Color.clear;
        }
        else
        {
            _toggleOn.color = Color.clear;
            _toggleOff.color = Color.white;
        }
    }

    private void ToggleImageState()
    {
        if (_isToggledOn)
        {
            _toggleOn.color = Color.clear;
            _toggleOff.color = Color.white;
            _isToggledOn= false;
        }
        else if (!_isToggledOn)
        {
            _toggleOn.color = Color.white;
            _toggleOff.color = Color.clear;
            _isToggledOn = true;
        }
    }

    public bool GetIsToggledOn()
    {
        return _isToggledOn;
    }

    public void SetIsToggledOn(bool _value)
    {
        _isToggledOn = _value;
    }
}
