using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kama : MonoBehaviour {

    [SerializeField] private float MoveSpeed;     //エネミーのスピード
    private byte AttackPhase;
    private float Count;
    Rigidbody2D rb;

    // Use this for initialization
    void Start () {
        AttackPhase = 0;
        Count = 0;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        // transformを取得
        Transform myTransform = this.transform;
        if (AttackPhase == 0)
        {
            // 現在の座標からのxyz を1ずつ加算して移動
            myTransform.Translate(MoveSpeed, 0.0f, 0.0f, Space.World);
        }
        else if(AttackPhase == 1)
        {
            // 現在の座標からのxyz を1ずつ加算して移動
            myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);
            Count += Time.deltaTime;
            if(Count >= 3)
            {
                AttackPhase = 2;
                Count = 0;
               
            }
        }
        else if(AttackPhase == 2)
        {
            myTransform.Translate(0.2f * gameObject.transform.localScale.x * -1, 0.0f, 0.0f, Space.World);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall" && AttackPhase != 1)
        {
            MoveSpeed *= -1;
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            if(AttackPhase == 2)
            {
                AttackPhase = 0;
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && AttackPhase == 0)
        {
            AttackPhase = 1;
            Count = 0;
        }
    }
}
