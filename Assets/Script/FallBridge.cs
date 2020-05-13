using UnityEngine;

public class FallBridge : MonoBehaviour {
    private bool isPlaySE;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && !isPlaySE) {
            SoundManagerV2.Instance.PlaySE(38);
            isPlaySE = true;
        }
    }
}
