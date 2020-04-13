using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
	private InputManager _inputManager;

	[SerializeField, CustomLabel("チートパネル")] private Image _cheatPanel;
	[SerializeField, CustomLabel("無敵")] private Text _godText;
	[SerializeField, CustomLabel("ノックバック無効")] private Text _nockBackText;
	[SerializeField, CustomLabel("マシンガン")] private Text _machineGunText;
	[SerializeField, CustomLabel("ボタンガイド")] private Text _buttonGuide;

	private Image panel;
	private Text god;
	private Text nockback;
	private Text machine;
	private Text guide;

	void Start ()
	{
		GameObject canvas = GameObject.Find("Canvas");
		_inputManager = InputManager.Instance;
		panel = Instantiate(_cheatPanel, canvas.transform, false);
		god = Instantiate(_godText, canvas.transform, false);
		nockback = Instantiate(_nockBackText, canvas.transform, false);
		machine = Instantiate(_machineGunText, canvas.transform, false);
		guide = Instantiate(_buttonGuide, canvas.transform, false);
	}
	
	void Update ()
	{
		if (_inputManager.CheatKey == 1)
		{
			panel.gameObject.SetActive(!panel.gameObject.activeSelf);
			guide.gameObject.SetActive(!guide.gameObject.activeSelf);
		}
	}

	public void GodTextOn(bool flag)
	{
		god.gameObject.SetActive(flag);
	}
	
	public void NockBackTextOn(bool flag)
	{
		nockback.gameObject.SetActive(flag);
	}
	
	public void machineTextOn(bool flag)
	{
		machine.gameObject.SetActive(flag);
	}
	
	

	
	
	
	
}
