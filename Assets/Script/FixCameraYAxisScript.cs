using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 階層の分かれ目など、強制的にカメラのY軸を補正したいときに使う
/// トリガーにアタッチし、そのトリガーにプレイヤーが触れるとカメラの
/// Y軸強制補正機能を作動させる
/// また、降りてまた上がったときにも正しく動作するように、降りたことがわかる
/// 対のトリガーも設定する。トリガーは触れられると自分をOFFにし、相方をONにする。
/// </summary>
public class FixCameraYAxisScript : MonoBehaviour {

    [SerializeField, CustomLabel("対のトリガー")] private GameObject _combiFixTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) {
            GameObject cam = GameObject.Find("Main Camera");
            cam.GetComponent<CameraController>().SetIsFloorChange();
            _combiFixTrigger.SetActive(true);
            gameObject.SetActive(false);
        }
    }

}