using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shutter : MonoBehaviour {

    [SerializeField, CustomLabel("閉まる速さ")] private float _closeSpeed = 0.2f;
    [SerializeField, CustomLabel("閉まった状態のY座標")] private float _closeYPosition = 13.51f;
    [SerializeField, CustomLabel("カメラ揺らすか")] private bool _isCameraShake;
    private CameraShake cameraShake;

    private void Start()
    {
        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    public void CloseShutter()
    {
        StartCoroutine("DoCloseShutter");
    }

    private IEnumerator DoCloseShutter()
    {
        if (_isCameraShake) SoundManagerV2.Instance.PlaySE(47);
        while (transform.position.y >= _closeYPosition) {
            transform.Translate(new Vector3(0, -_closeSpeed, 0));
            yield return 0;
        }
        if(_isCameraShake) cameraShake.Shake(0.2f, 0.18f);
    }
}
