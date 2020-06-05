using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public class Enemy_BossSpiderAnimTest : MonoBehaviour {

    private enum e_AttackType   //攻撃(attack)タイプの列挙体
    {
        fallmove,       //降下移動
        horizontalmove, //横移動
        BeforAttack,    //前攻撃
        Attack,         //攻撃
        AfterAttack,    //前攻撃
    }

    private enum e_ActivityType //ボスの行動タイプの列挙体
    {
        Stan,           //スタン
        Jump,           //ジャンプ
        Attack,         //攻撃
        CallSpider,     //子蜘蛛を呼ぶ
        BodyPress,      //ボディプレス
    }

    private enum e_StanType //スタン中の行動タイプ
    {
        Wait,           //待つ
        move,           //降下
        Stan,           //スタン
        afterStan,      //スタン後
    }
    
    [SerializeField] private float _HP = 12f;
    [SerializeField] private float _HitDamage = 1f;
    [SerializeField] private float _AttackHitDamage = 3000f;        //攻撃のダメージ
    [SerializeField] private float _BodyPressHitDamage = 5000f;     //ボディプレスのダメージ
    private float PlayerDamage;    //ダメージ格納用
    [SerializeField] private float _PlayerDamageRate = 3f;
    private float _PlayerDamageTime;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float moveSpeed;   //スピード格納用
    /*****************Anime********************/    //最初の演出用
    private bool AnimeSwitch;   //演出用に動くか TRUE:演出用  FALSE:ゲーム用
    private bool AnimeMove;     //動くタイミング用スイッチ
    [SerializeField] private float _aniFallSpeed;
    [SerializeField] private float _aniFallEndPosition; //降下終了時点
    [SerializeField] private Vector2 _aniPosition;  //演出の初期時点
    [SerializeField] private float _aniRoarRate;    //咆哮の時間
    private float AniRoarTime;  //咆哮時間格納用
    [SerializeField] private float _aniRoarswing;   //咆哮のカメラの揺れの強さ
    private bool AniRoarflag;   //叫んで一回のみ通るようにするフラグ
    /*******************Attack*********************/
    private byte AttackType;    //攻撃のタイプ 0:Wait 1:攻撃前befor 2:攻撃attack 3:攻撃後after
    [SerializeField] private int _rangeMin = 2;     //ランダムの最小値
    [SerializeField] private int _rangeMax = 4;     //ランダムの最大値
    private float AttackTime;           //攻撃の時間の格納用  
    [SerializeField] private float _playerY;    //playerのｙにプラス　降下時点の微調整用
    [SerializeField] private float _playerX;    //playerのｘにプラスかマイナスする　効果時点の微調整用
    [SerializeField] private float _fallSpeed = 30f;  //降下スピード
    [SerializeField] private float _ascentSpeed = 30f;    //上昇スピード
    [SerializeField] private float _horizontalSpeed = 20f;  //横の移動スピード
    private float IntAcceleration = 0f;       //加速度初期値
    private float Acceleration; //加速度の格納用
    [SerializeField] private float YenAround = 2f;   //一周するのにかかる時間
    private float DirectionX;   //方向    左なら1　右なら-1を格納する　移動の計算式に使う
    private float FallLocationY;    //降下時点のY座標
    private bool CenterIfflag;     //中央に向かってすぎる時の処理をするタイミング用
    [SerializeField] private float _bossRightMaxPx = -53.9f; //ボスの右側の移動範囲の上限
    [SerializeField] private float _bossLeftMaxPx = -103.97f; //ボスの左側の移動範囲の上限
    [SerializeField] private float _stageCenterPx = -79.85f;  //ステージの真ん中のｘ座標
    /*******************CallSpider*********************/
    [SerializeField] private float _callRate;   //呼ぶ時間
    private float CallTime;     //呼ぶ時間格納用
    [SerializeField] private Transform _eneSpawnPo1;   //エネミースポーン1
    [SerializeField] private Transform _eneSpawnPo2;   //エネミースポーン2
    /*******************BodyPress*********************/
    [SerializeField] private float _bodyPressRate = 3f;
    private float BodyPressTime;
    [SerializeField] private float _bodyPressFallSpeed = 50f;
    /*******************Stan***********************/
    private byte StanType;      //スタンのタイプ
    [SerializeField] private float _stanRate = 4f;  //スタンの時間
    [SerializeField] private float _afterStanRate = 2f; //スタン後の時間 //起き上がりのアニメーション用
    [SerializeField] private float _StanFallSpeed;  //スタン降下スピード
    private float StanTime;   //スタンの時間格納用
    /*******************Jump***********************/
    [SerializeField] private float _jumpRate = 3f;  //ジャンプまでの時間
    private float JumpTime;     //ジャンプまでの時間の格納用
    [SerializeField] private float _jumpSpeed = 30f;  //ジャンプスピード
    /*******************ActivityTime***********************/
    private byte ActivityType;     //行動のタイプ
    [SerializeField] e_ActivityType[] _activityTypeCount;
    private int ActivityCount;     //カウントによって行動を登録
    /*******************GameObject**********************/
    [SerializeField] private GameObject _switchObject;   //上昇して行動を切り替えさせるためのオブジェクト
    [SerializeField] private GameObject _bodyPressEndObject;    //ボディプレスを止めるためのオブジェクト
    [SerializeField] private GameObject _ThreadObject;  //糸のオブジェクト
    [SerializeField] private GameObject _animeObject;   //アニメ用オブジェクト当たり判定
    [SerializeField] private GameObject _bossSpiderFront;   //ボスの正面オブジェクト
    [SerializeField] private GameObject _ChildSpiderPrefab;   //子蜘蛛のオブジェクト　生成用
    /**********************************************/
    [SerializeField] private float _destroyTime = 2f;

    private bool SwitchFlag;    //スイッチオブジェクトに当たった時の処理をするかしないか
    private float nowHP;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    private Vector2 startScale;
    private Vector2 startposition;
    private Vector3 startPlayerPosition;
    private Vector2 StartAnimeObjectPo; //アニメオブジェクトのスタートポジション
    private Vector2 StartBossFrontPo;   //正面ボスのポジション
    private GameObject playerObject;  //playerのオブジェクトを格納
    private CameraShake cameraShake;
    Rigidbody2D rb;
    Rigidbody2D rdFront;
    Animator animator;
    Animator animetorFront;
    private CubismModel Model;
    private float anitime = 0f;
    Enemy_Boss_Thread Thread;   //ボスのクモの糸のスクリプト
    private bool Threadflag;    //蜘蛛の糸 false:横のクモの糸につく　true:正面のクモの糸につく
    [SerializeField] private Shutter _shutter1;
    [SerializeField] private Shutter _shutter2;

    [SerializeField, CustomLabel("回復薬投下ポイント1")] private Transform _medkitFallpoint1;
    [SerializeField, CustomLabel("回復薬投下ポイント2")] private Transform _medkitFallpoint2;
    [SerializeField, CustomLabel("回復薬プレハブ")] private GameObject _medkitPrefab;
    

    // Use this for initialization
    void Start()
    {
        /*transform*/
        startScale = transform.localScale;
        startposition = transform.position;
        StartAnimeObjectPo = _animeObject.transform.position;
        StartBossFrontPo = _bossSpiderFront.transform.position;

        /*HP*/
        nowHP = _HP;
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);

        /*GameObject*/
        playerObject = GameObject.FindGameObjectWithTag("Player");
        _switchObject.transform.parent = null;
        _bodyPressEndObject.transform.parent = null;
        _bodyPressEndObject.SetActive(false);
        _ThreadObject.transform.position = new Vector3(-75f, 77f, 0.0f);
        _ThreadObject.SetActive(false);
        //_animeObject.transform.parent = null;

        /**/
        var p = playerObject.transform.position;
        SwitchFlag = false;
        startPlayerPosition = p;
        ActivityCount = 0;
        StanType = (byte)e_StanType.Wait;
        //InitActivityType((byte)_activityTypeCount[ActivityCount]);
        rb = GetComponent<Rigidbody2D>();
        rdFront = _bossSpiderFront.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animetorFront = _bossSpiderFront.GetComponent<Animator>();
        Model = this.FindCubismModel();
        Thread = _ThreadObject.transform.GetChild(0).GetComponent<Enemy_Boss_Thread>();
        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        IntAnime();

    }

    private void OnEnable()
    {
        if (isZeroHP)
        {
            transform.position = startposition;
            transform.localScale = startScale;
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP, nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            ActivityCount = _activityTypeCount.Length - 1;  //ジャンプの数値を格納
            InitActivityType((byte)_activityTypeCount[ActivityCount]);
            JumpTime += _jumpRate;
            _switchObject.SetActive(true);
            _ThreadObject.SetActive(false);

            animator.SetBool("Stand1", false);
            animator.SetBool("Stand2", false);
            animator.SetBool("BodyPress", false);
            animator.SetBool("Atack", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Stun", false);
            animator.SetBool("Death", false);
            animetorFront.SetBool("Stand", false);
            animetorFront.SetBool("Roar", false);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (AnimeSwitch)
        {
            if (!AnimeMove) return;
            if(0 < AniRoarTime)
            {
                AniRoarTime -= Time.deltaTime;
                if(AniRoarTime <= 0)
                {
                    InitActivityType((byte)_activityTypeCount[ActivityCount]);
                    AnimeSwitch = false;
                    _bossSpiderFront.SetActive(false);
                    SoundManagerV2.Instance.PlayBGM(3);
                    return;
                }
            }
            Vector2 speed = new Vector2(0, 0); //横の移動の設定
            //if ( transform.position.y >= _aniFallEndPosition)
            if( _bossSpiderFront.transform.position.y >= _aniFallEndPosition )
            {
                speed = new Vector2(0, moveSpeed); //縦の移動の設定
                if(_bossSpiderFront.transform.position.y <= _aniFallEndPosition + 10f && !AniRoarflag)
                {
                    AniRoarflag = true;
                    animetorFront.SetBool("Stand", false);
                    animetorFront.SetBool("Roar", true);
                }
            }
            else if(AniRoarTime <= 0)
            {
                AniRoarTime = _aniRoarRate;
                cameraShake.Shake(_aniRoarRate, _aniRoarswing); //カメラの揺れ
                GameObject.Find("Main Camera").GetComponent<RadialBlurSc>().RadialBlur(_aniRoarRate, _aniRoarswing);
                SoundManagerV2.Instance.PlaySE(45);

            }
            var po = _bossSpiderFront.transform.position;
            _ThreadObject.transform.position = new Vector2(po.x, po.y + 17.0f);
            rdFront.velocity = speed;
            return;
        }

        if (isZeroHP)
        {
            if (0 < _destroyTime)
            {
                _destroyTime -= Time.deltaTime;
                FadeManager.Instance.LoadSceneNormalTrans("BossDeath", 0.5f);
            }
            else
            {
                //FadeManager.Instance.LoadSceneNormalTrans("", 0.3f);
                gameObject.SetActive(false);
                enemyHpbar.hpbar.gameObject.SetActive(false);

            }
        }
        else
        {
            //弱点の点滅タイマー
            if (++anitime < 11){
                Model.Parts[9].Opacity = 1;
                Model.Parts[10].Opacity = 0;
                Model.Parts[11].Opacity = 0;
                Model.Parts[12].Opacity = 0;
            }else if (anitime < 21){
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 1;
                Model.Parts[11].Opacity = 0;
                Model.Parts[12].Opacity = 0;
            }else if (anitime < 31){
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 0;
                Model.Parts[11].Opacity = 1;
                Model.Parts[12].Opacity = 0;
            }else if (anitime < 41){
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 0;
                Model.Parts[11].Opacity = 0;
                Model.Parts[12].Opacity = 1;
            }else if (anitime > 51) { anitime = 0f; }

            if (StanType == (byte)e_StanType.Wait && Thread.GetSmallSwitch())
            {

                InitActivityType((byte)e_ActivityType.Stan);
                AttackTime = 0.0f;
                _bodyPressEndObject.SetActive(true);
                //StanTime = _stanRate;
            }

            if (0 < StanTime)
            {
                StanTime -= Time.deltaTime;
                animator.SetBool("Stand1", false);
                animator.SetBool("Stand2", false);
                animator.SetBool("BodyPress", false);
                animator.SetBool("Atack", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Stun", true);
                animator.SetBool("Death", false);
                if (StanTime <= 0)
                {

                    switch (StanType)
                    {
                        case (byte)e_StanType.Stan:
                            StanType = (byte)e_StanType.afterStan;
                            StanTime = _afterStanRate;
                            break;

                        case (byte)e_StanType.afterStan:
                            InitActivityType((byte)e_ActivityType.Jump);
                            StanType = (byte)e_StanType.Wait;
                            ActivityCount = 0;    //攻撃の２番目に設定する
                            break;
                    }
                }
            }

            if (0 < _PlayerDamageTime)  //接触ダメージの時間間隔
            {
                _PlayerDamageTime -= Time.deltaTime;
            }
            if (0 < AttackTime && ActivityType == (byte)e_ActivityType.Attack)     //攻撃時間
            {
                AttackTime -= Time.deltaTime;
                animator.SetBool("Stand1", false);
                animator.SetBool("BodyPress", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Stun", false);
                animator.SetBool("Death", false);
                if (AttackTime <= 0)
                {
                    switch (AttackType)
                    {
                        case (byte)e_AttackType.fallmove:
                            animator.SetBool("Stand2", true);
                            animator.SetBool("Atack", false);
                            var BossP = transform.position;
                            //var PlayerP = playerObject.transform.position;
                            if (0.0f > playerObject.transform.localScale.x)
                            {
                                //BossP.x = PlayerP.x + _playerX;
                                if (_bossRightMaxPx <= BossP.x)
                                {
                                    BossP.x = _bossRightMaxPx;
                                }
                            }
                            else
                            {
                                //BossP.x = PlayerP.x - _playerX;
                                if (BossP.x <= _bossLeftMaxPx)
                                {
                                    BossP.x = _bossLeftMaxPx;
                                }

                                var ls = gameObject.transform.localScale;   //localscaleの格納
                                gameObject.transform.localScale = new Vector3(ls.x, -ls.y, ls.z);
                            }

                            transform.position = BossP;
                            break;
                    }
                }
            }

            if (0 < BodyPressTime)
            {
                BodyPressTime -= Time.deltaTime;
                if (BodyPressTime <= 0)
                {
                    var BossP = transform.position;
                    var PlayerP = playerObject.transform.position;

                    if (PlayerP.x < BossP.x)
                    {
                        animator.SetBool("Stand1", false);
                        animator.SetBool("Stand2", false);
                        animator.SetBool("BodyPress", true);
                        animator.SetBool("Atack", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stun", false);
                        animator.SetBool("Death", false);

                        var ls = gameObject.transform.localScale;   //localscaleの格納
                        gameObject.transform.localScale = new Vector3(-ls.x, ls.y, ls.z);
                    }

                    BossP.x = PlayerP.x;
                    transform.position = BossP;
                }
            }

            if (0 < JumpTime)       //ジャンプをするまでの時間
            {
                JumpTime -= Time.deltaTime;
                animator.SetBool("Stand1", true);
                animator.SetBool("Stand2", false);
                animator.SetBool("BodyPress", false);
                animator.SetBool("BeforeAtack", false);
                animator.SetBool("Atack", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Stun", false);
                animator.SetBool("Death", false);

                if (JumpTime <= 0)
                {
                    _switchObject.SetActive(true);
                    SwitchFlag = false;
                }
            }

            if(0 < CallTime)
            {
                CallTime -= Time.deltaTime;
                if(CallTime <= 0)
                {
                    _switchObject.SetActive(true);
                    SwitchFlag = false;
                    Instantiate(_ChildSpiderPrefab, _eneSpawnPo1.position, Quaternion.identity);
                    Instantiate(_ChildSpiderPrefab, _eneSpawnPo2.position, Quaternion.identity);
                    //_SpiderObject1.transform.position = _eneSpawnPo1;
                    //_SpiderObject1.SetActive(true);
                    //_SpiderObject2.transform.position = _eneSpawnPo2;
                    //_SpiderObject2.SetActive(true);
                }
            }

            switch (ActivityType)   //行動のタイプによって動きを変える
            {
                case (byte)e_ActivityType.Stan:
                    if (StanType == (byte)e_StanType.move && StanTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, -_StanFallSpeed);
                        rb.velocity = speed;
                    }

                    break;

                case (byte)e_ActivityType.Jump:

                    if (JumpTime <= 0.18f)
                    {
                        animator.SetBool("Stand1", false);
                        animator.SetBool("Stand2", false);
                        animator.SetBool("BodyPress", false);
                        animator.SetBool("BeforeAtack", false);
                        animator.SetBool("Atack", false);
                        animator.SetBool("Jump", true);
                        animator.SetBool("Stun", false);
                        animator.SetBool("Death", false);
                    }
                    if (JumpTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, _jumpSpeed);
                        rb.velocity = speed;
                    }
                    break;

                case (byte)e_ActivityType.Attack:
                    switch (AttackType)
                    {
                        case (byte)e_AttackType.fallmove:
                            if (AttackTime <= 0)
                            {
                                animator.SetBool("Stand1", false);
                                animator.SetBool("Stand2", true);
                                animator.SetBool("BodyPress", false);
                                animator.SetBool("Atack", false);
                                animator.SetBool("Jump", false);
                                animator.SetBool("Stun", false);
                                animator.SetBool("Death", false);

                                Vector2 speed = new Vector2(0.0f, moveSpeed);
                                rb.velocity = speed;
                                if (startPlayerPosition.y + _playerY >= transform.position.y)
                                {
                                    AttackType = (byte)e_AttackType.horizontalmove; //上から降りてプレイヤーの中心点
                                    Acceleration = IntAcceleration;
                                    moveSpeed = _horizontalSpeed;

                                    
                                    if (playerObject.transform.position.x >= transform.position.x)
                                    {
                                        DirectionX = 1f;
                                        var ls = startScale;
                                        transform.localScale = new Vector2(ls.x, -ls.y);
                                    }

                                    else
                                    {
                                        DirectionX = -1f;
                                        var ls = startScale;
                                        transform.localScale = new Vector2(ls.x, ls.y);
                                    }
                                    FallLocationY = transform.position.y;
                                    Debug.Log(FallLocationY);
                                    rb.velocity = new Vector2(0.0f, 0.0f);
                                }
                            }
                            break;

                        case (byte)e_AttackType.horizontalmove:

                            if (AttackTime <= 0)
                            {
                                float T = 2;     //Ｔ＝１周に必要な秒数
                                float f = 1.0f / T; //１を何等分するか？
                                float sin = Mathf.Sin(2 * Mathf.PI * f * Acceleration);    //2 * Mathf.PI =  棒の直径 × π

                                Acceleration += Time.deltaTime;    //加速度の数値を上げる
                                Vector2 speed = new Vector2(moveSpeed * DirectionX, 0); //横の移動の設定
                                rb.velocity = speed;
                                Vector3 po = transform.position;    //今の座標を取得
                                this.transform.position = new Vector3(po.x, FallLocationY + sin * 5, po.z); //縦の波の動きの設定
                                if (_bossLeftMaxPx >= transform.position.x && DirectionX == -1 || _bossRightMaxPx <= transform.position.x && DirectionX == 1)
                                {
                                    DirectionX *= -1;
                                    var ls = transform.localScale;
                                    transform.localScale = new Vector2(ls.x, -ls.y);
                                    CenterIfflag = true;
                                }

                                if (CenterIfflag == true && (transform.position.x >= _stageCenterPx && DirectionX == 1 ||
                                    _stageCenterPx >= transform.position.x && DirectionX == -1))     //|| _stageCenterPx >= transform.position.x && DirectionX == -1
                                {
                                    if ((byte)_activityTypeCount[ActivityCount + 1] == (byte)e_ActivityType.Attack)
                                    {
                                        ActivityCount++;
                                        if (DirectionX == -1)
                                        {
                                            DirectionX = 1f;
                                            var ls = startScale;
                                            transform.localScale = new Vector2(ls.x, -ls.y);
                                        }
                                        else if (DirectionX == 1)
                                        {
                                            DirectionX = -1f;
                                            var ls = startScale;
                                            transform.localScale = new Vector2(ls.x, ls.y);
                                        }
                                        CenterIfflag = false;
                                    }
                                    else
                                    {
                                        speed = new Vector2(0, 0); //横の移動の設定
                                        rb.velocity = speed;
                                        InitActivityType((byte)_activityTypeCount[++ActivityCount]);
                                    }
                                }
                            }
                            break;
                    }
                    break;

                case (byte)e_ActivityType.CallSpider:
                    if(CallTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, _ascentSpeed);    
                        rb.velocity = speed;    //上昇する
                        
                    }
                    break;

                case (byte)e_ActivityType.BodyPress:

                    if (BodyPressTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, moveSpeed);
                        rb.velocity = speed;
                        animator.SetBool("Stand1", false);
                        animator.SetBool("Stand2", false);
                        animator.SetBool("BodyPress", true);
                        animator.SetBool("BeforeAtack", false);
                        animator.SetBool("Atack", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stun", false);
                        animator.SetBool("Death", false);
                    }
                    break;
            }
        }
    }

    void IntAnime()
    {
        transform.position = _aniPosition;
        _bossSpiderFront.transform.position = _aniPosition;
        _ThreadObject.SetActive(true);
        AnimeSwitch = true;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, 90f);
        _animeObject.transform.position = StartAnimeObjectPo;
        _animeObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        moveSpeed = -_aniFallSpeed;
        Threadflag = true; //真正面の方の子蜘蛛につく
    }

    void InitActivityType(byte Type)
    {
        //Debug.Log(SwitchFlag);
        switch (Type)
        {
            case (byte)e_ActivityType.Stan: //スタンの初期変更
                ActivityType = (byte)e_ActivityType.Stan;
                StanType = (byte)e_StanType.move;
                break;

            case (byte)e_ActivityType.Jump: //ジャンプの初期変更
                _ThreadObject.SetActive(false);
                ActivityType = (byte)e_ActivityType.Jump;
                AttackType = 0;
                JumpTime = _jumpRate;
                PlayerDamage = _AttackHitDamage;
                if (transform.localScale.y < 0)
                {
                    var ls = gameObject.transform.localScale;   //localscaleの格納
                    gameObject.transform.localScale = new Vector3(ls.x, -ls.y, ls.z);
                }
                break;

            case (byte)e_ActivityType.Attack:   //攻撃の初期変更
                
                _ThreadObject.SetActive(true);
                ActivityType = (byte)e_ActivityType.Attack;
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 90f);
                PlayerDamage = _AttackHitDamage;
                CenterIfflag = false;

                if (AnimeSwitch)
                {
                    if(Threadflag)
                    {
                        transform.position = _bossSpiderFront.transform.position;
                        _bossSpiderFront.transform.position = StartBossFrontPo;
                        var po = transform.position;
                        _ThreadObject.transform.position = new Vector2(po.x, po.y + 17f);
                    }
                    AttackType = (byte)e_AttackType.horizontalmove; //上から降りてプレイヤーの中心点
                    Acceleration = IntAcceleration;
                    moveSpeed = _horizontalSpeed;
                    FallLocationY = transform.position.y;

                    animator.SetBool("Stand1", false);
                    animator.SetBool("Stand2", true);
                    animator.SetBool("BodyPress", false);
                    animator.SetBool("BeforeAtack", false);
                    animator.SetBool("Atack", false);
                    animator.SetBool("Jump", false);
                    animator.SetBool("Stun", false);
                    animator.SetBool("Death", false);

                    //端まで移動したら反転する
                    if (playerObject.transform.position.x >= transform.position.x)
                    {
                        DirectionX = 1f;
                        var ls = startScale;
                        transform.localScale = new Vector2(ls.x, -ls.y);
                    }

                    else
                    {
                        DirectionX = -1f;
                        var ls = startScale;
                        transform.localScale = new Vector2(ls.x, ls.y);
                    }
                }
                else
                {
                    AttackType = (byte)e_AttackType.fallmove;
                    int range = Random.Range(_rangeMin, _rangeMax);
                    AttackTime += range;
                    transform.position = new Vector2(_stageCenterPx, transform.position.y);
                    moveSpeed = -_fallSpeed;
                }
                break;

            case (byte)e_ActivityType.CallSpider:
                ActivityType = (byte)e_ActivityType.CallSpider;
                CallTime = _callRate;
                SoundManagerV2.Instance.PlaySE(45);
                cameraShake.Shake(_callRate, _aniRoarswing); //カメラの揺れ
                GameObject.Find("Main Camera").GetComponent<RadialBlurSc>().RadialBlur(_callRate, _aniRoarswing);
                if (playerObject.transform.position.x >= transform.position.x)
                {
                    transform.localScale = new Vector3(startScale.x, startScale.y);
                }
                else
                {
                    transform.localScale = new Vector3(startScale.x, -startScale.y);
                }
                break;

            case (byte)e_ActivityType.BodyPress:    //ボディプレスの初期変更
                _ThreadObject.SetActive(false);
                ActivityType = (byte)e_ActivityType.BodyPress;
                BodyPressTime = _bodyPressRate;
                moveSpeed = -_bodyPressFallSpeed;
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _bodyPressEndObject.SetActive(true);
                PlayerDamage = _BodyPressHitDamage;
 
                break;
        }
    }

    public byte GetActivityType()
    {
        return (byte)_activityTypeCount[ActivityCount];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isZeroHP) return;

        if (AnimeSwitch)
        {
            if (collision.CompareTag("Player"))
            {
                AnimeMove = true;
                //_animeObject.SetActive(false);
                /*真正面の画像差し替えとアニメーションはここからがいいかも*/
                animetorFront.SetBool("Stand", true);
                animetorFront.SetBool("Roar", false);
                _shutter1.CloseShutter();
                _shutter2.CloseShutter();
            }
            return;
        }

        // 弱点のみ、IsTriggerをオンにしている。
        if (collision.CompareTag("AcidFlask"))
        {
            nowHP -= _HitDamage;
            Debug.Log(gameObject.name + "の弱点にヒット");
            SoundManagerV2.Instance.PlaySE(4);
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                isZeroHP = true;
            }
        }

        if (collision.CompareTag("PatrolPoint") && !SwitchFlag)     //SwitchObjectとの当たり判定
        {
            SwitchFlag = true;
            _switchObject.gameObject.SetActive(false);
            gameObject.transform.localScale = startScale;
            if (_activityTypeCount.Length <= ++ActivityCount)
            {
                ActivityCount = 0;
            }
            //Debug.Log((byte)_activityTypeCount[ActivityCount]);

            rb.velocity = new Vector2(0.0f, 0.0f);
            InitActivityType((byte)_activityTypeCount[ActivityCount]);

            if (Thread.getHpZero() == true)
            {
                Thread.SetIntSwitch(true);
            }

        }

        if (collision.CompareTag("WaitingPoint"))
        {
            if (ActivityType == (byte)e_ActivityType.BodyPress)
            {
                if (_activityTypeCount.Length <= ++ActivityCount)
                {
                    ActivityCount = 0;
                }
                Instantiate(_medkitPrefab, _medkitFallpoint1.position, Quaternion.identity);
                Instantiate(_medkitPrefab, _medkitFallpoint2.position, Quaternion.identity);

                rb.velocity = new Vector2(0.0f, 0.0f);
                InitActivityType((byte)_activityTypeCount[ActivityCount]);
                _bodyPressEndObject.SetActive(false);
                cameraShake.Shake(0.5f, _aniRoarswing); //カメラの揺れ
                SoundManagerV2.Instance.PlaySE(46);
            }

            if (ActivityType == (byte)e_ActivityType.Stan)
            {
                rb.velocity = new Vector2(0.0f, 0.0f);
                StanType = (byte)e_StanType.Stan;
                StanTime = _stanRate;
                _bodyPressEndObject.SetActive(false);
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                var p = transform.position;
                transform.position = new Vector3(p.x, startposition.y, p.z);
                cameraShake.Shake(0.5f, _aniRoarswing); //カメラの揺れ
                if (transform.localScale.y > 0)
                {
                    var ls = transform.localScale;
                    transform.localScale = new Vector3(ls.x, -ls.y, ls.z);
                }
                if (transform.position.x <= playerObject.transform.position.x)
                {
                    var ls = transform.localScale;
                    transform.localScale = new Vector3(-ls.x, ls.y, ls.z);
                }
            }


        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isZeroHP) return;

        if (collision.CompareTag("Player") && _PlayerDamageTime <= 0)
        {
            _PlayerDamageTime += _PlayerDamageRate;
            collision.gameObject.GetComponent<PlayerController>().Damage(PlayerDamage);
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 targetPos = collision.gameObject.transform.position;
            float y = _nockBuckUpperPower;
            float x = targetPos.x;
            Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
            if (!collision.gameObject.GetComponent<PlayerController>().IsNotNockBack)
            {
                prb.velocity = direction * _nockBuckPower;
            }
            SoundManagerV2.Instance.PlaySE(2);
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isZeroHP) return;

        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Debug.Log(gameObject.name + "の非弱点にヒット");
            SoundManagerV2.Instance.PlaySE(7);
        }

        if (collision.gameObject.CompareTag("Gareki"))
        {
            animator.SetBool("Stand1", false);
            animator.SetBool("Stand2", false);
            animator.SetBool("BodyPress", false);
            animator.SetBool("BeforeAtack", false);
            animator.SetBool("Atack", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Stun", true);
            animator.SetBool("Death", false);

            StanTime += _stanRate;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
            SoundManagerV2.Instance.PlaySE(3);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Damage(PlayerDamage);
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 targetPos = collision.gameObject.transform.position;
            float y = _nockBuckUpperPower;
            float x = targetPos.x;
            Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
            if (!collision.gameObject.GetComponent<PlayerController>().IsNotNockBack)
            {
                prb.velocity = direction * _nockBuckPower;
            }
            SoundManagerV2.Instance.PlaySE(2);
        }
    }
}
