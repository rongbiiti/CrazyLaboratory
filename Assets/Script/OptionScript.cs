using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour
{

	[SerializeField, CustomLabel("BGMスライダー")]
	private Slider _BGMSlider;
	[SerializeField, CustomLabel("SEスライダー")]
	private Slider _SESlider;

	private void Start ()
	{
		_BGMSlider.value = SoundManagerV2.Instance.volume.BGM;
		_SESlider.value = SoundManagerV2.Instance.volume.SE;
	}

	private void OnDisable()
	{
		SaveManager.Instance.OptionDataSave();
	}

	public void ChangeBGMVolume(Slider slider)
	{
		SoundManagerV2.Instance.volume.BGM = slider.value;
	}
	
	public void ChangeSEVolume(Slider slider)
	{
		SoundManagerV2.Instance.volume.SE = slider.value;
	}
}
