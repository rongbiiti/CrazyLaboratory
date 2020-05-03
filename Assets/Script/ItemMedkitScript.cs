using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// HP回復アイテム。触れるとプレイヤーの回復関数を起動して、
/// 引数にこのオブジェクトに設定された回復量を入れる。
/// </summary>
public class ItemMedkitScript : MonoBehaviour {

    [SerializeField, Range(0, 100), CustomLabel("回復量(%)")] private float _healPercent = 20f;

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            collision.GetComponent<PlayerController>().Heal(_healPercent);
            ScoreManager.Instance.MedGetCnt++;
            ScoreManager.Instance.TotalMedGetCnt++;
            SoundManagerV2.Instance.PlaySE(20);
            Destroy(gameObject);
        }
    }
}
