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

        [Header("BUDEE Effects")]
        public List<AudioClip> BUDEETalkingTones;
        public AudioClip BUDEEDeath;

        [Header("Enemy Effects")]
        public AudioClip enemyDamaged;
        public AudioClip enemyDeath;
        public AudioClip shot;
        public AudioClip shotHit;
        public AudioClip followerHit;
        public AudioClip followerMovement;

        [Header("General Sounds")]
        public AudioClip transitionearly;
        public AudioClip transitionend;
        public AudioClip Keyget;
        public AudioClip Coinget;
        public AudioClip openDoor;
        public AudioClip unlockDoor;
        public AudioClip hittingSpikes;

        [Header("UI Audio")]
        public AudioClip cursorHover;
        public AudioClip cursorSelect;
        public AudioClip buyItem;
        public AudioClip dialogueClick;

        [Header("Soundtracks")]
        public AudioClip Overworld;
        public AudioClip Boss;
        public AudioClip Shop;
        public AudioClip Credits;
        public AudioClip TitleTheme;



        //variables for the soundtrack
        public AudioSource Musicsource;

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
        public void PlaySFX(AudioClip clip, float volume = 1f, float deviation = 0f, GameObject fromObject = null)
        {
            if (clip == null) return;

            AudioSource src = GetFreeSource();
            if (src == null) return;

            src.transform.position = fromObject ? fromObject.transform.position : Camera.main ? Camera.main.transform.position : Vector3.zero;

            src.spatialBlend = fromObject ? 1f : 0f;
            src.volume = volume;
            src.clip = clip;
            src.pitch = UnityEngine.Random.Range(1 - deviation, 1 + deviation);
            src.Play();
        }
        //Soundtrack Functions and Coroutines
        public void PlayBackgroundSoundtrack(AudioClip clip, float volume = 1f, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f, GameObject fromObject = null)
        {

            AudioSource src = Musicsource;
            if (src == null) return;

            src.transform.position = fromObject ? fromObject.transform.position : Camera.main ? Camera.main.transform.position : Vector3.zero;

            src.spatialBlend = fromObject ? 1f : 0f;
            StopAllCoroutines();
            if (fadeout && src.isPlaying)
            {
                StartCoroutine(FadeOutAndIn(src, clip, fadeoutspeed, fadeinspeed, fadein));
            }
            else if (fadein)
            {
                src.Stop();
                src.clip = clip;
                src.volume = 0;
                StartCoroutine(Fadein(src, fadeinspeed, volume));
            }
            else
            {
                src.clip = clip;
                src.volume = volume;
                src.Play();
            }
        } 
                
        private IEnumerator FadeOutAndIn(AudioSource src, AudioClip newClip, float fadeoutspeed = 1f, float fadeinspeed = 1f, bool fadein = false)
        {
            float oldvolume = src.volume;
            for(float volume = src.volume; volume >= 0; volume -= 0.01f * oldvolume * fadeoutspeed)
            {
                if (volume < 0) volume = 0;
                src.volume = volume;
                yield return null;
            }
            src.Stop();
            src.clip = newClip;
            if (fadein)
            {
                StartCoroutine(Fadein(src, fadeinspeed, oldvolume));
            }
            else
            {
                src.volume = oldvolume;
                src.Play();
            }

        }
        private IEnumerator Fadein(AudioSource src, float fadeinspeed = 1f, float oldvolume = 1f)
        {
            src.Play();
            for(float volume = 0; volume <= oldvolume; volume+=0.01f * oldvolume * fadeinspeed)
            {
                if (volume > oldvolume) volume = oldvolume;
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
        public void PlayPlayerInteractSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(Interact, volume, deviation);
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

        //Enemy Sounds
        public void PlayEnemyDamagedSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(enemyDamaged, volume, deviation);
        }
        public void PlayEnemyDeathSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(enemyDeath, volume, deviation);
        }
        public void PlayEnemyShotSound(float volume = 1, float deviation = 0)
        {
            PlaySFX(shot, volume, deviation);
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


        //Soundtrack Functions
        public void PlayOverworldTrack(float volume = 1,bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(Overworld, volume,fadeout,fadeoutspeed, fadein, fadeinspeed);
        }
        public void PlayTitleTrack(float volume = 1, bool fadeout = false, float fadeoutspeed = 1f, bool fadein = false, float fadeinspeed = 1f)
        {
            PlayBackgroundSoundtrack(TitleTheme, volume, fadeout, fadeoutspeed, fadein, fadeinspeed);
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
