using System;
using UnityEngine;

namespace Interaction
{
    public class BeginCreditsInteraction : TriggerInteractBase
    {
        
        
        private bool _waitingForDialogueToEnd = false;

        protected override void Start()
        {
            base.Start();
            EventBroadcaster.StopDialogue += OnDialogueEnd;
        }

        public void OnDialogueEnd()
        {
            // check to see if we were waiting for dialogue to end
            if (_waitingForDialogueToEnd)
            {
                _waitingForDialogueToEnd = false;
                CreditsManager.Instance.BeginCredits();
            }
            
        }
        public override void Interact()
        {
            // Start the dialogue with Nick!
            _waitingForDialogueToEnd = true;
            GameStateManager.Instance.SetBuddeeDialogState("Example2");
            EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
            
        }
    }
}
