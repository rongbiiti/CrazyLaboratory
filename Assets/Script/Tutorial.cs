using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

	[SerializeField, CustomLabel("表示時間")] private float _displayTime = 5f;
	[SerializeField, CustomLabel("表示ディレイ")] private float _delayTime = 1f;
	[SerializeField, CustomLabel("チュートリアル")] private GameObject _obj;
	[SerializeField, CustomLabel("一つ前のチュートリアル")] private Tutorial _preObj;
	private Animator anim;
	private GameObject obj;
	private bool isPlayerEnter;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) return;
		var col = GetComponent<BoxCollider2D>().enabled = false;
		var canvas = GameObject.Find("Canvas");
		StartCoroutine(DelayDisplay(canvas.transform));
	}

	private IEnumerator DelayDisplay(Transform canvas = null)
	{
		yield return new WaitForSeconds(_delayTime);
		if (_preObj != null)
		{
			_preObj.Cansel();
		}
		obj = Instantiate(_obj, canvas.transform);
		anim = obj.GetComponent<Animator>();
		yield return new WaitForSeconds(_displayTime);
		anim.SetTrigger("CloseTrigger");
		yield return new WaitForSeconds(1f);
		Destroy(obj);
		Destroy(gameObject);
	}

	public void Cansel()
	{
		StopCoroutine(DelayDisplay());
		Destroy(obj);
		Destroy(gameObject);
	}
}
