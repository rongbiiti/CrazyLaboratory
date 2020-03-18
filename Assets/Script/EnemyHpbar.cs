using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpbar : MonoBehaviour {

    [SerializeField, CustomLabel("敵のHPバー")] private Slider HPbar;
    [HideInInspector] public Slider hpbar;
    [SerializeField, CustomLabel("HPバーのY軸オフセット")] private float _YposOffset = 3.0f;  // Enemyの頭上にHPバーを表示するためのオフセットの値
    private Camera cam;

    /// <summary>
    /// 最初に、シーンに内にあるカメラを探す。
    /// 名前がMain Cameraじゃないとエラーになるので注意
    /// </summary>
    private void Start()
    {
        GameObject obj = GameObject.Find("Main Camera");
        cam = obj.GetComponent<Camera>();
        hpbar = Instantiate(HPbar);
        GameObject canvas = GameObject.Find("Canvas");
        hpbar.transform.SetParent(canvas.transform, false);
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
    }

    /// <summary>
    /// HPバーの表示位置を、Enemyの頭上にくるようにしている。
    /// </summary>
    private void FixedUpdate()
    {
        hpbar.transform.position = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + _YposOffset, transform.position.z));
    }
}
