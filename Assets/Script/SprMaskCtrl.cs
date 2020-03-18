using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スプライトマスクを指定した時間だけONにするためのクラス
/// 基本的に弾側から秒数を設定させる。
/// </summary>
public class SprMaskCtrl : MonoBehaviour {

    private SpriteMask spriteMask;
    private float disableTime = 0f;

    private void Start()
    {
        spriteMask = GetComponent<SpriteMask>();
    }

    private void FixedUpdate()
    {
        if(0 < disableTime) {
            disableTime -= Time.deltaTime;
            if(disableTime <= 0) {
                spriteMask.enabled = false;
            }
        }
    }

    public void EnableSpriteMask(float enableTime)
    {
        spriteMask.enabled = true;
        if(disableTime < enableTime) {
            disableTime = enableTime;
        }
    }

}
