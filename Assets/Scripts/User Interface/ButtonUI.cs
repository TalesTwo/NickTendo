using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void PointerEnter()
    {
        AudioManager.Instance.PlayUIHoverSound();
    }

    public void PointerClick()
    {
        
    }
}
