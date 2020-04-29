using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpbar : MonoBehaviour {

    [SerializeField, CustomLabel("敵のHPバー")] private Slider HPbar;
    [HideInInspector] public Slider hpbar;
    [SerializeField, CustomLabel("HPバーのY軸オフセット")] private float _YposOffset = 3.0f;  // Enemyの頭上にHPバーを表示するためのオフセットの値
    private Camera cam;
    private RectTransform rect;

    /// <summary>
    /// 最初に、シーンに内にあるカメラを探す。
    /// 名前がMain Cameraじゃないとエラーになるので注意
    /// </summary>
    private void Awake()
    {
        GameObject obj = GameObject.Find("Main Camera");
        cam = obj.GetComponent<Camera>();
        hpbar = Instantiate(HPbar) as Slider;
        GameObject canvas = GameObject.Find("Canvas");
        hpbar.transform.SetParent(canvas.transform, false);
        rect = hpbar.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Enemy.csのStartの中と、ダメージを受けたときにこれを呼ぶ。
    /// 最大HPと、現在のHPを引数に入れること
    /// </summary>
    /// <param name="maxHP">最大HP</param>
    /// <param name="nowHP">現在のHP</param>
    public void SetBarValue(float maxHP, float nowHP)
    {
        hpbar.maxValue = maxHP;
        hpbar.value = nowHP;
        rect.localScale = new Vector3((maxHP * 0.2f),1,1);
    }

    /// <summary>
    /// HPバーの表示位置を、Enemyの頭上にくるようにしている。
    /// </summary>
    private void FixedUpdate()
    {
        hpbar.transform.position = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + _YposOffset, transform.position.z));
    }

    //private void OnDisable()
    //{
    //    hpbar.enabled = false;
    //}

    private void OnEnable()
    {
        hpbar.enabled = true;
    }
}
