using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : Singleton<SceneFadeManager>
{

    [SerializeField] private Image _fadeOutImage;
    [Range(0.1f, 10f)][SerializeField] private float _fadeOutSpeed = 1f;
    [Range(0.1f, 10f)][SerializeField] private float _fadeInSpeed = 1f;
    
    [SerializeField] private Color fadeOutStartColor = Color.clear;
    
    // sounds effects for fade in and fade out
    [SerializeField] private AudioClip _fadeOutSFX;
    [SerializeField] private AudioClip _fadeInSFX;
    
    public bool IsFadingOut { get; private set; } = false;
    public bool IsFadingIn { get; private set; } = false;

    private void Awake()
    {
        fadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (IsFadingOut)
        {
            if(_fadeOutImage.color.a < 1f)
            {
                fadeOutStartColor.a += Time.deltaTime * _fadeOutSpeed;
                _fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingOut = false;
            }

        }
        if (IsFadingIn)
        {
            if(_fadeOutImage.color.a > 0f)
            {
                fadeOutStartColor.a -= Time.deltaTime * _fadeInSpeed;
                _fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingIn = false;
            }

        }

    }
    
    public void StartFadeOut()
    {
        if (_fadeOutSFX )
        {
            // play the fade out sound effect
            AudioManager.Instance.PlaySFX(_fadeOutSFX, 5);
        }
        _fadeOutImage.color = fadeOutStartColor;
        IsFadingOut = true;
        
    }
    public void StartFadeIn()
    {
        if (_fadeOutImage.color.a >= 1f)
        {
            if (_fadeInSFX )
            {
                AudioManager.Instance.PlaySFX(_fadeInSFX, 5);
            }
            _fadeOutImage.color = fadeOutStartColor;
            IsFadingIn = true;
        }

    }
    
}
