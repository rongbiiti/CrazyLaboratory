﻿using System;
using UnityEngine;
using UnityEngineInternal;
using System.Collections;

public class TitleSceneManager : MonoBehaviour {

    private bool isAnyKeyPress;
    private bool isDicide;

    [SerializeField, CustomLabel("Logoのアニメーター")]
    private Animator _animator;

    [SerializeField, CustomLabel("PleaseAnyKey")]
    private GameObject _plsAnyKey;

    [SerializeField, CustomLabel("StartMenuPanel")]
    private GameObject _startMenu;

    [SerializeField, CustomLabel("StageSelect")]
    private GameObject _stageSelect;

    [SerializeField, CustomLabel("Option")]
    private GameObject _option;

    [SerializeField, CustomLabel("Quit")]
    private GameObject _quit;

    [SerializeField, CustomLabel("Stage1")]
    private GameObject _stage1;

    [SerializeField, CustomLabel("Stage2")]
    private GameObject _stage2;

    [SerializeField, CustomLabel("Stage3")]
    private GameObject _stage3;

    private float anitime = 0f;

    private void Start()
    {
        SoundManagerV2.Instance.StopBGM();
    }

    void Update()
    {
        if (Input.anyKey && !isAnyKeyPress) {
            _animator.SetTrigger("Decision1");
            SoundManagerV2.Instance.PlaySE(21);
            _plsAnyKey.SetActive(false);
            StartMenuOpen();
            isAnyKeyPress = true;
        } else if (InputManager.Instance.JumpKey == 1 && isAnyKeyPress && !_startMenu.activeSelf && !isDicide) {
            StartMenuOpen();
        }
    }

    public void NewGame()
    {
        _animator.SetTrigger("Decision2");
        StartCoroutine(TransScene("PrologueScene"));
        SoundManagerV2.Instance.PlaySE(16);
        _startMenu.SetActive(false);
        SaveManager.Instance.IsNewGame = true;
        SaveManager.Instance.Save(9999, 1);
        isDicide = true;
        ScoreManager.Instance.AllReset();
    }

    public void StageSelectOpen()
    {
        SoundManagerV2.Instance.PlaySE(21);
        switch (SaveManager.Instance.save.stage) {
            case 1:
                _stage1.SetActive(true);
                break;
            case 2:
                _stage1.SetActive(true);
                _stage2.SetActive(true);
                break;
            case 3:
                _stage1.SetActive(true);
                _stage2.SetActive(true);
                _stage3.SetActive(true);
                break;
        }
        _startMenu.SetActive(false);
        _stageSelect.SetActive(true);
    }

    public void StartMenuOpen()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _startMenu.SetActive(true);
        _stageSelect.SetActive(false);
        _option.SetActive(false);
        _quit.SetActive(false);
    }

    public void OptionOpen()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _startMenu.SetActive(false);
        _option.SetActive(true);
    }

    public void QuitOpen()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _startMenu.SetActive(false);
        _quit.SetActive(true);
    }

    public void Stage1()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _animator.SetTrigger("Decision2");
        _stageSelect.SetActive(false);
        StartCoroutine(TransScene("Stage1"));
        SaveManager.Instance.IsNewGame = false;
        isDicide = true;
        ScoreManager.Instance.AllReset();
        StartCoroutine(PlayBGM(1));
    }

    public void Stage2()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _animator.SetTrigger("Decision2");
        _stageSelect.SetActive(false);
        StartCoroutine(TransScene("Stage2"));
        SaveManager.Instance.IsNewGame = false;
        isDicide = true;
        ScoreManager.Instance.AllReset();
        StartCoroutine(PlayBGM(0));
    }

    public void Stage3()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _animator.SetTrigger("Decision2");
        _stageSelect.SetActive(false);
        StartCoroutine(TransScene("Stage3"));
        SaveManager.Instance.IsNewGame = false;
        isDicide = true;
        ScoreManager.Instance.AllReset();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
              UnityEngine.Application.Quit();
#endif
    }

    private IEnumerator PlayBGM(int i)
    {
        yield return new WaitForSeconds(2.1f);
        SoundManagerV2.Instance.PlayBGM(i);
    }

    private IEnumerator TransScene(string sceneName)
    {
        yield return new WaitForSeconds(3f);
        FadeManager.Instance.LoadSceneNormalTrans(sceneName, 1.5f);
    }
}