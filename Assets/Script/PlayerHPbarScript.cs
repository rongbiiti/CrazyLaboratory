using UnityEngine;
using UnityEngine.UI;

public class PlayerHPbarScript : MonoBehaviour
{

	[SerializeField, CustomLabel("赤のバー")] private Slider _damagebar;
	[SerializeField, CustomLabel("回復バー")] private Slider _healbar;
	private Slider mySlider;
	private float targetValue;
	private Color _dangerColor_super;
	private bool blinkingFlag;
	[SerializeField,CustomLabel("50%以下の色")] private Color _coutionColor;
	[SerializeField,CustomLabel("25%以下の色")] private Color _dangerColor;
	[SerializeField, CustomLabel("画面効果")] private Image _dangerEffect;
	float time = 0f;
	
	private void Start ()
	{
		mySlider = GetComponent<Slider>();
		_damagebar.maxValue = mySlider.maxValue;
		_damagebar.value = mySlider.value;
		_healbar.maxValue = mySlider.maxValue;
		_healbar.value = mySlider.value;
		targetValue = mySlider.value;
		_dangerEffect.color = Color.clear;	
	}

	private void FixedUpdate()
	{
		if (mySlider.value < 2500f)
		{
			_dangerEffect.color = new Color(1,1,1,Mathf.Sin(Time.time * 7) / 4 +  _dangerColor.a);
		}
		if (mySlider.value <= targetValue && targetValue < _damagebar.value)
		{
			_damagebar.value -= 30f;
			if (_damagebar.value <= targetValue)
			{
				_damagebar.value = mySlider.value;
				targetValue = mySlider.value;
				_healbar.value = mySlider.value;
			}
		} else if (mySlider.value < _healbar.value)
		{
			mySlider.value += 30f;
			targetValue = mySlider.value;
			if (_healbar.value <= mySlider.value)
			{
				mySlider.value = _healbar.value;
				_damagebar.value = _healbar.value;
				targetValue = _healbar.value;
			}
		}
	}

	public void ChangeValue()
	{
		DangerEffectChange();
		if (mySlider.value < targetValue)	// ダメージ
		{
			_damagebar.value = targetValue;
			_healbar.value = mySlider.value;
			targetValue = mySlider.value;
		} 
		else if (_healbar.value < mySlider.value) //	回復
		{
			_healbar.value = mySlider.value;
			mySlider.value = targetValue;
			_damagebar.value = targetValue;
		}
	}

	private void DangerEffectChange()
	{
		if (mySlider.value < 2500f)
		{
			return;
		}
		if (mySlider.value < 5000f)
		{
			_dangerEffect.color = _coutionColor;
		}
		else
		{
			_dangerEffect.color = Color.clear;
		}
	}
	
}
