using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Beaker : MonoBehaviour
{

	private GameObject inAcid;
	private Vector3 inAcidStartScale;
	private Vector3 increaseScale;
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

	[SerializeField, CustomLabel("ランダム範囲最小値")]
	private float _randomMin = -10f;
	
	[SerializeField, CustomLabel("ランダム範囲最大値")]
	private float _randomMax = 10f;

	private int intoAcidCount = 0;

	private void Awake()
	{
		explodable = GetComponent<Explodable>();
		inAcid = transform.GetChild(0).gameObject;
		inAcidStartScale = inAcid.transform.localScale;
		increaseScale = new Vector3(0,inAcidStartScale.y / _acidCollectMax,0);
		inAcid.transform.localScale = new Vector3(1,0,1);
		PreCalcSpreadAngle();
	}

	void Start ()
	{
		pool = GameObject.FindWithTag("Player").GetComponent<ObjectPool>();
	}

	private void FixedUpdate()
	{
		
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
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("AcidFlask") && intoAcidCount < _acidCollectMax)
		{
			++intoAcidCount;
			inAcid.transform.localScale += increaseScale;
			if (_acidCollectMax <= intoAcidCount)
			{
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			}
		}
	}
	
	private void PreCalcSpreadAngle()
	{
		var range = _spreadRange / _shotBulletCount;
		// 一番上から一番下まで順に計算する。
		// 一番上は拡散範囲÷2
		SpreadAngle.Add(_spreadRange / 2 + Random.Range(_randomMin, _randomMax));
		int i;

		// 2番目から真ん中まではループで求める
		for(i = 1; i < _shotBulletCount / 2; i++) {
			SpreadAngle.Add(SpreadAngle[i-1] - range + Random.Range(_randomMin, _randomMax));
		}

		// 下半分の最初は拡散範囲÷同時発射数×2した数にする
		SpreadAngle.Add(SpreadAngle[i-1] - range * 2 + Random.Range(_randomMin, _randomMax));
		i++;

		// 残りの一番下まではループで
		for(; i < _shotBulletCount; i++) {
			SpreadAngle.Add(SpreadAngle[i-1] - range + Random.Range(_randomMin, _randomMax));
		}
        
	}

	private void AcidSpread()
	{
		for (var i = 0; i < _shotBulletCount; i++)
		{
			var bullet = pool.GetObject();
			if (bullet != null)
			{
				bullet.GetComponent<AcidFlask>().Init(transform.position);
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
