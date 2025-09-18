using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private int _poolSize = 10;
    private List<AudioSource> _sources;

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
    }

    /// <summary>
    /// Play a sound effect at a given volume multiplier.  
    /// Volume can be higher than 1.0f to boost the clip.  
    /// If a GameObject is provided, sound plays from its world position (3D).  
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f, GameObject fromObject = null)
    {
        if (clip == null) return;

        AudioSource src = GetFreeSource();
        if (src == null) return;

        src.transform.position = fromObject ? fromObject.transform.position : Camera.main ? Camera.main.transform.position : Vector3.zero;

        src.spatialBlend = fromObject ? 1f : 0f; // 3D if from object, else 2D
        src.volume = volume; // ✅ supports values > 1.0
        src.clip = clip;
        src.Play();
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
