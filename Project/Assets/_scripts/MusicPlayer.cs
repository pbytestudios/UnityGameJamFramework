using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;
using Pixelbyte;
using UnityEngine.Rendering;

public class MusicPlayer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] bool persistScenes = true;

    float finalVol = 1.0f;

    public float Volume
    {
        get => audioSource.volume;
        set => audioSource.volume = value;
    }

    public void SetToFinalVolme() => Volume = finalVol;

    void Awake()
    {
        //Check if a Music player is already in the scene and act accordingly
        var existing = FindObjectOfType<MusicPlayer>();
        if (existing != null && existing.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        if (persistScenes)
            GameObject.DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        finalVol = audioSource.volume;
    }

    public void ToggleMusic(float fadeTime = 0.5f)
    {
        if (audioSource.isPlaying)
        {
            Stop(fadeTime);
        }
        else
        {
            Play(fadeTime);
        }
    }

    public void Stop(float fadeTime = 0)
    {
        fadeTime = Mathf.Max(0, fadeTime);
        if (audioSource.isPlaying)
        {
            audioSource.DOKill();

            if (fadeTime > 0)
                audioSource.DOFade(0, fadeTime).SetEase(Ease.OutQuint).OnComplete(() => audioSource.Stop());
            else
                audioSource.Stop();
        }
    }

    public void Play(float fadeTime = 0)
    {
        fadeTime = Mathf.Max(0, fadeTime);
        audioSource.DOKill();
        if (fadeTime > 0)
        {
            audioSource.volume = 0;
            audioSource.Play();
            audioSource.DOFade(finalVol, fadeTime).SetEase(Ease.InQuint);
        }
        else
        {
            audioSource.volume = finalVol;
            audioSource.Play();
        }
    }
}
