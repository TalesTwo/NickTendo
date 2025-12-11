using Managers;
using System;
using System.Collections;
using UnityEngine;

namespace Interaction
{
    public class BeginCreditsInteraction : TriggerInteractBase
    {
        [SerializeField]
        private SceneField _endingScene;
        [SerializeField]
        private Transform _cageMainTransform;
        
        private bool _waitingForDialogueToEnd = false;
        private bool _hasCalledCinematic = false;
        private bool _hasTalked = false;

        protected override void Start()
        {
            base.Start();
            EventBroadcaster.StopDialogue += OnDialogueEnd;

            //interactPrompt.GetComponent<Transform>().localScale = new Vector2(10f, 10);
        }

        public void OnDialogueEnd()
        {
            // check to see if we were waiting for dialogue to end
            if (_waitingForDialogueToEnd)
            {
                _waitingForDialogueToEnd = false;
                SetInteractAllowedToInteract(false);
                //CreditsManager.Instance.BeginCredits();
                
                StartCageLift();
            }
            
        }
        public override void Interact()
        {
            if(!_hasTalked)
            {
                // Start the dialogue with Nick!
                _waitingForDialogueToEnd = true;
                GameStateManager.Instance.SetBuddeeDialogState("Final");
                EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
                _hasTalked = true;
            }         
        }


        private void StartCageLift()
        {
            StartCoroutine(SlowStart());
        }

        IEnumerator SlowStart()
        {
            float moveTime = 0;
            float startX = _cageMainTransform.localPosition.x;
            float startY = _cageMainTransform.localPosition.y;
            float currentY = startY;
            Managers.AudioManager.Instance.PlayCageRaiseSound();
            while (moveTime < 0.35f)
            {
                moveTime += Time.deltaTime;
                _cageMainTransform.localPosition = new Vector2(startX, currentY);
                float lerpValue = moveTime / 0.35f;
                currentY = Mathf.Lerp(startY, 6f, lerpValue);

                yield return null;
            }
            Invoke(nameof(ContinueCageLift), 1f);
        }

        private void ContinueCageLift()
        {
            StartCoroutine(Continue());
        }

        IEnumerator Continue()
        {
            float soundTime = 0;
            float moveTime = 0;
            float startX = _cageMainTransform.localPosition.x;
            float startY = _cageMainTransform.localPosition.y;
            float currentY = startY;
            while (moveTime < 10)
            {
                if (moveTime == 0) Managers.AudioManager.Instance.PlayCageRaiseSound();
                moveTime += Time.deltaTime;
                soundTime += Time.deltaTime;
                if (soundTime >= 1f)
                {
                    soundTime = 0;
                    Managers.AudioManager.Instance.PlayCageRaiseSound();
                }
                _cageMainTransform.localPosition = new Vector2(startX, currentY);
                float lerpValue = moveTime / 10f;
                currentY = Mathf.Lerp(startY, 50f, lerpValue);

                if (currentY >= 10)
                {
                    StartEndCinematic();
                    _hasCalledCinematic = true;
                }

                yield return null;
            }
        }

        private void StartEndCinematic()
        {
            if (!_hasCalledCinematic)
            {
                SceneSwapManager.Instance.SwapScene(_endingScene, 1f, 3f);
            }
        }
    }
}
