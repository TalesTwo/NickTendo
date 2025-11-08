using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffectsUI : MonoBehaviour
{
    [SerializeField]
    private float _effectTime = 0.2f;
    [SerializeField]
    private float _scaleFactor = 1.5f;

    private RectTransform _rectTransform;
    private float _baseScaleX;
    private float _baseScaleY;
    private bool _isEffectRunning;

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();        
    }

    private void Start()
    {
        _baseScaleX = _rectTransform.localScale.x;
        _baseScaleY = _rectTransform.localScale.y;
        _isEffectRunning = false;
    }

    public void StartBreathe()
    {
        _isEffectRunning = true;
        StartCoroutine(BreathCycle1());
    }

    public void StopBreathe()
    {
        _isEffectRunning = false;
        StopAllCoroutines();
        _rectTransform.localScale = new Vector2(_baseScaleX, _baseScaleY);
    }

    public void IncreaseSize()
    {
        _isEffectRunning = true;
        Invoke(nameof(ResetEffectRunningBool), _effectTime/2);
        StartCoroutine(IncreaseSizeCR());
    }

    public void DecreaseSize()
    {
        _isEffectRunning = true;
        Invoke(nameof(ResetEffectRunningBool), _effectTime / 2);
        StartCoroutine(DecreaseSizeCR());
    }

    private void ResetEffectRunningBool()
    {
        _isEffectRunning = false;
    }

    private IEnumerator IncreaseSizeCR()
    {
        float _runTime = 0;
        while (_runTime  < _effectTime)
        {
            _runTime += Time.deltaTime;
            float _lerpValue = _runTime / (_effectTime/2);
            float _tempX = Mathf.Lerp(_baseScaleX, _baseScaleX * _scaleFactor, _lerpValue);
            float _tempY = Mathf.Lerp(_baseScaleY, _baseScaleY * _scaleFactor, _lerpValue);
            Vector2 _tempVector = new Vector2(_tempX, _tempY);
            _rectTransform.localScale = _tempVector;
            yield return null;
        }        
    }

    private IEnumerator DecreaseSizeCR()
    {
        float _runTime = 0;
        while ( _runTime < _effectTime)
        {
            _runTime += Time.deltaTime;
            float _lerpValue = _runTime/ (_effectTime/2);
            float _tempX = Mathf.Lerp(_baseScaleX * _scaleFactor, _baseScaleX, _lerpValue);
            float _tempY = Mathf.Lerp(_baseScaleY * _scaleFactor, _baseScaleY, _lerpValue);
            Vector2 _tempVector = new Vector2(_tempX, _tempY);
            _rectTransform.localScale = _tempVector;
            yield return null;
        }
    }

    private IEnumerator BreathCycle1()
    {
        float _runTime = 0;
        while (_runTime < _effectTime)
        {
            _runTime += Time.deltaTime;
            float _lerpValue = _runTime / (_effectTime / 2);
            float _tempX = Mathf.Lerp(_baseScaleX, _baseScaleX * _scaleFactor, _lerpValue);
            float _tempY = Mathf.Lerp(_baseScaleY, _baseScaleY * _scaleFactor, _lerpValue);
            Vector2 _tempVector = new Vector2(_tempX, _tempY);
            _rectTransform.localScale = _tempVector;
            yield return null;
        }
        StartCoroutine(BreathCycle2());
    }

    private IEnumerator BreathCycle2()
    {
        float _runTime = 0;
        while (_runTime < _effectTime)
        {
            _runTime += Time.deltaTime;
            float _lerpValue = _runTime / (_effectTime / 2);
            float _tempX = Mathf.Lerp(_baseScaleX * _scaleFactor, _baseScaleX, _lerpValue);
            float _tempY = Mathf.Lerp(_baseScaleY * _scaleFactor, _baseScaleY, _lerpValue);
            Vector2 _tempVector = new Vector2(_tempX, _tempY);
            _rectTransform.localScale = _tempVector;
            yield return null;
        }
        StartCoroutine(BreathCycle1());
    }

    public bool GetIsEffecetRunning()
    {
        return _isEffectRunning;
    }

    public void ResetScale()
    {
        _rectTransform.localScale = new Vector2(_baseScaleX, _baseScaleY);
    }
}
