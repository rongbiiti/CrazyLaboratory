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
	[SerializeField, CustomLabel("無限ジャンプ")] private Text _superJumpText;

	private Image panel;
	private Text god;
	private Text nockback;
	private Text machine;
	private Text guide;
	private Text superJump;

	void Start ()
	{
        if (panel == null) {
            GameObject canvas = GameObject.Find("Canvas");
            _inputManager = InputManager.Instance;
            panel = Instantiate(_cheatPanel, canvas.transform, false);
            god = Instantiate(_godText, canvas.transform, false);
            nockback = Instantiate(_nockBackText, canvas.transform, false);
            machine = Instantiate(_machineGunText, canvas.transform, false);
            guide = Instantiate(_buttonGuide, canvas.transform, false);
            superJump = Instantiate(_superJumpText, canvas.transform, false);
        }
	}

    private void OnEnable()
    {
        if(panel == null) {
            GameObject canvas = GameObject.Find("Canvas");
            _inputManager = InputManager.Instance;
            panel = Instantiate(_cheatPanel, canvas.transform, false);
            god = Instantiate(_godText, canvas.transform, false);
            nockback = Instantiate(_nockBackText, canvas.transform, false);
            machine = Instantiate(_machineGunText, canvas.transform, false);
            guide = Instantiate(_buttonGuide, canvas.transform, false);
            superJump = Instantiate(_superJumpText, canvas.transform, false);
        }
       
        panel.enabled = true;
        guide.enabled = true;
    }

    private void OnDisable()
    {
        if (panel == null) {
            return;
        }
        panel.enabled = false;
        guide.enabled = false;
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

	public void SuperJumpTextOn(bool flag)
	{
		superJump.gameObject.SetActive(flag);
	}
	
	

	
	
	
	
}
