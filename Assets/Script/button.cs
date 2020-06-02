using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// シーンリロードボタンと終了ボタン。
/// 終了ボタンは、エディターでプレイしてるときとビルドでプレイしているときとを
/// 検知している
/// </summary>
public class button : MonoBehaviour {
    
    private PlayerController _playerController;
    private PostProcessLayer _postProcess;
    private CheatMenu _cheatMenu;
    
    void Start ()
    {
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _postProcess = GameObject.FindWithTag("MainCamera").GetComponent<PostProcessLayer>();
        _cheatMenu = GameObject.FindWithTag("MainCamera").GetComponent<CheatMenu>();
    }

	public void SceneReload()
    {
        // 現在のScene名を取得する
        Scene loadScene = SceneManager.GetActiveScene();
        // Sceneの読み直し
        SceneManager.LoadScene(loadScene.name);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
              UnityEngine.Application.Quit();
        #endif
    }
    
    public void BackToTitle()
    {
        FadeManager.Instance.LoadSceneNormalTrans("TitleScene",0.5f);
        ScoreManager.Instance.IsStage2RestartPointReached = false;
    }

    public void Stage1Load()
    {
        FadeManager.Instance.LoadSceneNormalTrans("Stage1",0.5f);
        ScoreManager.Instance.IsStage2RestartPointReached = false;
    }

    public void Stage2Load()
    {
        FadeManager.Instance.LoadSceneNormalTrans("Stage2",0.5f);
        ScoreManager.Instance.IsStage2RestartPointReached = false;
    }
    
    public void Stage3Load()
    {
        FadeManager.Instance.LoadSceneNormalTrans("Stage3",0.5f);
        ScoreManager.Instance.IsStage2RestartPointReached = false;
    }
    
    public void GotoEndingScene()
    {
        FadeManager.Instance.LoadSceneNormalTrans("EndingScene",0.5f);
        ScoreManager.Instance.IsStage2RestartPointReached = false;
    }

    public void HPMAX()
    {
        _playerController.DebugHP(9999f);
    }

    public void GodON()
    {
        _playerController.IsGodMode = true;
        _cheatMenu.GodTextOn(true);
    }

    public void GodOFF()
    {
        _playerController.IsGodMode = false;
        _cheatMenu.GodTextOn(false);
    }

    public void NockBackON()
    {
        _playerController.IsNotNockBack = false;
        _cheatMenu.NockBackTextOn(false);
    }

    public void NockBackOFF()
    {
        _playerController.IsNotNockBack = true;
        _cheatMenu.NockBackTextOn(true);
    }

    public void HP1()
    {
        _playerController.DebugHP(1);
    }

    public void Suicide()
    {
        _playerController.Damage(9999f);
    }

    public void GetGun()
    {
        _playerController.GetGun();
    }

    public void MachineGunON()
    {
        _playerController.IsMachinGun = true;
        _cheatMenu.machineTextOn(true);
    }

    public void MachineGunOFF()
    {
        _playerController.IsMachinGun = false;
        _cheatMenu.machineTextOn(false);
    }

    public void PostProcessON()
    {
        _postProcess.enabled = true;
    }

    public void PostProcessOFF()
    {
        _postProcess.enabled = false;
    }

    public void SuperJumpON()
    {
        _playerController.IsSuperJump = true;
        _cheatMenu.SuperJumpTextOn(true);
    }
    
    public void SuperJumpOFF()
    {
        _playerController.IsSuperJump = false;
        _cheatMenu.SuperJumpTextOn(false);
    }

    public void GhostOn()
    {
        _playerController.ChangeGhost(true);
    }

    public void GhostOff()
    {
        _playerController.ChangeGhost(false);
    }
}
