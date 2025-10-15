using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPlayer : AnimatedEntity
{
    [Header("Animation Cycles")]
    private bool _isAttacking = false;
    public List<Sprite> attackAnimation;
    public List<Sprite> dashAnimation;
    public List<Sprite> walkAnimation;
    
    // Start is called before the first frame update
    private void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    private void Update()
    {
        AnimationUpdate();
    }

    public void SetAttacking()
    {
        _isAttacking = true;
        Interrupt(attackAnimation);
    }
}
