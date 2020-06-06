using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
	[SerializeField, CustomLabel("ステージクリア")] private GameObject _stageClear;
	[SerializeField, CustomLabel("リザルト")] private GameObject _result;
	
	[SerializeField, CustomLabel("クリア時間文字")] private GameObject _Text1;
	[SerializeField, CustomLabel("撃破数文字")] private GameObject _Text2;
	[SerializeField, CustomLabel("リトライ数文字")] private GameObject _Text3;
	[SerializeField, CustomLabel("スタン回数文字")] private GameObject _Text4;
	[SerializeField, CustomLabel("回復薬取得数文字")] private GameObject _Text5;
	
	[SerializeField, CustomLabel("クリア時間")] private GameObject _count1;
	[SerializeField, CustomLabel("撃破数")] private GameObject _count2;
	[SerializeField, CustomLabel("リトライ数")] private GameObject _count3;
	[SerializeField, CustomLabel("スタン回数")] private GameObject _count4;
	[SerializeField, CustomLabel("回復薬取得数")] private GameObject _count5;

	private Text _clearText1;
	private Text _countText1;
	private Text _countText2;
	private Text _countText3;
	private Text _countText4;
	private Text _countText5;

	private int status;

	private ScoreManager sm;
	private SoundManagerV2 sound;

	private void Start ()
	{
		sound = SoundManagerV2.Instance;
		sm = ScoreManager.Instance;
		sm.OverCheck();

		_clearText1 = _stageClear.GetComponent<Text>();
		_countText1 = _count1.GetComponent<Text>();
		_countText2 = _count2.GetComponent<Text>();
		_countText3 = _count3.GetComponent<Text>();
		_countText4 = _count4.GetComponent<Text>();
		_countText5 = _count5.GetComponent<Text>();

		_clearText1.text = sm.SceneName + "Clear!!";
		
		int minutes = Mathf.FloorToInt(sm.PlayTime / 60F);
		int seconds = Mathf.FloorToInt(sm.PlayTime - minutes * 60); 
		_countText1.text = string.Format("{0:00}:{1:00}", minutes, seconds);

		_countText2.text = sm.KillCnt.ToString();
		_countText3.text = sm.RetryCnt.ToString();
		_countText4.text = sm.StunCnt.ToString();
		_countText5.text = sm.MedGetCnt.ToString();

		StartCoroutine("ScoreDisplay");
	}

	private void Update()
	{

		switch (status)
		{
			case 0:
				if (Input.GetButtonDown("Submit"))
				{
					status = 1;
				}
				break;
			case 1:
				if (Input.GetButtonDown("Submit"))
				{
					status = 2;
					sound.PlaySE(21);
				} else if (Input.GetButtonDown("Jump"))
				{
					status = 3;
					sound.PlaySE(21);
				}
				break;
		}
		
	}

	private void FixedUpdate()
	{
		switch (status)
		{
			case 1:
				StopCoroutine("ScoreDisplay");
				TextAllOn();
				break;
			case 2:
				switch (sm.SceneName)
				{
					case "Stage 2 ":
						FadeManager.Instance.LoadSceneNormalTrans("Stage3", 1.5f);
                        ScoreManager.Instance.IsStage2RestartPointReached = false;
                        StartCoroutine(PlayBGM(2));
                        status = 99;
						break;
					case "Stage3":
						FadeManager.Instance.LoadSceneNormalTrans("EndingScene", 1.5f);
						status = 99;
						break;
					default:
						FadeManager.Instance.LoadSceneNormalTrans("Stage2", 1.5f);
                        StartCoroutine(PlayBGM(0));
						status = 99;
						break;
				}
				break;
			case 3:
				FadeManager.Instance.LoadSceneNormalTrans("TitleScene", 1.5f);
				status = 99;
				break;
		}
	}

	private IEnumerator ScoreDisplay()
	{
		sound.PlaySE(17);
		yield return new WaitForSeconds(0.1f);
		sound.PlaySE(18);
		yield return new WaitForSeconds(0.7f);
		sound.PlaySE(19);
		_count1.SetActive(true);
		_Text1.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		sound.PlaySE(19);
		_count2.SetActive(true);
		_Text2.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		sound.PlaySE(19);
		_count3.SetActive(true);
		_Text3.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		sound.PlaySE(19);
		_count4.SetActive(true);
		_Text4.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		sound.PlaySE(19);
		_count5.SetActive(true);
		_Text5.SetActive(true);
		status = 1;
	}

	private void TextAllOn()
	{
		_count1.SetActive(true);
		_Text1.SetActive(true);
		_count2.SetActive(true);
		_Text2.SetActive(true);
		_count3.SetActive(true);
		_Text3.SetActive(true);
		_count4.SetActive(true);
		_Text4.SetActive(true);
		_count5.SetActive(true);
		_Text5.SetActive(true);
	}

    private IEnumerator PlayBGM(int i)
    {
        yield return new WaitForSeconds(1.45f);
        SoundManagerV2.Instance.PlayBGM(i);
    }
}
