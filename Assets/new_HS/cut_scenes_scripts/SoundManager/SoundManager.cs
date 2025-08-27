using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 소스")]
    [Tooltip("배경음악(BGM)을 재생할 AudioSource")]
    public AudioSource bgmSource;
    [Tooltip("효과음(SFX)을 재생할 AudioSource")]
    public AudioSource sfxSource;

    [Header("오디오 클립 목록")]
    [Tooltip("이름으로 관리할 배경음악(BGM) 목록")]
    public Sound[] bgmSounds;
    [Tooltip("이름으로 관리할 효과음(SFX) 목록")]
    public Sound[] sfxSounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string name)
    {
        Sound s = Array.Find(bgmSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("SoundManager: BGM '" + name + "' not found!");
            return;
        }

        bgmSource.clip = s.clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("SoundManager: SFX '" + name + "' not found!");
            return;
        }
        sfxSource.PlayOneShot(s.clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    // --- 아래 함수들이 추가되었습니다 ---

    /// <summary>
    /// 현재 재생 중인 배경음악(BGM)을 멈춥니다.
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 현재 재생 중인 모든 효과음(SFX)을 멈춥니다.
    /// </summary>
    public void StopAllSFX()
    {
        sfxSource.Stop();
    }

    /// <summary>
    /// 모든 사운드(BGM 및 SFX)를 멈춥니다.
    /// </summary>
    public void StopAllSounds()
    {
        StopBGM();
        StopAllSFX();
    }
}