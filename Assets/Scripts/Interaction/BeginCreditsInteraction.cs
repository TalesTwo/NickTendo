using UnityEngine;

namespace Interaction
{
    public class BeginCreditsInteraction : TriggerInteractBase
    {
        public override void Interact()
        {
            Debug.Log("Beginning Credits...");
            CreditsManager.Instance.BeginCredits();
        }
    }
}
