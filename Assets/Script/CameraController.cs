using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラが少し遅れてプレイヤーに追従するようにしている。
/// Y軸はキャラが画面上の一定の位置に来たときに追従している。
/// また、Y軸を強制的に補正させるトリガーに侵入したときは
/// その補正を追加で行っている。
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField, CustomLabel("チートON")] private bool _isCheatEnable = false;
    private GameObject player = null;
    private Camera cam;
    private Vector3 offset = Vector3.zero;
    private bool isFloarChange = false;
    private bool isFocasUnder = false;
    private float YAxisFixTime = 0f;
    private float setYAxisFixTime = 1f;
    private float focasOffset = 12f;

    private void Awake()
    {
        if (!_isCheatEnable)
        {
            Destroy(GetComponent<CheatMenu>());
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
        cam = GetComponent<Camera>();
        
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        Vector3 viewPos = cam.WorldToViewportPoint(player.transform.position);
        if (viewPos.y > 0.75f && !isFocasUnder) {
            newPosition.y = player.transform.position.y - offset.y;
        } else if (viewPos.y < 0.3f && !isFocasUnder) {
            newPosition.y = player.transform.position.y + offset.y;
        }
        newPosition.x = player.transform.position.x + offset.x;
        newPosition.z = player.transform.position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);
        if (isFloarChange) {
            FloarChange(newPosition);
        }

        if (isFocasUnder)
        {
            FocasUnder(newPosition);
        }
    }

    private void FloarChange(Vector3 newPosition)
    {
        YAxisFixTime -= Time.deltaTime;
        var position = player.transform.position;
        newPosition.x = position.x + offset.x;
        newPosition.y = position.y + offset.y;
        newPosition.z = position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 2.5f * Time.deltaTime);
        if(YAxisFixTime <= 0f) {
            isFloarChange = false;
        }
    }

    private void FocasUnder(Vector3 newPosition)
    {
        var position = player.transform.position;
        newPosition.x = position.x + offset.x;
        newPosition.y = position.y - focasOffset;
        newPosition.z = position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 2.5f * Time.deltaTime);
    }

    public void SetIsFloarChange()
    {
        if (!isFloarChange) {
            isFloarChange = true;
            YAxisFixTime = setYAxisFixTime;
        }
    }

    public void SetIsFocasUnder(bool flag)
    {
        isFocasUnder = flag;
    }
}