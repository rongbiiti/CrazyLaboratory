using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// キー入力を管理している。シングルトンなので、シーンにひとつ置いておくこと。
/// </summary>
public class InputManager : MonoBehaviour
{
    protected static readonly string[] findTags =
    {
        "InputManager",
    };

    private static InputManager instance;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {

                Type type = typeof(InputManager);

                foreach (var tag in findTags)
                {
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

                    for (int j = 0; j < objs.Length; j++)
                    {
                        instance = (InputManager)objs[j].GetComponent(type);
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
            instance = (InputManager)this;
            DontDestroyOnLoad(gameObject);
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }

        Destroy(this);
        return false;
    }

    /* -- Horizontal入力 --------------------------------------------------------------------------- */
    private float moveKey = 0;
    public float MoveKey
    {
        get { return moveKey; }
    }

    /* -- Horizontal入力 --------------------------------------------------------------------------- */
    private float upMoveKey = 0;
    public float UpMoveKey
    {
        get { return upMoveKey; }
    }

    /* -- Jump入力 --------------------------------------------------------------------------------- */
    private int jumpKey = 0;
    public int JumpKey
    {
        get { return jumpKey; }
    }

    /* -- 攻撃入力 --------------------------------------------------------------------------------- */
    private int shotKey = 0;
    public int ShotKey
    {
        get { return shotKey; }
    }

    /* -- 停止入力 --------------------------------------------------------------------------------- */
    private int moveStopKey = 0;
    public int MoveStopKey
    {
        get { return moveStopKey; }
    }

    /* -- 方向パッド右入力 --------------------------------------------------------------------------------- */
    private bool equipHoleMaker = false;
    public bool EquipHoleMaker
    {
        get { return equipHoleMaker; }
    }

    /* -- 方向パッド下入力 --------------------------------------------------------------------------------- */
    private bool equipHandGun = false;
    public bool EquipHandGun
    {
        get { return equipHandGun; }
    }

    void Update()
    {
        // 移動
        moveKey = Input.GetAxisRaw("Horizontal");
        upMoveKey = Input.GetAxisRaw("Vertical");

        // 攻撃
        if (Input.GetButtonDown("LB")) {
            moveStopKey = 1;
        } else if (Input.GetButton("LB")) {
            moveStopKey = 2;
        } else if (Input.GetButtonUp("LB")) {
            moveStopKey = 0;
        }

        // 攻撃
        if (Input.GetButtonDown("Fire1")) {
            shotKey = 1;
        }
        else if(Input.GetButton("Fire1")) {
            shotKey = 2;
        }
        else if (Input.GetButtonUp("Fire1")) {
            shotKey = 0;
        }

        // ジャンプ
        if (Input.GetButtonDown("Jump"))
        {
            jumpKey = 1;
        }
        else if (Input.GetButton("Jump"))
        {
            jumpKey = 2;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpKey = 0;
        }

        // ホールメイカー装備
        if (0 < Input.GetAxisRaw("DpadHorizontal")) {
            equipHoleMaker = true;
        } else {
            equipHoleMaker = false;
        }

        // ホールメイカー装備
        if (Input.GetAxisRaw("DpadVertical") < 0) {
            equipHandGun = true;
        } else {
            equipHandGun = false;
        }
    }
}