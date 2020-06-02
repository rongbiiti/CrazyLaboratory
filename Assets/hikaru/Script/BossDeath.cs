using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour {

    [SerializeField] private float _fadeRate;    //フェード開ける時間　前のシーン参照
    private float FadeTime;    //fadeRate格納用
    [SerializeField] private float _deathRate;   //死ぬ時間　カメラの処理などで使う
    private float DeathTime;    //死ぬ時間の格納用
    Animator animator;  //アニメーション用

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.SetBool("Stand1", false);
        animator.SetBool("Stand2", false);
        animator.SetBool("BodyPress", false);
        animator.SetBool("Atack", false);
        animator.SetBool("Jump", false);
        animator.SetBool("Stun", false);
        animator.SetBool("Death", false);
    }
	
	// Update is called once per frame
	void Update () {
		
        if(0 < FadeTime)
        {
            FadeTime -= Time.deltaTime;
            if(FadeTime <= 0)
            {
                DeathTime = _deathRate;
                animator.SetBool("Death", true);
            }
        }

        if(0 < DeathTime)
        {
            DeathTime -= Time.deltaTime;
        }

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            FadeTime = _fadeRate;

        }
        return;
    }

}
