using UnityEngine;

public class Stage2RestartPoint : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            ScoreManager.Instance.IsStage2RestartPointReached = true;
            collision.GetComponent<PlayerController>().Stage2RestartValueSave();
            Destroy(gameObject);
        }
    }
}