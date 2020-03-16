using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
