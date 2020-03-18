using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// プレイヤーの移動能力に関する各種設定ができるクラス
/// </summary>
public class PlayerManager : MonoBehaviour
{
    protected static readonly string[] findTags =
    {
        "PlayerManager",
    };

    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {

                Type type = typeof(PlayerManager);

                foreach (var tag in findTags)
                {
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

                    for (int j = 0; j < objs.Length; j++)
                    {
                        instance = (PlayerManager)objs[j].GetComponent(type);
                        if (instance != null)
                            return instance;
                    }
                }

                Debug.LogWarning(string.Format("{0} is not found", type.Name));
            }

            return instance;
        }
    }

    void Awake()
    {
        CheckInstance();
    }

    private bool CheckInstance()
    {
        if (instance == null)
        {
            instance = (PlayerManager)this;
            //DontDestroyOnLoad(gameObject);
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }

        Destroy(this);
        return false;
    }

    /* -- 移動速度(地上) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 20f), CustomLabel("地上移動速度")] private float moveSpeed = 10f;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    /* -- 移動速度(地上) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 50f), CustomLabel("滑りにくさ")] private float moveForceMultiplier = 10f;
    public float MoveForceMultiplier
    {
        get { return moveForceMultiplier; }
        set { moveForceMultiplier = value; }
    }

    /* -- 移動速度(空中) --------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 20f), CustomLabel("空中移動速度")] private float jumpMoveSpeed = 8f;
    public float JumpMoveSpeed
    {
        get { return jumpMoveSpeed; }
        set { jumpMoveSpeed = value; }
    }

    /* -- ジャンプ力 ------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 200f), CustomLabel("ジャンプ力")] private float jumpPower = 20f;
    public float JumpPower
    {
        get { return jumpPower; }
        set { jumpPower = value; }
    }

    /* -- ジャンプ時間 ------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 2f), CustomLabel("ジャンプ押し続け最大時間")] private float jumpTime = 0.35f;
    public float JumpTime
    {
        get { return jumpTime; }
        set { jumpTime = value; }
    }

    /* -- ジャンプ力減衰率 ------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 50f), CustomLabel("ジャンプ押し続け時減衰量")] private float jumpPowerAttenuation = 0.1f;
    public float JumpPowerAttenuation
    {
        get { return jumpPowerAttenuation; }
        set { jumpPowerAttenuation = value; }
    }

    /* -- 重力倍率 --------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 50f), CustomLabel("非ジャンプ時落下しやすさ")] private float gravityRate = 1.8f;
    public float GravityRate
    {
        get { return gravityRate; }
        set { gravityRate = value; }
    }

    /* -- 最大落下速度 --------------------------------------------------------------------------------- */
    [SerializeField, Range(0f, 50f), CustomLabel("最大落下速度")] private float maxFallSpeed = 10f;
    public float MaxFallSpeed
    {
        get { return maxFallSpeed; }
        set { maxFallSpeed = value; }
    }
}