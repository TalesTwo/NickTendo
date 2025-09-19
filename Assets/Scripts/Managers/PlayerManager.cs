using System;

namespace Managers
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        // Start is called before the first frame update
        void Start()
        {
            EventBroadcaster.PlayerDamaged += OnPlayerDamaged;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    
        private void OnPlayerDamaged(float damageAmount)
        {
            DebugUtils.LogWarning("OnPlayerDamaged received damageAmount: " + damageAmount);
        }
    }
}
