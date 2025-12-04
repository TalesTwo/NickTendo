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
    public List<Sprite> blockedFrames;
    public float blockScreenTime;
    
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
        SetRedScreen(redScreenTime);
    }

    public void RemoveRedScreen()
    {
        screen.sprite = blueScreen;
    }

    public void SetRedScreen(float time)
    {
        screen.sprite = redScreen;
        Invoke(nameof(RemoveRedScreen), time);
    }

    public void SetHurtAnimation()
    {
        DefaultAnimationCycle = hurtFrames;
    }

    public void SetBlockedAnimation()
    {
        DefaultAnimationCycle = blockedFrames;
        SetRedScreen(blockScreenTime);
        Invoke(nameof(SetIdleAnimation), blockScreenTime);
    }
}
