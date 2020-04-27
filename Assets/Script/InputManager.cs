using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// キー入力を管理している。シングルトンなので、シーンにひとつ置いておくこと。
/// </summary>
public class InputManager : MonoBehaviour
{
    public enum ControlMode
    {
        TypeA = 0,
        TypeB
    }

    public ControlMode controlMode;
    
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

    /* -- ホールメイカー装備 --------------------------------------------------------------------------------- */
    private bool equipHoleMaker = false;
    public bool EquipHoleMaker
    {
        get { return equipHoleMaker; }
    }

    /* -- ハンドガン装備 --------------------------------------------------------------------------------- */
    private bool equipHandGun = false;
    public bool EquipHandGun
    {
        get { return equipHandGun; }
    }
    
    /* -- Cheat入力 --------------------------------------------------------------------------------- */
    private int cheatKey = 0;
    public int CheatKey
    
    {
        get { return cheatKey; }
    }
    
    /* -- L,Rトリガー入力 --------------------------------------------------------------------------------- */
    private float trigger = 0;
    public float Trigger
    
    {
        get { return trigger; }
    }
    
    /* -- 立ち止まる --------------------------------------------------------------------------------- */
    private int moonWalkKey = 0;
    public int MoonWalkKey
    
    {
        get { return moonWalkKey; }
    }
    
    /* -- 操作タイプ切り替え --------------------------------------------------------------------------------- */
    private int controlTypeChange = 0;
    public int ControlTypeChange
    
    {
        get { return controlTypeChange; }
    }
    
    /* -- ポーズメニュー開閉 --------------------------------------------------------------------------------- */
    private int pauseKey = 0;
    public int PauseKey

    {
        get { return pauseKey; }
    }
    
    private void Update()
    {
        switch (controlMode)
        {
            case ControlMode.TypeA:
                ModeA();
                break;
            case ControlMode.TypeB:
                ModeB();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void ModeA()
    {
        // 移動
        moveKey = Input.GetAxisRaw("Horizontal");
        upMoveKey = Input.GetAxisRaw("Vertical");
        
        // トリガー入力
        trigger = Input.GetAxis ("L_R_Trigger");

        // 方向を変えずに移動
        if (Input.GetButtonDown("LB")) {
            moonWalkKey = 1;
        } else if (Input.GetButton("LB")) {
            moonWalkKey = 2;
        } else if (Input.GetButtonUp("LB")) {
            moonWalkKey = 0;
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

        // チート
        if (Input.GetButtonDown("RStickButton"))
        {
            cheatKey = 1;
        } 
        else if (Input.GetButton("RStickButton"))
        {
            cheatKey = 2;
        } 
        else if (Input.GetButtonUp("RStickButton"))
        {
            cheatKey = 0;
        }
        

        // ホールメイカー装備
        equipHoleMaker = 0 < Input.GetAxisRaw("DpadHorizontal");

        // ホールメイカー装備
        equipHandGun = Input.GetAxisRaw("DpadVertical") < 0;

        // 操作タイプ切り替え
        if (Input.GetButtonDown("ControlTypeChange"))
        {
            controlTypeChange = 1;
            controlMode = ControlMode.TypeB;
        }
        else if (Input.GetButton("ControlTypeChange"))
        {
            controlTypeChange = 2;
        }
        else if (Input.GetButtonUp("ControlTypeChange"))
        {
            controlTypeChange = 0;
        }
        
        // 方向を変えずに移動
        if (Input.GetButtonDown("Pause")) {
            pauseKey = 1;
        } else if (Input.GetButton("Pause")) {
            pauseKey = 2;
        } else if (Input.GetButtonUp("Pause")) {
            pauseKey = 0;
        }
    }

    private void ModeB()
    {
        // 移動
        moveKey = Input.GetAxisRaw("Horizontal");
        upMoveKey = Input.GetAxisRaw("Vertical");
        
        // トリガー入力
        trigger = Input.GetAxis ("DpadVertical");

        // 方向を変えずに移動
        if (Input.GetButtonDown("LB")) {
            moonWalkKey = 1;
        } else if (Input.GetButton("LB")) {
            moonWalkKey = 2;
        } else if (Input.GetButtonUp("LB")) {
            moonWalkKey = 0;
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
        
        // 立ち止まる
        if (Input.GetButtonDown("Stop"))
        {
            moveStopKey = 1;
        }
        else if (Input.GetButton("Stop"))
        {
            moveStopKey = 2;
        }
        else if (Input.GetButtonUp("Stop"))
        {
            moveStopKey = 0;
        }

        // チート
        if (Input.GetButtonDown("RStickButton"))
        {
            cheatKey = 1;
        } 
        else if (Input.GetButton("RStickButton"))
        {
            cheatKey = 2;
        } 
        else if (Input.GetButtonUp("RStickButton"))
        {
            cheatKey = 0;
        }
        
        // 操作タイプ切り替え
        if (Input.GetButtonDown("ControlTypeChange"))
        {
            controlTypeChange = 1;
            controlMode = ControlMode.TypeA;
        }
        else if (Input.GetButton("ControlTypeChange"))
        {
            controlTypeChange = 2;
        }
        else if (Input.GetButtonUp("ControlTypeChange"))
        {
            controlTypeChange = 0;
        }
        
        // 方向を変えずに移動
        if (Input.GetButtonDown("Pause")) {
            pauseKey = 1;
        } else if (Input.GetButton("Pause")) {
            pauseKey = 2;
        } else if (Input.GetButtonUp("Pause")) {
            pauseKey = 0;
        }
    }
}