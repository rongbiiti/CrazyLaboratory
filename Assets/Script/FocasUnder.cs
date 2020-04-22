using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocasUnder : MonoBehaviour
{
    [Header("12でキャラの頭が画面の上端につくくらいになります")]
    [Header("マイナスにすると逆に上を注視するようになります")]
    [SerializeField, CustomLabel("カメラどのくらい下げるか")] private float _offset = 12f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var cam = GameObject.Find("Main Camera");
        cam.GetComponent<CameraController>().SetIsFocusUnder(true, _offset);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var cam = GameObject.Find("Main Camera");
        cam.GetComponent<CameraController>().SetIsFocusUnder(false);
    }
}
