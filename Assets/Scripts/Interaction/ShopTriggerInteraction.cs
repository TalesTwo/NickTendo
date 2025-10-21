using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTriggerInteraction : TriggerInteractBase
{
    // Start is called before the first frame update

    private ShopUIManager ShopUIM;

    public override void Interact()
    {
        ShopUIM = gameObject.GetComponent<ShopUIManager>();
        if (!ShopUIM.IsInShop)
        {
            ShopUIM.OpenShop();
        }
    }
}
