using UnityEngine;

public class GotoEndingScene : MonoBehaviour {
	private void Update () {
		if (Input.GetButtonDown("Submit"))
		{
			FadeManager.Instance.LoadScene("EndingScene",1f);
		}
	}
}
