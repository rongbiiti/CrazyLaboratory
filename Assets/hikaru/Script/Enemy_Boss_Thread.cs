using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_Thread : MonoBehaviour
{

    private enum e_ActivityType
    {
        Wait,
        Jump,
        Attack,
        BodyPress,
    }

    private bool isPlayerStay;
    private bool SmallSwitch;   //小さくするタイミングのフラグ    true:小さくする false:待機（なにもしない）
    private bool IntSwitch;     //初期化するタイミングのフラグ    true:初期化する false:待機 (なにもしない)
    private bool HpZero;        //HPが０か０じゃないか           true:0         false:0じゃない
    [SerializeField] float _hp;   //HP
    [SerializeField] private float _hitDamage = 1f; //一発当たるごとのダメージ
    [SerializeField] private float Boss_X;  //ボスのｘ座標を読み取り位置を調整する
    private float NowHP;          //HPの格納用
    
    [SerializeField, CustomLabel("消えるまでの時間")] private float destroyTime = 2f;
    private Vector2 startScale;
    private Vector2 Startposition;
    private Vector2 StartChildposition;
    private Vector2 StartChildScale;
    private GameObject parent1;
    private SpriteRenderer spriteRenderer;

    [SerializeField] GameObject BossGameObject;     //ボスのオブジェクト格納用
    private Enemy_Boss Boss;    //ボスのスクリプト格納

    void Start()
    {
        parent1 = transform.parent.gameObject;                      //親のオブジェクト格納用
        startScale = parent1.transform.localScale;                  //親の初期サイズ格納
        Startposition = parent1.transform.position;                 //親の初期ポジション格納
        StartChildposition = gameObject.transform.position;         //子(このスクリプトを持ってるオブジェクト)の初期ポジション
        StartChildScale = gameObject.transform.localScale;          //子(このスクリプトを持ってるオブジェクト)の初期サイズ
        NowHP = _hp;
        Boss = BossGameObject.GetComponent<Enemy_Boss>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SmallSwitch = false;
        IntSwitch = false;

    }

    private void FixedUpdate()
    {
        var x = BossGameObject.transform.position.x + Boss_X;
        var BossP = BossGameObject.transform.position;
        parent1.transform.position = new Vector3(x, BossP.y, BossP.z);

        if (SmallSwitch)
        {
            parent1.transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            if (parent1.transform.localScale.x <= 0)
            {
                SetSmallSwitch(false);
                parent1.SetActive(false);
                HpZero = true;
            }
            return;
        }

        if (IntSwitch)
        {
            parent1.transform.localScale = startScale;
            parent1.transform.position = Startposition;
            transform.position = StartChildposition;
            transform.localScale = StartChildScale;
            NowHP = _hp;
            HpZero = false;
            parent1.transform.position = BossGameObject.transform.position;
            parent1.SetActive(true);
            SetIntSwitch(false);    //初期化スイッチをfalseにする
            SetSmallSwitch(false);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SmallSwitch == true)
            return;

        if(collision.gameObject.CompareTag("AcidFlask") && NowHP > 0)
        {
            NowHP -= _hitDamage;
            Debug.Log(NowHP);
            if(NowHP <= 0)
            {
                SetSmallSwitch(true);
            }
        }

        if (NowHP <= 0 && SmallSwitch)
        {

            //// transformを取得 親
            //Transform myTransform = parent1.transform;

            //// ワールド座標を基準に、座標を取得
            //Vector3 worldPos = myTransform.position;


            ////当たった場所を比較し　ピボット（親のオブジェクト）の位置を移動させる　X座標
            ////Y座標の３分の一に分けて位置を指定
            //var ThisX = spriteRenderer.size.x * gameObject.transform.localScale.x / 2 / 2;
            //if (transform.position.x + ThisX <= collision.transform.position.x)
            //{
            //    worldPos.x = transform.position.x - spriteRenderer.size.x * transform.localScale.x / 2.0f;
            //}
            //else if (gameObject.transform.position.x - ThisX >= collision.transform.position.x)
            //{

            //    worldPos.x = transform.position.x + spriteRenderer.size.x * transform.localScale.x / 2.0f;
            //}

            ////Y座標の３分の一に分けて位置を指定
            //var ThisY = spriteRenderer.size.y * gameObject.transform.localScale.y / 2 / 2;
            ////Y座標
            //if (gameObject.transform.position.y + ThisY <= collision.transform.position.y)
            //{
            //    worldPos.y = transform.position.y - spriteRenderer.size.y * gameObject.transform.localScale.y / 2.0f;
            //}
            //else if (gameObject.transform.position.y - ThisY >= collision.transform.position.y)
            //{
            //    worldPos.y = transform.position.y + spriteRenderer.size.y * gameObject.transform.localScale.y / 2.0f;
            //}
            //worldPos.z = 0.0f;
            //myTransform.position = worldPos;  // ワールド座標での座標を設定

            ////このスクリプトを持っているobject
            //myTransform = gameObject.transform;

            //myTransform.position = Startposition;  // ワールド座標での座標を設定

            SoundManagerV2.Instance.PlaySE(1);
        }
    }

    public void SetSmallSwitch(bool set)
    {
        SmallSwitch = set;
    }

    public bool GetSmallSwitch()
    {
        return SmallSwitch;
    }

    public void SetIntSwitch(bool set)
    {
        IntSwitch = set;
    }

    public bool getHpZero()
    {
        return HpZero;
    }

    //public void SetisEnterAcid()
    //{
    //    SoundManagerV2.Instance.PlaySE(1);
    //}
}
