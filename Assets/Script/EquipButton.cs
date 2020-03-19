using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipButton : MonoBehaviour {

    private Image image;
    private Image gunImage;
    [SerializeField, CustomLabel("非表示までの時間")] private float _unShowTimeReset = 3f;
    [SerializeField, CustomLabel("フェードアウト時間")] private float _fadeOutTimeReset = 1f;
    private float unshowTime;
    private float fadeOutTime;

	void Start () {
        image = GetComponent<Image>();
        gunImage = transform.GetChild(0).GetComponent<Image>();
        gameObject.SetActive(false);
	}

    public void Show()
    {
        image.color = new Color(1, 1, 1, 1);
        gunImage.color = new Color(1, 1, 1, 1);
        unshowTime = _unShowTimeReset;
        fadeOutTime = _fadeOutTimeReset;
    }

    private void FixedUpdate()
    {
        if(0 < unshowTime) {
            unshowTime -= Time.deltaTime;
        } else {
            if(0 < fadeOutTime) {
                fadeOutTime -= Time.deltaTime;
                image.color -= new Color(0, 0, 0, image.color.a / fadeOutTime * Time.deltaTime);
                gunImage.color -= new Color(0, 0, 0, image.color.a / fadeOutTime * Time.deltaTime);
            }
        }
    }

}
