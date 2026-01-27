using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;    
}

// 음향 재생
public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    AudioClip bgm;

    [Header("사전 정의된 사운드 목록")]
    [SerializeField]
    List<Sound> soundList;

    [Header("사운드 플레이어 리스트")]
    [SerializeField]
    List<AudioSource> playerList;

    [Header("사운드 믹서")]
    [SerializeField]
    AudioMixerGroup sfxOutput;
    [SerializeField]
    AudioMixerGroup bgmOutput;

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        // BGM은 GameManager에서 명시적으로 재생하도록 변경
        // PlaySound(bgm, SoundType.Bgm);  // 자동 재생 제거

        return true;
    }

    /// <summary>
    /// BGM 재생 (GameManager.Awake에서 호출)
    /// </summary>
    public void PlayBGM()
    {
        if (bgm != null)
        {
            PlaySound(bgm, SoundType.Bgm);
        }
    }

    // 사운드 재생 : 문자열
    public void PlaySound(string _name, SoundType type = SoundType.Sfx)
    {
        // 문자열 -> 클립으로 변환 시도
        AudioClip clip = NameToClip(_name);        
        PlaySound(clip, type);        
    }

    // 사운드 재생 : 클립
    public void PlaySound(AudioClip clip, SoundType type = SoundType.Sfx)
    {
        if (clip == null)
        {
            //Debug.Log("clip is null");
            return;
        }

        // 빈 플레이어 찾고, 없으면 플레이어 추가하여 사용
        AudioSource audio = GetEmptyPlayer();
        if (audio == null) audio = CreateNewSoundPlayer();        

        // 오디오 믹서 할당
        switch (type)
        {
            case SoundType.Bgm:
                audio.outputAudioMixerGroup = bgmOutput;
                // Bgm은 루프 설정
                audio.loop = true;
                break;

            case SoundType.Sfx:
                audio.outputAudioMixerGroup = sfxOutput;
                break;

            default:
                Debug.Log("정의되지 않은 동작의 SoundType : " + type.ToString());
                break;
        }
        
        // 플레이어에서 클립 재생
        audio.clip = clip;
        audio.Play();
    }

    // 재생 중이 아닌 플레이어 구하기
    AudioSource GetEmptyPlayer()
    {
        foreach (AudioSource soundPlayer in playerList)
        {
            if (!soundPlayer.isPlaying) return soundPlayer;
        }

        //Debug.Log("모든 플레이어 재생 중");
        return null;
    }

    // 새로운 사운드 플레이어 생성
    AudioSource CreateNewSoundPlayer()
    {
        GameObject go = new GameObject("Instansiated Sound Player", typeof(AudioSource));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();

        playerList.Add(audio);

        return audio;
    }

    #region 사운드 문자열 <-> 사운드 클립 변환

    AudioClip NameToClip(string _name)
    {
        foreach (Sound sound in soundList)
        {
            if (sound.name == _name) return sound.clip;
        }

        Debug.Log("cannot find sound : " + _name);
        return null;
    }

    string ClipToName(AudioClip clip)
    {
        foreach (Sound sound in soundList)
        {
            if (sound.clip == clip) return sound.name;
        }

        Debug.Log("cannot find Sound : " + clip.name);
        return null;
    }

    #endregion
}
