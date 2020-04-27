﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
	[SerializeField, CustomLabel("ポーズメニュー")] private GameObject _pauseMenu;
	[SerializeField, CustomLabel("リスタートパネル")] private GameObject _restartPanel;
	[SerializeField, CustomLabel("オプションパネル")] private GameObject _optionPanel;
	[SerializeField, CustomLabel("クイットパネル")] private GameObject _quitPanel;

	private InputManager im;
	private Image panel;
	private PlayerController pc;

	private void Start () {
		im = InputManager.Instance;
		panel = GetComponent<Image>();
		pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
	}

	private void Update () {
		// ポーズキーが押されたらポーズメニューを閉じる（非アクティブにする）
		// 閉じるときに、Pauserでポーズさせていたオブジェクト達のポーズを解除する
		// また、タイムスケールを1に戻してFixedUpdateや物理演算が動くようにする
		if (im.PauseKey == 1)
		{
			if (panel.IsActive())
			{
				ClosePausePanel();
			}
			else
			{
				OpenPausePanel();
			}
		}
		
	}

	public void OpenPausePanel()
	{
		_pauseMenu.SetActive(true);
		panel.enabled = true;
		Time.timeScale = 0;
		pc.enabled = false;
		Pauser.Pause();
	}

	public void ClosePausePanel()
	{
		_pauseMenu.SetActive(false);
		_restartPanel.SetActive(false);
		_optionPanel.SetActive(false);
		_quitPanel.SetActive(false);
		panel.enabled = false;
		Time.timeScale = 1;
		pc.enabled = true;
		Pauser.Resume();
	}

	public void OpenRestartPanel()
	{
		_pauseMenu.SetActive(false);
		_restartPanel.SetActive(true);
	}
	
	public void CloseRestartPanel()
	{
		_pauseMenu.SetActive(true);
		_restartPanel.SetActive(false);
	}
	
	public void OpenOptionPanel()
	{
		_pauseMenu.SetActive(false);
		_optionPanel.SetActive(true);
	}
	
	public void CloseOptionPanel()
	{
		_pauseMenu.SetActive(true);
		_optionPanel.SetActive(false);
	}
	
	public void OpenQuitPanel()
	{
		_pauseMenu.SetActive(false);
		_quitPanel.SetActive(true);
	}
	
	public void CloseQuitPanel()
	{
		_pauseMenu.SetActive(true);
		_quitPanel.SetActive(false);
	}

	public void Restart()
	{
		ClosePausePanel();
		FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 1f);
	}

	public void Quit()
	{
		#if UNITY_EDITOR
			ClosePausePanel();
			UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_STANDALONE
			ClosePausePanel();	
		    UnityEngine.Application.Quit();
		#endif
	}
}
