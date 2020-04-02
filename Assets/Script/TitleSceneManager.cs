using UnityEngine;

public class TitleSceneManager : MonoBehaviour {

    private bool isAnyKeyPress;

    void Update () {
		if(Input.anyKey && !isAnyKeyPress) {
            FadeManager.Instance.LoadScene("zikken3_Stage1", 2f);
            SoundManagerV2.Instance.PlaySE(16);
            isAnyKeyPress = true;
        }
	}
}