using UnityEngine;

public class FallBridge : MonoBehaviour {
    private bool isPlaySE;
    private CameraShake cameraShake;

    private void Start()
    {
        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && !isPlaySE) {
            SoundManagerV2.Instance.PlaySE(38);
            isPlaySE = true;
            cameraShake.Shake(0.35f, 0.3f);
        }
    }
}
