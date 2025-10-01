using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public Image Item0Image;
    public Button Item0Button;
    public TextMeshProUGUI Item0Name;
    public TextMeshProUGUI Item0Desc;

    public Image Item1Image;
    public Button Item1Button;
    public TextMeshProUGUI Item1Name;
    public TextMeshProUGUI Item1Desc;

    public Image Item2Image;
    public Button Item2Button;
    public TextMeshProUGUI Item2Name;
    public TextMeshProUGUI Item2Desc;

    public GameObject ShopUI;
    public Button CloseButton;

    // Start is called before the first frame update
    void Start()
    {
        CloseButton.onClick.AddListener(CloseShop);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (ShopUI != null)
            {
                ShopUI.SetActive(true);
            }
        }

        
    }

    void CloseShop()
    {
        ShopUI.SetActive(false);    
    }
}
