using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedButtonUI : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _buttonAnimation;
    [SerializeField]
    private float _timeBetweenFrames;

    private Sprite _baseSprite;
    float _currentTime;
    private SpriteRenderer _renderer;
    bool _isPlaying;
    int _frameIndex;


    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _baseSprite = _renderer.sprite;
        _isPlaying = false;
        _frameIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {        
        DebugUtils.Log("I am clicked");
    }

    void Animation()
    {
        if(_isPlaying)
        {
            if (Time.time - _currentTime >= 1/_timeBetweenFrames)
            {
                _currentTime = Time.time;
                _renderer.sprite = _buttonAnimation[_frameIndex];
                _frameIndex++;

            }
        }        
    }
}
