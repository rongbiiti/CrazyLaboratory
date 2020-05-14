using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockPivot : MonoBehaviour
{
    private bool isEnterAcid;   // 酸弾に当たったか
    private bool isPlayerStay;  // 復活時プレイヤーが重なっているか
    [SerializeField, CustomLabel("消滅時エフェクト")] private GameObject _dstEffect;    // 消滅時エフェクト
    [SerializeField, CustomLabel("消えるまでの時間")] private float destroyTime = 2f;   // 消えるまでの時間
    [SerializeField, CustomLabel("元に戻るまでの時間")] private float restorTime = 5f;   // 元に戻るまでの時間
    private Vector2 startScale; // 初期の大きさ
    private Vector2 Startposition;  // 初期の位置
    private Vector2 Startzikkenposition;    // スタート実験ポジション（直訳）
    private GameObject parent1; // 親1
    private SpriteRenderer spriteRenderer;  // スプライトレンダラーコンポーネント
    private Color changeColorDst;   // 消滅時にだんだん黒くするための変数
    private Color changeColorRst;   // 復活時にだんだん白くするための変数
    private enum Status
    {
        Normal,
        Restoring
    }
    private Status status;  // 通常時か、復活時か。

    void Start()
    {
        parent1 = transform.parent.gameObject;                      //親のオブジェクト格納用
        startScale = parent1.transform.localScale;                  //親の初期サイズ格納
        Startposition = parent1.transform.position;                 //親の初期ポジション格納
        Startzikkenposition = gameObject.transform.position;        //子の初期ポジション
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        changeColorDst = new Color(spriteRenderer.color.r / destroyTime * Time.deltaTime, spriteRenderer.color.g / destroyTime * Time.deltaTime, spriteRenderer.color.b / destroyTime * Time.deltaTime, 0);
        changeColorRst = new Color(spriteRenderer.color.r / restorTime * Time.deltaTime, spriteRenderer.color.g / restorTime * Time.deltaTime, spriteRenderer.color.b / restorTime * Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {
        if (status == Status.Normal) {
            if (isEnterAcid) {
                parent1.transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime); ;
                spriteRenderer.color -= changeColorDst;
                if (parent1.transform.localScale.x <= 0) {
                    SetRestoring();
                }
            }
        } else if(status == Status.Restoring) {
            if (!isPlayerStay) {
                parent1.transform.localScale += new Vector3(startScale.x / restorTime * Time.deltaTime, startScale.y / restorTime * Time.deltaTime); ;
                spriteRenderer.color += changeColorRst;
                if (startScale.x <= parent1.transform.localScale.x) {
                    parent1.transform.position = Startposition;
                    parent1.transform.localScale = startScale;
                    gameObject.transform.position = Startzikkenposition;
                    UnSetRestoring();
                }
            }
        }
    }

    private void SetRestoring()
    {
        isEnterAcid = false;
        spriteRenderer.color -= new Color(0, 0, 0, 0.6f);
        status = Status.Restoring;
    }

    private void UnSetRestoring()
    {
        status = Status.Normal;
        spriteRenderer.color += new Color(0, 0, 0, 0.6f);
        GetComponent<BoxCollider2D>().isTrigger = false;
    }

    private void DisableCollider()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Acid")) {
            isEnterAcid = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            isPlayerStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            isPlayerStay = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnterAcid == true)
            return;

        if (collision.gameObject.CompareTag("AcidFlask") && status == Status.Normal) {

            // transformを取得 親
            Transform myTransform = parent1.transform;

            // ワールド座標を基準に、座標を取得
            Vector3 worldPos = myTransform.position;


            //当たった場所を比較し　ピボット（親のオブジェクト）の位置を移動させる　X座標
            //Y座標の３分の一に分けて位置を指定
            var ThisX = spriteRenderer.size.x * gameObject.transform.localScale.x / 2 / 2;
            if (transform.position.x + ThisX <= collision.transform.position.x) {
                worldPos.x = transform.position.x - spriteRenderer.size.x * transform.localScale.x / 2.0f;
            } else if (gameObject.transform.position.x - ThisX >= collision.transform.position.x) {

                worldPos.x = transform.position.x + spriteRenderer.size.x * transform.localScale.x / 2.0f;
            }

            //Y座標の３分の一に分けて位置を指定
            var ThisY = spriteRenderer.size.y * gameObject.transform.localScale.y / 2 / 2;
            //Y座標
            if (gameObject.transform.position.y + ThisY <= collision.transform.position.y) {
                worldPos.y = transform.position.y - spriteRenderer.size.y * gameObject.transform.localScale.y / 2.0f;
            } else if (gameObject.transform.position.y - ThisY >= collision.transform.position.y) {
                worldPos.y = transform.position.y + spriteRenderer.size.y * gameObject.transform.localScale.y / 2.0f;
            }
            worldPos.z = 0.0f;
            myTransform.position = worldPos;  // ワールド座標での座標を設定

            //このスクリプトを持っているobject
            myTransform = gameObject.transform;

            myTransform.position = Startposition;  // ワールド座標での座標を設定

            isEnterAcid = true;
            DisableCollider();
            SoundManagerV2.Instance.PlaySE(1);
            GameObject efcObj;  // 消滅エフェクトオブジェクトを自身に追跡させるのに使う変数
            efcObj = Instantiate(_dstEffect, transform.position, _dstEffect.transform.rotation);
            efcObj.transform.parent = transform;
            efcObj.transform.localScale = Vector3.one;
        }
    }

    public void SetisEnterAcid()
    {
        isEnterAcid = true;
        SoundManagerV2.Instance.PlaySE(1);
    }

}
