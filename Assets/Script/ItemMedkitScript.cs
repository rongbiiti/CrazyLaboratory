using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMedkitScript : MonoBehaviour {

    [SerializeField, Range(0, 100)] private float _healPercent = 20f;

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            collision.GetComponent<PlayerController>().Heal(_healPercent);
            SoundManagerV2.Instance.PlaySE(11);
            Destroy(gameObject);
        }
    }
}
