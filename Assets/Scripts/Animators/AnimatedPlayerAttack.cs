using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class AnimatedPlayerAttack : AnimatedEntity
{
    public GameObject animation;
    
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
            index = 0;
            animation.SetActive(false);
        }
    }
}
