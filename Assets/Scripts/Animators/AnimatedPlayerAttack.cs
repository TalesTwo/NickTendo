using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class AnimatedPlayerAttack : AnimatedEntity
{
    public GameObject animation;

    private bool _nextLoop;
    
    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
    }

    private void Awake()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        if (index == DefaultAnimationCycle.Count - 1)
        {
            _nextLoop = true;
        }
        else if (_nextLoop)
        {
            animation.SetActive(false);
            _nextLoop = false;
        }
    }
}
