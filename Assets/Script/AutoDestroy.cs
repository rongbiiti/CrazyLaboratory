using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自動で消えるためのクラス
/// AddComponentするときに.timeの中に消えるまでの秒数を書くこと。
/// </summary>

public class AutoDestroy : MonoBehaviour {

    public float time;

	void Start () {
        StartCoroutine("TimeDestroy");
	}
	
    IEnumerator TimeDestroy()
    {
        yield return new WaitForSeconds(time);
        gameObject.AddComponent<BreakBlock>().SetisEnterAcid();
    }
}
