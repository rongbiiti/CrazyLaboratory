using UnityEngine;
using System;
using System.Collections;


// 音管理クラス
public class SoundManagerV2 : MonoBehaviour
{

    protected static SoundManagerV2 instance;

    public static SoundManagerV2 Instance
    {
        get
        {
            if (instance == null) {
                instance = (SoundManagerV2)FindObjectOfType(typeof(SoundManagerV2));

                if (instance == null) {
                    Debug.LogError("SoundManagerV2 Instance Error");
                }
            }

            return instance;
        }
    }

    // 音量
    public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    private AudioSource BGMsource;
    // SE
    private AudioSource[] SEsources = new AudioSource[16];
    // 音声
    private AudioSource[] VoiceSources = new AudioSource[16];

    // === AudioClip ===
    // BGM
    public AudioClip[] BGM;
    // SE
    public AudioClip[] SE;
    // 音声
    public AudioClip[] Voice;

    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManagerV2");
        if (obj.Length > 1) {
            // 既に存在しているなら削除
            Destroy(gameObject);
        } else {
            // 音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }

        // 全てのAudioSourceコンポーネントを追加する

        // BGM AudioSource
        BGMsource = gameObject.AddComponent<AudioSource>();
        // BGMはループを有効にする
        BGMsource.loop = true;

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++) {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        // 音声 AudioSource
        for (int i = 0; i < VoiceSources.Length; i++) {
            VoiceSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        volume.BGM = SaveManager.Instance.save.BGMVolume;
        volume.SE = SaveManager.Instance.save.SEVolume;
    }

    void Update()
    {
        // ミュート設定
        BGMsource.mute = volume.Mute;
        foreach (AudioSource source in SEsources) {
            source.mute = volume.Mute;
        }
        foreach (AudioSource source in VoiceSources) {
            source.mute = volume.Mute;
        }

        // ボリューム設定
        BGMsource.volume = volume.BGM;
        foreach (AudioSource source in SEsources) {
            source.volume = volume.SE;
        }
        foreach (AudioSource source in VoiceSources) {
            source.volume = volume.Voice;
        }
    }



    // ***** BGM再生 *****
    // BGM再生
    public void PlayBGM(int index)
    {
        if (0 > index || BGM.Length <= index) {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsource.clip == BGM[index]) {
            return;
        }
        BGMsource.Stop();
        BGMsource.clip = BGM[index];
        BGMsource.Play();
    }

    // BGM停止
    public void StopBGM()
    {
        BGMsource.Stop();
        BGMsource.clip = null;
    }


    // ***** SE再生 *****
    // SE再生
    public void PlaySE(int index)
    {
        if (0 > index || SE.Length <= index) {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources) {
            if (false == source.isPlaying) {
                source.clip = SE[index];
                source.Play();
                return;
            }
        }
    }

    // SE停止
    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources) {
            source.Stop();
            source.clip = null;
        }
    }


    // ***** 音声再生 *****
    // 音声再生
    public void PlayVoice(int index)
    {
        if (0 > index || Voice.Length <= index) {
            return;
        }
        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in VoiceSources) {
            if (false == source.isPlaying) {
                source.clip = Voice[index];
                source.Play();
                return;
            }
        }
    }

    // 音声停止
    public void StopVoice()
    {
        // 全ての音声用のAudioSouceを停止する
        foreach (AudioSource source in VoiceSources) {
            source.Stop();
            source.clip = null;
        }
    }
}

// 音量クラス
[Serializable]
public class SoundVolume
{
    public float BGM = 1.0f;
    public float Voice = 1.0f;
    public float SE = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        BGM = 1.0f;
        Voice = 1.0f;
        SE = 1.0f;
        Mute = false;
    }
}