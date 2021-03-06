﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaker : MonoBehaviour
{

	private GameObject inAcid;
	private Vector3 inAcidStartScale;   // 初期の容器内の酸の大きさ
	private Vector3 increaseScale;      // 弾1発でどの程度容器内の酸のかさが増えるか
    private Vector3 targetScale;        // かさが増えるアニメーション終了するまでの目盛り
    private Vector3 timeIncreaseScale;  // 毎フレーム少しずつ増やす用
	private Explodable explodable;
	private List<float> SpreadAngle = new List<float>();
	private ObjectPool pool;

	[SerializeField, CustomLabel("酸が溜まる量")]
	private int _acidCollectMax = 4;

	[SerializeField, CustomLabel("飛び散る酸の数")]
	private int _shotBulletCount = 8;

	[SerializeField, CustomLabel("拡散範囲")] private float _spreadRange = 90f;
	[SerializeField, CustomLabel("砲口初速")] private float _muzzleVelocity = 10f;
	[SerializeField, CustomLabel("弾の落下しやすさ")] private float _gravityScale = 6f;
	[SerializeField, CustomLabel("発射角度")] private float _fireAngle = 90f;
	
	private int intoAcidCount = 0;

	private void Awake()
	{
		explodable = GetComponent<Explodable>();
		inAcid = transform.GetChild(0).gameObject;
		inAcidStartScale = inAcid.transform.localScale;
		increaseScale = new Vector3(0, inAcidStartScale.y / _acidCollectMax, 0);
		inAcid.transform.localScale = new Vector3(1,0,1);
		PreCalcSpreadAngle();
	}

	void Start ()
	{
		pool = GameObject.FindWithTag("Player").GetComponent<ObjectPool>();
	}

	private void FixedUpdate()
	{
		if(inAcid.transform.localScale.y <= targetScale.y) {
            inAcid.transform.localScale += increaseScale * Time.deltaTime;
            if(inAcidStartScale.y <= inAcid.transform.localScale.y && _acidCollectMax <= intoAcidCount) {
                var rb = GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                SoundManagerV2.Instance.PlaySE(42);
            }
        }
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Enemy") ||
		    other.gameObject.CompareTag("BreakBlock"))
		{
			AcidSpread();
			explodable.explode();
			var ef = FindObjectOfType<ExplosionForce>();
			ef.doExplosion(transform.position);
            SoundManagerV2.Instance.PlaySE(43);
            SoundManagerV2.Instance.PlaySE(44);
        }
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("AcidFlask") && intoAcidCount < _acidCollectMax)
		{
			++intoAcidCount;
            targetScale += increaseScale;
            SoundManagerV2.Instance.PlaySE(41);
		}
	}
	
	private void PreCalcSpreadAngle()
	{
		var range = _spreadRange / _shotBulletCount;
		// 一番上から一番下まで順に計算する。
		// 一番上は拡散範囲÷2
		SpreadAngle.Add(_spreadRange / 2);
		int i;

		// 2番目から真ん中まではループで求める
		for(i = 1; i < _shotBulletCount / 2; i++) {
			SpreadAngle.Add(SpreadAngle[i-1] - range);
		}

		// 下半分の最初は拡散範囲÷同時発射数×2した数にする
		SpreadAngle.Add(SpreadAngle[i-1] - range * 2);
		i++;

		// 残りの一番下まではループで
		for(; i < _shotBulletCount; i++) {
			SpreadAngle.Add(SpreadAngle[i-1] - range);
		}
        
	}

	private void AcidSpread()
	{
		for (var i = 0; i < _shotBulletCount; i++)
		{
			var bullet = pool.GetObject();
			if (bullet != null)
			{
				bullet.GetComponent<AcidFlask>().Init(transform.position, false);
			}

			var right = bullet.transform.right;

			var bRb = bullet.GetComponent<Rigidbody2D>();

			float rad = 0;

			rad = (_fireAngle + SpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
			bRb.gravityScale = _gravityScale; //上へ発射時の弾の重量を代入

			//rad(ラジアン角)から発射用ベクトルを作成
			var addforceX = Math.Cos(rad * 1);
			var addforceY = Math.Sin(rad * 1);
			var shotAngle = new Vector3((float) addforceX, (float) addforceY, 0);

			//　発射角度にオブジェクトを回転させる、進んでいる方向と角度を一致させる
			var axis = Vector3.Cross(right, shotAngle);
			var angle = Vector3.Angle(right, shotAngle) * (axis.z < 0 ? -1 : 1);
			bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			//　で、でますよ
			bRb.AddForce(shotAngle * _muzzleVelocity, ForceMode2D.Force);
		}
	}
}
