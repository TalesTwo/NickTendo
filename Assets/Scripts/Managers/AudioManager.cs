using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private int _poolSize = 10;
        private List<AudioSource> _sources;

        public float sfxvolumeslider;

        public float musicvolumeslider = 1;

        [Header("Mutes")]
        public bool muteSFX = false;
        public bool muteMusic = false;

        [Header("Player Sounds")]
        public AudioClip swordSwing;
        public AudioClip walkingSound;
        public AudioClip dashSound;
        public List<AudioClip> playerTalkingTones;
        public int amtOfTones = 6;
        public List<AudioClip> playerTalkingTypes;
        public int amtOfTypes = 6;
        public AudioClip playerDamaged;
        public AudioClip swordHitEffect;
        public AudioClip PlayerDeath;
        public AudioClip Interact;
        public AudioClip Deflect;
        public AudioClip HittingWall;

        [Header("BUDEE Effects")]
        public List<AudioClip> BUDEETalkingTones;
        public int BUDDEEToneCount = 3;
        public AudioClip BUDEEDeath;
        public AudioClip RocketArm;
        public AudioClip BUDDEESlam;
        public AudioClip Dizzy;
        public AudioClip BUDDEEShoot;
        public AudioClip BUDDEEDamaged;
        public AudioClip SpawningEnemies;
        public AudioClip Nope;

        [Header("Enemy Effects")]
        public AudioClip enemyDamaged;
        public AudioClip enemyDeath;
        public AudioClip shooterMovement;
        public AudioClip shot;
        public AudioClip shotHit;
        public AudioClip shotMiss;
        public AudioClip followerHit;
        public AudioClip followerMovement;

        [Header("General Sounds")]
        public AudioClip transitionearly;
        public AudioClip transitionend;
        public AudioClip Keyget;
        public AudioClip Coinget;
        public AudioClip ItemGet;
        public AudioClip openDoor;
        public AudioClip unlockDoor;
        public AudioClip explosion;
        public AudioClip crateBreak;
        public AudioClip Heal;
        public AudioClip Pitfall;
        public AudioClip WallSlam;

        [Header("UI Audio")]
        public AudioClip cursorHover;
        public AudioClip cursorSelect;
        public AudioClip buyItem;
        public AudioClip dialogueClick;
        public AudioClip pauseMenu;
        public AudioClip personaMenuOpen;
        public AudioClip personaMenuClose;
        public AudioClip shopMenu;
        public AudioClip uiInvalidClick;

        [Header("Soundtracks")]
        public AudioClip Overworld;
        public AudioClip Boss;
        public AudioClip Shop;
        public AudioClip Credits;
        public AudioClip TitleTheme;
        private AudioClip NullClip = null;



        //variables for the soundtrack
        public AudioSource Musicsource;

        private bool muted = false;

        protected override void Awake()
        {
            base.Awake();

            // Create a pool of AudioSources we can reuse
            _sources = new List<AudioSource>();
            for (int i = 0; i < _poolSize; i++)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f; // 2D by default
                _sources.Add(src);
            }

            AudioSource mus = gameObject.AddComponent<AudioSource>();
            Musicsource = mus;
            mus.playOnAwake = false;
            mus.spatialBlend = 0f;
            mus.loop = true;
            mus.volume = 0;
        }

        /// <summary>
        /// Play a sound effect at a given volume multiplier.  
        /// Volume can be higher than 1.0f to boost the clip.  
        /// If a GameObject is provided, sound plays from its world position (3D).  
        /// </summary>
        /// 

        private void Start()
        {
            EventBroadcaster.PlayerEnteredBossRoom += OnPlayerEnteredBossRoom;
            EventBroadcaster.PlayerEnteredShopRoom += OnPlayerEnteredShopRoom;
        }

        private void OnPlayerEnteredBossRoom(bool bIsInRoom)
        {
            if (bIsInRoom) PlayBossTrack();
            if (!bIsInRoom) PlayOverworldTrack();

        }

        private void OnPlayerEnteredShopRoom(bool IsInRoom)
        {
            if (IsInRoom) PlayShopTrack();
            if (!IsInRoom) PlayOverworldTrack();
        }

        private void Update()
        {

            if(Musicsource.volume != musicvolumeslider)Musicsource.volume = musicvolumeslider;

            if (muteMusic && !muted)
            {
                Musicsource.volume = 0;
                muted = true;
            }
            if (!muteMusic && muted)
            {
                Musicsource.volume = 1;
                muted = false;
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f, float deviation = 0f, GameObject fromObject = null)
        {
            if (muteSFX) return;
            if (clip == null) return;

            AudioSource src = GetFreeSource();
            if (src == null) return;

            src.transform.position = fromObject ? fromObject.transform.position : Camera.main ? Camera.main.transform.position : Vector3.zero;

            src.spatialBlend = fromObject ? 1f : 0f;
            src.volume = volume * sfxvolumeslider;
            src.clip = clip;
            src.pitch = UnityEngine.Random.Range(1 - deviation, 1 + deviation);
            src.Play();
        }
        //Soundtrack Functions and Coroutines
        public void PlayBackgroundSoundtrack(AudioClip clip, float volume = 1f, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f, GameObject fromObject = null)
        {


            AudioSource src = Musicsource;
            if (muteMusic)
            {
                Musicsource.volume = 0;
            }

            if (src == null) return;

            src.transform.position = fromObject ? fromObject.transform.position : Camera.main ? Camera.main.transform.position : Vector3.zero;

            src.spatialBlend = fromObject ? 1f : 0f;
            StopAllCoroutines();
            if (fadeout && src.isPlaying && !muteMusic)
            {
                StartCoroutine(FadeOutAndIn(src, clip, fadeoutspeed, fadeinspeed, fadein, volume*musicvolumeslider));
            }
            else if (fadein && !muteMusic)
            {
                src.Stop();
                src.clip = clip;
                src.volume = 0;
                StartCoroutine(Fadein(src, fadeinspeed, volume*musicvolumeslider));
            }
            else
            {
                src.clip = clip;
                if(!muteMusic)src.volume = volume*musicvolumeslider;
                src.Play();
            }
        } 
                
        private IEnumerator FadeOutAndIn(AudioSource src, AudioClip newClip, float fadeoutspeed = 1f, float fadeinspeed = 1f, bool fadein = false, float volume=1f)
        {
            float oldvolume = src.volume;
            for(float tempvolume = src.volume; tempvolume >= 0; tempvolume -= 0.01f * oldvolume * fadeoutspeed)
            {
                if (tempvolume < 0) tempvolume = 0;
                if (muteMusic) tempvolume = 0;
                src.volume = tempvolume;
                yield return null;
            }
            src.Stop();
            src.clip = newClip;
            if (fadein)
            {
                StartCoroutine(Fadein(src, fadeinspeed, volume));
            }
            else
            {
                if (muteMusic) volume = 0;
                src.volume = volume;
                src.Play();
            }

        }
        private IEnumerator Fadein(AudioSource src, float fadeinspeed = 1f, float oldvolume = 1f)
        {
            src.Play();
            for(float volume = 0; volume <= oldvolume; volume+=0.01f * oldvolume * fadeinspeed)
            {
                if (volume > oldvolume) volume = oldvolume;
                if (muteMusic) volume = 0;
                src.volume = volume;
                yield return null;
            }
        }




        //Player Sounds
        public void PlayWalkingSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(walkingSound, volume, deviation);
        }
        public void PlayDashSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(dashSound, volume, deviation);
        }
        public void PlaySwordSwingSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(swordSwing, volume, deviation);
        }
        public void PlaySwordHitSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(swordHitEffect, volume, deviation);
        }
        public void PlayPlayerDamagedSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(playerDamaged, volume, deviation);
        }
        public void PlayPlayerDeathSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(PlayerDeath, volume, deviation);
        }

        public void PlayPlayerTalkingTone(float volume = 1)
        {
            int tonenumber;
            tonenumber = UnityEngine.Random.Range(0, amtOfTones);
            int typenumber = UnityEngine.Random.Range(0, amtOfTypes);
            AudioClip tone = playerTalkingTones[tonenumber];
            AudioClip type = playerTalkingTypes[typenumber];

            PlaySFX(type, volume);
            PlaySFX(tone, volume);
        }
        public void PlayDeflectSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Deflect, volume, deviation);
        }
        public void PlayHittingWallSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(HittingWall, volume, deviation);
        }

        //BUDDEE Sound
        public void PlayBUDDEEPunchSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(RocketArm, volume, deviation);
        }
        public void PlayBUDDEESlamSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(BUDDEESlam, volume, deviation);
        }
        public void PlayBUDDEEShootSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(BUDDEEShoot, volume, deviation);
        }
        public void PlayBUDDEEDamagedSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(BUDDEEDamaged, volume, deviation);
        }
        public void PlayBUDDEEDyingSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(BUDEEDeath, volume, deviation);
        }
        public void PlaySpawnEnemiesSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(SpawningEnemies, volume, deviation);
        }
        public void PlayBUDDEETalkingTone(float volume = 1)
        {
            int tonenumber;
            tonenumber = UnityEngine.Random.Range(0, BUDDEEToneCount);
            int typenumber = UnityEngine.Random.Range(0, BUDDEEToneCount);
            AudioClip tone = BUDEETalkingTones[tonenumber];
            PlaySFX(tone, volume);
        }
        public void PlayBUDDEEDizzy(float volume = 1, float deviation = 0)
        {
            PlaySFX(Dizzy, volume, deviation);

        }
        public void PlayBUDDEENope(float volume = 1, float deviation = 0)
        {
            PlaySFX(Nope, volume, deviation);

        }


        //General Sounds
        public void PlayPlayerInteractSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Interact, volume, deviation);
        }
        public void PlayFirstTransitionSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(transitionearly, volume, deviation);
        }
        public void PlaySecondTransitionSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(transitionend, volume, deviation);
        }
        public void PlayKeyGetSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Keyget, volume, deviation);
        }
        public void PlayCoinGetSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Coinget, volume, deviation);
        }
        public void PlayItemGetSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(ItemGet, volume, deviation);
        }
        public void PlayHealSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Heal, volume, deviation);
        }
        public void PlayExplosionSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(explosion, volume, deviation);
        }
        public void PlayOpenDoorSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(openDoor, volume, deviation);
        }
        public void PlayUnlockDoorSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(unlockDoor, volume, deviation);
        }
        public void PlayCrateBreakSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(crateBreak, volume, deviation);
        }
        public void PlayPitFallSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Pitfall, volume, deviation);
        }
        public void PlayWallSlamSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(WallSlam, volume, deviation);
        }

        //Enemy Sounds
        public void PlayEnemyDamagedSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(enemyDamaged, volume, deviation);
        }
        public void PlayEnemyDeathSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(enemyDeath, volume, deviation);
        }
        public void PlayFollowMovementSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(followerMovement, volume, deviation);
        }
        public void PlayFollowerHitSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(followerHit, volume, deviation);
        }
        public void PlayRangedEnemyMovementSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shooterMovement, volume, deviation);
        }
        public void PlayEnemyShotSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shot, volume, deviation);
        }
        public void PlayEnemyShotHitSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shotHit, volume, deviation);
        }
        public void PlayEnemyShotMissSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shotMiss, volume, deviation);
        }

        //UI Sounds
        public void PlayUIHoverSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(cursorHover, volume, deviation);
        }
        public void PlayUISelectSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(cursorSelect, volume, deviation);
        }
        public void PlayDialogueSelectSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(dialogueClick, volume, deviation);
        }
        public void PlayItemBuySound(float volume = 1, float deviation = 0)
        {
            PlaySFX(buyItem, volume, deviation);
        }
        public void PlayPauseMenuSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(pauseMenu, volume, deviation);
        }
        public void PlayPersonaMenuOpenSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(personaMenuOpen, volume, deviation);
        }
        public void PlayPersonaMenuCloseSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(personaMenuClose, volume, deviation);
        }
        public void PlayShopMenuSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shopMenu, volume, deviation);
        }

        public void PlayUIInvalidClick(float volume = 1, float deviation = 0)
        {
            PlaySFX(uiInvalidClick, volume, deviation);
        }

        //Soundtrack Functions
        public void PlayOverworldTrack(float volume = 1,bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(Overworld, volume,fadeout,fadeoutspeed, fadein, fadeinspeed);
        }
        public void PlayTitleTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(TitleTheme, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
        }
        public void PlayShopTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(Shop, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
        }
        public void PlayBossTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(Boss, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
        }
        public void PlayCreditsTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(Credits, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
        }
        public void StopTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(NullClip, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
        }

        private AudioSource GetFreeSource()
        {
            foreach (var src in _sources)
            {
                if (!src.isPlaying)
                    return src;
            }
            // If none are free, just reuse the first
            return _sources[0];
        }
    }
}
