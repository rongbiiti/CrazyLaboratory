using System;
using UnityEngine;
using UnityEngineInternal;

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
    
    void Update () {
		if(Input.anyKey && !isAnyKeyPress) {
            _animator.SetTrigger("Decide");
            SoundManagerV2.Instance.PlaySE(21);
            _plsAnyKey.SetActive(false);
            StartMenuOpen();
            isAnyKeyPress = true;
        } else if (InputManager.Instance.JumpKey == 1 && isAnyKeyPress && !_startMenu.activeSelf && !isDicide)
        {
            StartMenuOpen();
        }
	}

    public void NewGame()
    {
        FadeManager.Instance.LoadScene("Stage1", 2f);
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
        switch (SaveManager.Instance.save.stage)
        {
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
        _stageSelect.SetActive(false);
        FadeManager.Instance.LoadScene("Stage1", 2f);
        SaveManager.Instance.IsNewGame = false;
        isDicide = true;
        ScoreManager.Instance.AllReset();
    }
    
    public void Stage2()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _stageSelect.SetActive(false);
        FadeManager.Instance.LoadScene("Stage2", 2f);
        SaveManager.Instance.IsNewGame = false;
        isDicide = true;
        ScoreManager.Instance.AllReset();
    }
    
    public void Stage3()
    {
        SoundManagerV2.Instance.PlaySE(21);
        _stageSelect.SetActive(false);
        FadeManager.Instance.LoadScene("Stage3", 2f);
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
}