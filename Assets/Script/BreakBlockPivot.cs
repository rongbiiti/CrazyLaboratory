using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockPivot : MonoBehaviour
{

    private bool isEnterAcid;
    [SerializeField] private float destroyTime = 2f;
    private Vector2 startScale;
    private Vector2 Startposition;
    private GameObject parent1;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        parent1 = transform.parent.gameObject;
        startScale = parent1.transform.localScale;
        Startposition = gameObject.transform.position;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isEnterAcid) {
            //transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            parent1.transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            if (parent1.transform.localScale.x <= 0) {
                Destroy(parent1);
                Destroy(gameObject);

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Acid")) {
            isEnterAcid = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnterAcid == true)
            return;

        if (collision.gameObject.CompareTag("AcidFlask")) {

            // transformを取得 親
            Transform myTransform = parent1.transform;

            // ワールド座標を基準に、座標を取得
            Vector3 worldPos = myTransform.position;


            //当たった場所を比較し　ピボット（親のオブジェクト）の位置を移動させる　X座標
            //Y座標の３分の一に分けて位置を指定
            var ThisX = spriteRenderer.size.x * gameObject.transform.localScale.x / 2 / 2;
            Debug.Log(ThisX);
            if (transform.position.x + ThisX <= collision.transform.position.x) {
                Debug.Log("+");
                Debug.Log(gameObject.transform.position.x + ThisX);
                Debug.Log(collision.transform.position.x);
                worldPos.x = transform.position.x - spriteRenderer.size.x * transform.localScale.x / 2.0f;
            } else if (gameObject.transform.position.x - ThisX >= collision.transform.position.x) {
                Debug.Log("-");
                Debug.Log(gameObject.transform.position.x - ThisX);
                Debug.Log(collision.transform.position.x);

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
            SoundManagerV2.Instance.PlaySE(1);
        }
    }

    public void SetisEnterAcid()
    {
        isEnterAcid = true;
        SoundManagerV2.Instance.PlaySE(1);
    }

}
