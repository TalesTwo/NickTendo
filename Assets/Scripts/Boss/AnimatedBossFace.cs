using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBossFace : AnimatedEntity
{
    public SpriteRenderer screen;
    public Sprite redScreen;
    public Sprite blueScreen;
    public float redScreenTime;
    public List<Sprite> idleFrames;
    public List<Sprite> exhaustedFrames;
    public List<Sprite> loadingMinionsFrames;
    public List<Sprite> hurtFrames;
    
    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    public void SetIdleAnimation()
    {
        DefaultAnimationCycle = idleFrames;
    }

    public void SetExhuastedAnimation()
    {
        DefaultAnimationCycle = exhaustedFrames;
    }

    public void SetLoadingMinionsAnimation()
    {
        Interrupt(loadingMinionsFrames);
        screen.sprite = redScreen;
        Invoke(nameof(RemoveRedScreen), redScreenTime);
    }

    public void RemoveRedScreen()
    {
        screen.sprite = blueScreen;
    }

    public void SetHurtAnimation()
    {
        DefaultAnimationCycle = hurtFrames;
    }
}
