using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ShopTriggerInteraction : TriggerInteractBase
{
    // Start is called before the first frame update

    private ShopUIManager ShopUIM;
    private bool _haveWeBeenTalkedTo = false;
    private bool _waitingForShopDialogueToFinish = false;

    private string _oldBuddeeState = "";
    protected override void Start()
    {
        base.Start();
        
        // hook up to the on dialogue complete event to open the shop after dialogue
        EventBroadcaster.StopDialogue += OnDialogueComplete;
    }


    private void OnDialogueComplete()
    {
        if (!_waitingForShopDialogueToFinish){return;}
        _waitingForShopDialogueToFinish = false;
        GameStateManager.Instance.SetBuddeeDialogState(_oldBuddeeState);
        OpenShop();
    }

    
    public override void Interact()
    {
        // track whether we've talked to this shop before
        List<ShopTriggerInteraction> talkedToShops = GameStateManager.Instance.GetShopKeepersTalkedTo();

        if (!talkedToShops.Contains(this))
        {
            GameStateManager.Instance.AddShopKeeperTalkedTo(this);
        }
        else
        {
            OpenShop();
            return;
        }

        // how many unique shopkeepers we have talked to so far
        int count = GameStateManager.Instance.GetShopKeepersTalkedTo().Count;

        if (count == 1)
        {
            _waitingForShopDialogueToFinish = true;
            _oldBuddeeState = GameStateManager.Instance.GetBuddeeDialogState();
            GameStateManager.Instance.SetBuddeeDialogState("Shop1");
            EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
            GameStateManager.Instance.SetBuddeeDialogState(_oldBuddeeState);
            GameStateManager.Instance.UpdateNumberOfTimesTalkedToShopkeeper();
        }
        else if (count == 2)
        {
            _waitingForShopDialogueToFinish = true;
            _oldBuddeeState = GameStateManager.Instance.GetBuddeeDialogState();
            GameStateManager.Instance.SetBuddeeDialogState("Shop2");
            EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
            GameStateManager.Instance.SetBuddeeDialogState(_oldBuddeeState);
            GameStateManager.Instance.UpdateNumberOfTimesTalkedToShopkeeper();
        }
        else
        {
            OpenShop();
        }
    }


    private void OpenShop()
    {
        ShopUIM = gameObject.GetComponent<ShopUIManager>();
        if (!ShopUIM.IsInShop)
        {
            ShopUIM.OpenShop();
        }
    }
}
