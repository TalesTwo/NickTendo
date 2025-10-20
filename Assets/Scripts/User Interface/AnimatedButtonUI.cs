using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedButtonUI : AnimatedEntity
{
    [SerializeField]
    private List<Sprite> _buttonAnimation;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {
        DebugUtils.Log("I am clicked");
        Interrupt(_buttonAnimation);
    }
}
