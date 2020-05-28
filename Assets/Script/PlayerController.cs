﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーのスクリプト
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private ScoreManager sm;
    private InputManager im;
    private PlayerManager pm;
    private ObjectPool pool;
    private GameObject cam;
    private CapsuleCollider2D capcol;
    private BoxCollider2D boxcol;
    private CubismRenderController cubismRender;
    public bool _Debug;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField, CustomLabel("デバッグテキスト")] private Text _debug;
    [SerializeField, CustomLabel("BGMミュート")] private bool isBGMMute = false;

    [SerializeField, CustomLabel("最初から銃を所持")] private bool isStartGetGun = false;
    [SerializeField, CustomLabel("HPバー")] private Slider _HPbar;
    [HideInInspector, CustomLabel("残弾数UI")] private Text _bulletsRemain;
    [HideInInspector, CustomLabel("ハンドガン装備UI")] private Button _handgunUI;
    [HideInInspector, CustomLabel("ホールメイカー装備UI")] private Button _hmUI;
    [HideInInspector, CustomLabel("装備UI表示切替")] private bool _isUIDisplay = false;

    private bool flip = true;

    //アニメーション関連
    Animator animator;
    private State state;
    private int Shot0Layer;
    private int Shot1Layer;
    private int Shot2Layer;
    private int Shot3Layer;
    private int Shot4Layer;
    private float weight0;
    private float weight1;
    private float weight2;
    private float weight3;
    private float weight4;
    private bool smoothFlag;
    private CubismModel Model;
    private float anicount;
    enum State
    {
        None,
        Shot0,
        Shot1,
        Shot2,
        Shot3,
        Shot4
    }
    private GameObject drawables;    // キャラクターの見た目

    [SerializeField, CustomLabel("ジャンプオフセット銃未取得")] private Vector3 _drwablOffsetNormal;
    [SerializeField, CustomLabel("ジャンプオフセット銃未取得走り時")] private Vector3 _drwablOffsetNormalRunning;
    [SerializeField, CustomLabel("ジャンプオフセット銃取得済")] private Vector3 _drwablOffsetGetGun;
    [SerializeField, CustomLabel("ジャンプオフセット銃取得済走り")] private Vector3 _drwablOffsetGetGunRunning;
    [SerializeField, CustomLabel("着地オフセット")] private Vector3 _drwablOffsetGraunding;
    private Vector3 drwablsStartOffset;

    [SerializeField, CustomLabel("地面との当たり判定")] private ContactFilter2D filter2d;
    private bool isGrounded = true;
    private int groundingTime = 11;

    [SerializeField, CustomLabel("残留酸プール")] private GameObject _rsdAcdPool;
    [SerializeField, CustomLabel("弾のプレハブ")] private GameObject _acidbulletPrefab;
    [SerializeField, CustomLabel("ダメージエフェクト")] private GameObject _damageEffect;
    [SerializeField, Range(0.001f, 9999f), CustomLabel("最大HP")] private float _maxHP = 9999f;
    [SerializeField, CustomLabel("無敵時間")] private float _resetInvincibleTime = 4f;
    [SerializeField, Range(0f, 9999f), CustomLabel("酸に触れたときの被ダメージ")] private float _acidDamage = 500f;
    [SerializeField, Range(0.0167f, 10f), CustomLabel("酸の被ダメージレート")] private float _acidDamageRate = 0.5f;
    [SerializeField, Range(0, 999), CustomLabel("弾の最大所持数")] private int _bulletCapacity = 10;
    [HideInInspector, CustomLabel("ホールメイカー弾最大所持数")] private int _hmBulletCapacity = 9999;
    [HideInInspector, CustomLabel("ホールメイカー弾同時発射数")] private int _hmShotBullets = 6;
    [HideInInspector, CustomLabel("ホールメイカー拡散範囲")] private float _hmSpreadRange = 45f;
    [HideInInspector, CustomLabel("ホールメイカー発射間隔")] private float _hmFireRate = 2.5f;
    [SerializeField, Range(0f, 5f), CustomLabel("弾の発射間隔")] private float _fireRate = 0f;

    [SerializeField, Range(0.001f, 1f), CustomLabel("スティック上向きの閾値")] private float YStickUpDeadZone = 0.4f;
    [SerializeField, Range(0.001f, 1f), CustomLabel("真上の閾値")] private float ceilDeadZone = 0.5f;
    [SerializeField, Range(-0.001f, -1f), CustomLabel("スティック下向きの閾値")] private float _YStickDownDeadZone = -0.4f;
    [SerializeField, Range(0.001f, 1f), CustomLabel("真下の閾値")] private float floorDeadZone = -0.5f;
    [SerializeField, CustomLabel("上下狙い中横移動デッドゾーン")] private float _moveDeadZone = 0.55f;
    [SerializeField, CustomLabel("酸ダメージ中プレイヤー色")] private Color _colorInAcidDamage;

    [Serializable]
    private class CeilShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・真上")] private float _muzzleVelocity = 11.5f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・真上")] private float _gravityScale = 6f;
        [SerializeField, Range(0f, 90f), CustomLabel("発射角度・真上")] private float _fireAngle = 90f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("真上へ発射")] UpShot ceilShot;
    
    [Serializable]
    private class UpShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・上")] private float _muzzleVelocity = 11.5f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・上")] private float _gravityScale = 6f;
        [SerializeField, Range(0f, 90f), CustomLabel("発射角度・上")] private float _fireAngle = 63f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("上へ発射")] UpShot upShot;

    [Serializable]
    private class HorizontalShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・水平")] private float _muzzleVelocity = 11f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・水平")] private float _gravityScale = 12f;
        [SerializeField, Range(-45f, 45f), CustomLabel("発射角度・水平")] private float _fireAngle = 20f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("水平に発射")] HorizontalShot horizontalShot;

    [Serializable]
    private class DownShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・下")] private float _muzzleVelocity = 11f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・下")] private float _gravityScale = 5f;
        [SerializeField, Range(0f, -90f), CustomLabel("発射角度・下")] private float _fireAngle = -20f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("下へ発射")] DownShot downShot;
    
    [Serializable]
    private class FloorShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・真下")] private float _muzzleVelocity = 11.5f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・真下")] private float _gravityScale = 4f;
        [SerializeField, Range(0f, 90f), CustomLabel("発射角度・真下")] private float _fireAngle = -90f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("真下へ発射")] FloorShot floorShot;

    [SerializeField, CustomLabel("全Drawables")] private CubismRenderer[] Drawables;

    private enum Equipment
    {
        None,
        Handgun,
        HoleMaker
    }
    Equipment equipment;

    private List<float> hmSpreadAngle = new List<float>();
    private GameObject[] restartPoints;
    private float muzzleVelocity = 5f;
    private bool isGetGun = false;
    private bool isGetHoleMaker = false;
    private Vector3 mainThrowPoint;
    private GameObject fireCheckPoint;
    private float HP;
    private float startMoveSpeed;
    private float moveDeadZone;

    public float Hp
    {
        get { return HP; }
    }

    private float invincibleTime;
    private int bullets;
    private int hmBullets;
    
    private float fireTime;
    private float acidDamageTime;

    private float jumpWaitTime = -0.1f;
    private bool isJumping;
    private bool isJumpingCheck = true;
    private float jumpTimeCounter;
    private float _jumpPower;
    private float jumpMinTime;

    private Vector3 restartPosition;
    private float restartHP;
    private Vector3 startPosition;
    private Vector3 restartCameraPosition;

    private GameObject damageEffect;

    public bool IsGodMode { get; set; }

    public bool IsNotNockBack { get; set; }

    public PlayerController()
    {
        IsMachinGun = false;
        IsNotNockBack = false;
        IsGodMode = false;
    }

    public bool IsMachinGun { get; set; }
    
    public bool IsSuperJump { get; set; }

    private bool isGhost;

    public bool IsGhost
    {
        get { return isGhost; }
    }

    private void Awake()
    {
        pool = gameObject.AddComponent<ObjectPool>();
        pool.CreatePool(_acidbulletPrefab, 6);
        Instantiate(_rsdAcdPool);
        damageEffect = Instantiate(_damageEffect);
    }

    private void Start()
    {
        sm = ScoreManager.Instance;
        sm.StageScoreReset();
        im = InputManager.Instance;
        pm = PlayerManager.Instance;
        cam = GameObject.Find("Main Camera");
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        boxcol = transform.GetChild(7).GetComponent<BoxCollider2D>();
        fireCheckPoint = transform.GetChild(8).gameObject;
        cubismRender = GetComponent<CubismRenderController>();
        jumpTimeCounter = pm.JumpTime;

        if (!isBGMMute) {

        }
        if (_hmShotBullets % 2 != 0) {
            _hmShotBullets--;
        }
        PreCalcSpreadAngle();

        //アニメーション関連
        Model = this.FindCubismModel();
        animator = GetComponent<Animator>();
        Shot0Layer = animator.GetLayerIndex("Shot0 Layer");
        Shot1Layer = animator.GetLayerIndex("Shot1 Layer");
        Shot2Layer = animator.GetLayerIndex("Shot2 Layer");
        Shot3Layer = animator.GetLayerIndex("Shot3 Layer");
        Shot4Layer = animator.GetLayerIndex("Shot4 Layer");
        weight0 = 0f;
        weight1 = 0f;
        weight2 = 0f;
        weight3 = 0f;
        weight4 = 0f;
        SetState(State.None, first: true);
        drawables = transform.GetChild(5).gameObject;
        drwablsStartOffset = drawables.transform.localPosition;

        if (_isUIDisplay) {
            _bulletsRemain.text = " ∞ ";
            _bulletsRemain.enabled = false;
        }

        restartPoints = GameObject.FindGameObjectsWithTag("RestartPoint");
        restartPosition = transform.position;
        startPosition = transform.position;
        restartHP = _maxHP;
        restartCameraPosition = cam.transform.position;

        if (isStartGetGun)
        {
            if (_isUIDisplay) {
                _handgunUI.gameObject.SetActive(true);
                _handgunUI.Select();
                _bulletsRemain.enabled = true;
            }
            
            isGetGun = true;
            if (state != State.Shot2){
                SetState(State.Shot2);
            }
            equipment = Equipment.Handgun;
        }

        startMoveSpeed = pm.MoveSpeed;

        HP = _maxHP;
        if (!SaveManager.Instance.IsNewGame) {
            HP = SaveManager.Instance.save.playerHP;
        }

        _HPbar.maxValue = _maxHP;
        _HPbar.value = HP;
        _HPbar.GetComponent<PlayerHPbarScript>().isStartFunctionCalledAfter = true;
    }

    private void Update()
    {
        if (!(0 < HP)) return;

        // 真上に発射
        if (ceilDeadZone < im.Trigger && isGetGun)
        {
            mainThrowPoint = transform.GetChild(3).transform.position;
            anicount = 0.0f;
            animator.SetBool("Wait", false);
            if (state != State.Shot0){
                SetState(State.Shot0);
            }

            // 真下に発射
        } else if (floorDeadZone > im.Trigger && isGetGun) {
            mainThrowPoint = transform.GetChild(2).transform.position;
            anicount = 0.0f;
            animator.SetBool("Wait", false);
            if (state != State.Shot4){     
                SetState(State.Shot4);
            }

            // 上に発射
        } else if (YStickUpDeadZone < im.UpMoveKey && isGetGun) {
            mainThrowPoint = transform.GetChild(0).transform.position;
            anicount = 0.0f;
            animator.SetBool("Wait", false);
            moveDeadZone = _moveDeadZone;
            if (state != State.Shot1){
                SetState(State.Shot1);
            }

            // 下に発射
        } else if (im.UpMoveKey < _YStickDownDeadZone && isGetGun) {
            mainThrowPoint = transform.GetChild(2).transform.position;
            anicount = 0.0f;
            animator.SetBool("Wait", false);
            moveDeadZone = _moveDeadZone;
            if (state != State.Shot3){
                SetState(State.Shot3);
            }
            
            // 正面に発射
        } else if (isGetGun){
            mainThrowPoint = transform.GetChild(1).transform.position;
            moveDeadZone = 0;
            if (state != State.Shot2){
                SetState(State.Shot2);
            }
        }

        if (smoothFlag){
            if (state == State.Shot0){
                weight0 = 1f;
                weight1 = 0f;
                weight2 = 0f;
                weight3 = 0f;
                weight4 = 0f;
            }else if (state == State.Shot1){
                weight1 = 1f;
                weight0 = 0f;
                weight2 = 0f;
                weight3 = 0f;
                weight4 = 0f;
            }else if (state == State.Shot2){
                weight2 = 1f;
                weight0 = 0f;
                weight1 = 0f;
                weight3 = 0f;
                weight4 = 0f;
            }else if (state == State.Shot3){
                weight3 = 1f;
                weight0 = 0f;
                weight1 = 0f;
                weight2 = 0f;
                weight4 = 0f;
            }else if (state == State.Shot4){
                weight4 = 1f;
                weight0 = 0f;
                weight1 = 0f;
                weight2 = 0f;
                weight3 = 0f;
            }
            animator.SetLayerWeight(Shot0Layer, weight0);
            animator.SetLayerWeight(Shot1Layer, weight1);
            animator.SetLayerWeight(Shot2Layer, weight2);
            animator.SetLayerWeight(Shot3Layer, weight3);
            animator.SetLayerWeight(Shot4Layer, weight4);
        }

        // ジャンプボタンを押した瞬間は、アニメーションだけ先に動作させます！
        if (isJumpingCheck && im.JumpKey == 1 && isGrounded && jumpWaitTime < 0) {
            anicount = 0.0f;
            if (animator.GetBool("Run")) {
                animator.SetBool("JumpStart-run", true);
            } else {
                animator.SetBool("JumpStart", true);
            }

            animator.SetBool("JumpUp", false);
            animator.SetBool("JumpUp-run", false);
            animator.SetBool("JumpDown", false);
            animator.SetBool("JumpEnd", false);
            animator.SetBool("Run", false);
            animator.SetBool("Stand", false);
            animator.SetBool("Wait", false);
            jumpWaitTime = jumpWaitTime * 0 + (0.0167f * pm.JumpWaitTime);
            groundingTime = 0;
            
        }

        // ジャンプの本処理はこっち。
        if (0 <= jumpWaitTime)
        {
            jumpWaitTime -= Time.deltaTime;
            if (isJumpingCheck && jumpWaitTime < 0 && jumpMinTime <= 0 && !isJumping) 
            {
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", true);
                animator.SetBool("JumpUp", true);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", false);
                animator.SetBool("Run", false);
                animator.SetBool("Stand", false);
                animator.SetBool("Wait", false);

                isJumpingCheck = false;
                jumpTimeCounter = pm.JumpTime;
                isJumping = true;
                _jumpPower = pm.JumpPower;
                jumpMinTime = pm.JumpMinTime;
                SoundManagerV2.Instance.PlaySE(9);
                groundingTime = 0;
            } else {
                isJumpingCheck = true;
            }
        }

        if (im.MoveStopKey == 2) {
            pm.MoveSpeed = 0;
        } else if (im.MoveStopKey == 0) {
            pm.MoveSpeed = startMoveSpeed;
        }

        //{11(6),3(7),手前}{15(11),14(12)奥側} 手のパーツ
        if (!isGetGun){                             //銃を持ってるとき、右手に銃を持たす
            Model.Parts[6].Opacity = 1;
            Model.Parts[7].Opacity = 0;
            Model.Parts[11].Opacity = 1;
            Model.Parts[12].Opacity = 0;
        }else if (isGetGun && flip){                             //銃を持ってるとき、右手に銃を持たす
            Model.Parts[6].Opacity = 0;
            Model.Parts[7].Opacity = 1;
            Model.Parts[11].Opacity = 1;
            Model.Parts[12].Opacity = 0;
        }else if (isGetGun && !flip){                             //銃を持ってるとき、左手に銃を持たす
            Model.Parts[6].Opacity = 1;
            Model.Parts[7].Opacity = 0;
            Model.Parts[11].Opacity = 0;
            Model.Parts[12].Opacity = 1;
        }

        if ((im.ShotKey == 1 || (im.ShotKey == 2 && IsMachinGun)) && fireTime <= 0) {

            if(equipment == Equipment.Handgun && isGetGun) {
                HandgunShot();
            } else if(equipment == Equipment.HoleMaker && isGetHoleMaker && 0 < hmBullets) {
                HoleMakerShot();
            }
        }

        if (im.EquipHandGun && isGetGun) {
            if (_isUIDisplay) {
                _handgunUI.Select();
                _bulletsRemain.text = " ∞ ";
            }
            equipment = Equipment.Handgun;
        }

        if (im.EquipHoleMaker && isGetHoleMaker && 0 < hmBullets) {
            if (_isUIDisplay) {
                _hmUI.Select();
                _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
            }
            equipment = Equipment.HoleMaker;
        }

        // ホールメイカーの隠しコマンド
        if(Input.GetKey(KeyCode.H) && Input.GetKeyUp(KeyCode.M)) {
            if (_isUIDisplay) {
                _hmUI.gameObject.SetActive(true);
                _hmUI.Select();
                _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
            }
            isGetHoleMaker = true;
            SoundManagerV2.Instance.PlaySE(12);
            equipment = Equipment.HoleMaker;
            hmBullets += _hmBulletCapacity;
        }
 
    }

    private void FixedUpdate()
    {
        // HPが0だった場合の処理
        if (!(0 < HP)) {
            rb.AddForce(new Vector2(pm.MoveForceMultiplier * (0 * pm.MoveSpeed - rb.velocity.x), Physics.gravity.y * pm.GravityRate));
            return;
        }

        sm.PlayTime += Time.deltaTime;
        sm.GameClearTime += Time.deltaTime;
        
        if(0 < fireTime) {
            fireTime -= Time.deltaTime;
        }

        if(0 < acidDamageTime) {
            acidDamageTime -= Time.deltaTime;
            if(acidDamageTime <= 0) {
                foreach (var dr in Drawables) {
                    dr.Color = new Color(1,1,1,1);
                }
            } else {
                foreach (var dr in Drawables) {
                    dr.Color += new Color(1 / _acidDamageRate * Time.deltaTime, 1 / _acidDamageRate * Time.deltaTime, 1 / _acidDamageRate * Time.deltaTime, 0);
                }
            }

        }

        if (0 < invincibleTime) {
            invincibleTime -= Time.deltaTime;
            IsNotNockBack = true;
            if (invincibleTime <= 0)
            {
                cubismRender.Opacity = 1f;
                IsNotNockBack = false;
            }
        }
        
        if (im.MoveKey >= 0.005 && !flip && im.MoonWalkKey == 0) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = true;
        }
        if (im.MoveKey <= -0.005 && flip && im.MoonWalkKey == 0) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = false;
        }
        
        // 地面と当たり判定をしている
        isGrounded = rb.IsTouching(filter2d);

        if (isGhost)
        {
            GhostMove();
        }
        else
        {
            PlayerMove();
        }
        
    }

    private void LateUpdate()
    {
        StartCoroutine("DrawablesControl");
    }

    private void PlayerMove()
    {
        // 無限ジャンプのチート用処理
        if (IsSuperJump && im.JumpKey == 2)
        {
            jumpTimeCounter = pm.JumpTime;
            isJumpingCheck = false;
            isJumping = true;
            _jumpPower = pm.JumpPower;
            anicount = 0.0f;
            animator.SetBool("JumpStart", false);
            animator.SetBool("JumpUp", true);
            animator.SetBool("JumpStart-run", false);
            animator.SetBool("JumpUp-run", true);
            animator.SetBool("JumpDown", false);
            animator.SetBool("JumpEnd", false);
            animator.SetBool("Run", false);
            animator.SetBool("Stand", false);
            animator.SetBool("Wait", false);      
        }

        // 地面にいるとき
        if (isGrounded && !isJumping && jumpWaitTime < 0)
        {
            if (groundingTime < pm.MaxLandingTime) {
                groundingTime++;
            }

            if ((im.MoveKey <= -moveDeadZone || moveDeadZone <= im.MoveKey) && jumpWaitTime < 0)
            {
                rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.MoveSpeed - rb.velocity.x), rb.velocity.y));
            }
            else
            {
                rb.AddForce(new Vector2(pm.MoveForceMultiplier * (0 * pm.MoveSpeed - rb.velocity.x), rb.velocity.y));
            }
            
            anicount += Time.deltaTime;

            if (0 < groundingTime && groundingTime < pm.MaxLandingTime && jumpWaitTime < 0)
            {
                anicount = 0.0f;
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", true);
                // ここに着地した瞬間の処理書くといいかも
            }
            else if ((im.MoveKey < -moveDeadZone || moveDeadZone < im.MoveKey) && jumpWaitTime < 0 && im.MoveStopKey == 0)    // 移動中
            {
                anicount = 0.0f;            
                animator.SetBool("Run", true);
                animator.SetBool("Stand", false);
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", false);
                animator.SetBool("Wait", false);
            }
            else if (anicount >= 5.0f && im.MoveKey >= -moveDeadZone && moveDeadZone >= im.MoveKey  && jumpWaitTime < 0)    // 待機モーション中
            {
                if (anicount >= 12.0f) {anicount = 0.0f; }         
                animator.SetBool("Wait", true);
                animator.SetBool("Stand", false);
            }
            else if (im.MoveKey >= -moveDeadZone && moveDeadZone >= im.MoveKey && rb.velocity.x <= 4f && -4f <= rb.velocity.x  && jumpWaitTime < 0 || im.MoveStopKey != 0)    // 停止中
            {
                animator.SetBool("Stand", true);
                animator.SetBool("Run", false);
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", false);
                animator.SetBool("Wait", false);
            }
            // 空中にいるとき
        } else {
            groundingTime = 0;

            if (IsSuperJump && im.JumpKey == 2)
            {
                isJumping = true;
            }

            // ジャンプキーが話されたらジャンプ中でないことにする
            if (im.JumpKey == 0  && jumpWaitTime < 0 && jumpMinTime < 0) {
                isJumping = false;
            }
            
            // ジャンプしてない
            if (!isJumping  && jumpWaitTime < 0) {
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", false);
                animator.SetBool("Run", false);
                if (!isGrounded && !isJumping){                   
                    animator.SetBool("JumpDown", true);
                    animator.SetBool("JumpUp-run", false);
                    animator.SetBool("JumpEnd", false);
                    animator.SetBool("Run", false);
                }

                // 落ちる力が一定を超えるとそれ以上落下速度が上がらないようにする
                if (rb.velocity.y <= -pm.MaxFallSpeed) {
                    rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.JumpMoveSpeed - rb.velocity.x), 0));
                } else if(rb.velocity.y <= 0) {
                    // 落ち始めると重力を使って落ちるようにする
                    rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * pm.GravityRate));
                } else {
                    // ジャンプパワーがまだあるときは小ジャンプ実現のためにジャンプパワー軽減率を使う
                    if (0 <= _jumpPower) {
                        _jumpPower -= pm.JumpPowerAttenuation * 2;
                        rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.JumpMoveSpeed - rb.velocity.x), _jumpPower));
                    // ジャンプパワーがないときは重力を使って落とす
                    } else {
                        rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * pm.GravityRate));
                    }
                }
            }
        }

        // ジャンプ中
        if (isJumping)
        {
            groundingTime = 0;
            // ジャンプキーを押し続けていられる時間をへらす
            jumpTimeCounter -= Time.deltaTime;
            jumpMinTime -= Time.deltaTime;
            anicount = 0.0f;
            animator.SetBool("Run", false);

            // ジャンプキーを押し続けている間は通常のジャンプパワー軽減率がはたらく
            if (im.JumpKey == 2 || 0 <= jumpMinTime) {
                _jumpPower -= pm.JumpPowerAttenuation;
                rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.JumpMoveSpeed - rb.velocity.x), _jumpPower));
            }

            // ジャンプキーを押し続けていられる時間がくると、ジャンプ中を解除する
            if (jumpTimeCounter < 0 && jumpMinTime < 0) {
                isJumping = false;
            }

            // 下に落ちているときはジャンプ中を解除
            if (rb.velocity.y <= -1) {
                isJumping = false;
            }
        }

        if (im.JumpKey == 0 && jumpWaitTime < 0) {
            isJumpingCheck = true;
        }

        if (_Debug) _debug.text = animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("JumpStart")).ToString();
    }

    private IEnumerator DrawablesControl()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("JumpStart-run"))) {
            yield return new WaitForSeconds(0.02f);
            drawables.transform.localPosition = _drwablOffsetNormalRunning;
        } else if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("JumpStart"))) {
            yield return new WaitForSeconds(0.02f);
            drawables.transform.localPosition = _drwablOffsetNormal;
        } else if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("JumpEnd"))) {
            yield return new WaitForSeconds(0.02f);
            drawables.transform.localPosition = _drwablOffsetGraunding;
        } else {
            drawables.transform.localPosition = drwablsStartOffset;
        }
    }

    private void GhostMove()
    {
        rb.AddForce(new Vector2(pm.MoveForceMultiplier * (im.MoveKey * pm.MoveSpeed*2 - rb.velocity.x), pm.MoveForceMultiplier * (im.UpMoveKey * pm.MoveSpeed*2 - rb.velocity.y)));
        if (im.JumpKey == 2)
        {
            rb.AddForce(new Vector2(rb.velocity.x, pm.MoveForceMultiplier * (pm.MoveSpeed*2 - rb.velocity.y)));
        }
    }

    public void ChangeGhost(bool flag)
    {
        if (flag)
        {
            isGhost = true;
            capcol.enabled = false;
            boxcol.enabled = false;
            cubismRender.Opacity = 0.3f;
        }
        else
        {
            isGhost = false;
            capcol.enabled = true;
            boxcol.enabled = true;
            cubismRender.Opacity = 1f;
        }
    }

    public void Damage(float damage, bool isAcidDamage = false)
    {
        if (!(0 < HP) || IsGodMode) return;
        if (isAcidDamage || invincibleTime <= 0)
        {
            HP -= damage;
            _HPbar.value = HP;
            Debug.Log("HP " + HP + " / " + _maxHP);
            if (!isAcidDamage)
            {
                invincibleTime += _resetInvincibleTime;
                damageEffect.transform.position = transform.position;
                damageEffect.SetActive(true);
                cubismRender.Opacity = 0.3f;
            } else {
                foreach (var dr in Drawables) {
                    dr.Color = _colorInAcidDamage;
                }
            }

            // ↓はHPが0になった瞬間の処理。
            if (HP <= 0)
            {
                animator.SetBool("Stand", true);
                animator.SetBool("Wait", false);
                animator.SetBool("Run", false);
                animator.SetBool("JumpStart", false);
                animator.SetBool("JumpStart-run", false);
                animator.SetBool("JumpUp-run", false);
                animator.SetBool("JumpUp", false);
                animator.SetBool("JumpDown", false);
                animator.SetBool("JumpEnd", false);

                // リスタートするときの位置が初期位置のままだったらシーンリロードさせる。
                // でなければ（リスタートを一度でも通過していれば）リスタートコルーチン発動。
                if (startPosition == restartPosition)
                {
                    FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 1f);
                }
                else
                {
                    FadeManager.Instance.FadeScreen(1f);
                    StartCoroutine(Restart(1f));
                }
                
            }
        }
        
    }

    public void Heal(float healPercent)
    {
        if (!(0 < HP)) return;
        HP += _maxHP * (healPercent / 100);
        _HPbar.value = HP;
        if(_maxHP < HP) {
            HP = _maxHP;
        }
        Debug.Log("HP " + HP + " / " + _maxHP);

    }

    public void DebugHP(float heal)
    {
        HP = heal;
        _HPbar.value = HP;
        Debug.Log("HP " + HP + " / " + _maxHP);
    }

    // ハンドガン発射
    private void HandgunShot()
    {
        GameObject bullet = pool.GetObject();
        if (bullet != null) {
            if (Physics2D.Linecast(fireCheckPoint.transform.position, mainThrowPoint, _layerMask))
            {
                bullet.GetComponent<AcidFlask>().Init(fireCheckPoint.transform.position, true);
                Debug.Log("撃った瞬間に壁に当たっていた");
            } else {
                bullet.GetComponent<AcidFlask>().Init(mainThrowPoint, false);
                Debug.Log("正常に発射された");
            }
            
        }
        Rigidbody2D bRb = bullet.GetComponent<Rigidbody2D>();

        float rad;

        // 上に発射
        if (ceilDeadZone < im.Trigger)
        {
            rad = ceilShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = ceilShot.MuzzleVelocity; //真上へ発射時の初速を代入
            bRb.gravityScale = ceilShot.GravityScale; //真上へ発射時の弾の重量を代入
            
        } else if(im.Trigger < floorDeadZone){
            rad = floorShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = floorShot.MuzzleVelocity; //真下へ発射時の初速を代入
            bRb.gravityScale = floorShot.GravityScale; //真下へ発射時の弾の重量を代入

        } else if (YStickUpDeadZone < im.UpMoveKey) {
            rad = upShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = upShot.MuzzleVelocity; //上へ発射時の初速を代入
            bRb.gravityScale = upShot.GravityScale; //上へ発射時の弾の重量を代入
                                                    // 下に発射
        } else if (im.UpMoveKey < _YStickDownDeadZone) {
            rad = downShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = downShot.MuzzleVelocity; //下へ発射時の初速を代入
            bRb.gravityScale = downShot.GravityScale; //下へ発射時の弾の重量を代入
                                                      // 正面に発射
        } else {
            rad = horizontalShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = horizontalShot.MuzzleVelocity;  //正面へ発射時の初速を代入
            bRb.gravityScale = horizontalShot.GravityScale;  //正面へ発射時の弾の重量を代入
        }

        //rad(ラジアン角)から発射用ベクトルを作成
        double addforceX = Math.Cos(rad * 1);
        double addforceY = Math.Sin(rad * 1);
        Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY, 0);

        // キャラが反対向いてたら反対に発射させる
        if (!flip) {
            shotangle = Vector3.Reflect(shotangle, Vector3.right);
        }

        //　発射角度にオブジェクトを回転させる、飛ぶ方向と弾自身の角度を一致させる
        var axis = Vector3.Cross(bullet.transform.right, shotangle);
        var angle = Vector3.Angle(bullet.transform.right, shotangle) * (axis.z < 0 ? -1 : 1);
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //　発射
        bRb.AddForce(shotangle * muzzleVelocity, ForceMode2D.Force);
        SoundManagerV2.Instance.PlaySE(6);
        anicount = 0.0f;
        animator.SetBool("Wait", false);
        if (_isUIDisplay) {
            _bulletsRemain.text = " ∞ ";
        }
            
        fireTime += _fireRate;
        if (IsMachinGun) fireTime = 0.068f;
    }

    // ホールメイカー発射
    private void HoleMakerShot()
    {
        for (int i = 0; i < _hmShotBullets; i++) {
            GameObject bullet = pool.GetObject();
            if (bullet != null) {
                bullet.GetComponent<AcidFlask>().Init(mainThrowPoint,false);
            }
            Rigidbody2D bRb = bullet.GetComponent<Rigidbody2D>();

            float rad = 0;

            // 上に発射
            if (YStickUpDeadZone < im.UpMoveKey) {
                rad = (upShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = upShot.MuzzleVelocity; //上へ発射時の初速を代入
                bRb.gravityScale = upShot.GravityScale; //上へ発射時の弾の重量を代入
                                                        // 下に発射
            } else if (im.UpMoveKey < _YStickDownDeadZone) {
                rad = (downShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = downShot.MuzzleVelocity; //下へ発射時の初速を代入
                bRb.gravityScale = downShot.GravityScale; //下へ発射時の弾の重量を代入
                                                          // 正面に発射
            } else {
                rad = (horizontalShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = horizontalShot.MuzzleVelocity;  //正面へ発射時の初速を代入
                bRb.gravityScale = horizontalShot.GravityScale;  //正面へ発射時の弾の重量を代入
            }

            //rad(ラジアン角)から発射用ベクトルを作成
            double addforceX = Math.Cos(rad * 1);
            double addforceY = Math.Sin(rad * 1);
            Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY, 0);

            // キャラが反対向いてたら反対に発射させる
            if (!flip) {
                shotangle = Vector3.Reflect(shotangle, Vector3.right);
            }

            //　発射角度にオブジェクトを回転させる、進んでいる方向と角度を一致させる
            var axis = Vector3.Cross(bullet.transform.right, shotangle);
            var angle = Vector3.Angle(bullet.transform.right, shotangle) * (axis.z < 0 ? -1 : 1);
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //　で、でますよ
            bRb.AddForce(shotangle * muzzleVelocity, ForceMode2D.Force);
        }

        StartCoroutine("HoleMakerSound");
        if (_isUIDisplay) {
            _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
        }
        fireTime += _hmFireRate;
        if (IsMachinGun) fireTime = 0.2f;
        if (hmBullets <= 0) {
            _handgunUI.Select();
            equipment = Equipment.Handgun;
            _bulletsRemain.text = " ∞ ";
        }
    }

    private IEnumerator HoleMakerSound()
    {
        SoundManagerV2.Instance.PlaySE(13);
        yield return new WaitForSeconds(_hmFireRate - 1.5f);
        SoundManagerV2.Instance.PlaySE(14);
        yield return new WaitForSeconds(0.5f);
        SoundManagerV2.Instance.PlaySE(15);
    }

    private void PreCalcSpreadAngle()
    {
        float range = _hmSpreadRange / _hmShotBullets;
        // 一番上から一番下まで順に計算する。
        // 一番上は拡散範囲÷2
        hmSpreadAngle.Add(_hmSpreadRange / 2);
        int i;

        // 2番目から真ん中まではループで求める
        for(i = 1; i < _hmShotBullets / 2; i++) {
            hmSpreadAngle.Add(hmSpreadAngle[i-1] - range);
        }

        // 下半分の最初は拡散範囲÷同時発射数×2した数にする
        hmSpreadAngle.Add(hmSpreadAngle[i-1] - range * 2);
        i++;

        // 残りの一番下まではループで
        for(; i < _hmShotBullets; i++) {
            hmSpreadAngle.Add(hmSpreadAngle[i-1] - range);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(0 < HP)) return;
        if (collision.CompareTag("ItemBullet")) {
            Debug.Log("アイテム取得");
            ItemScript itemScript = collision.gameObject.GetComponent<ItemScript>();
            bullets += itemScript.GetBulletsInItem();

            if (_bulletCapacity < bullets) {
                itemScript.SetBulletsInItem(bullets - _bulletCapacity);
                bullets = _bulletCapacity;
            } else {
                itemScript.SetBulletsInItem(0);
                Destroy(collision.gameObject);              
            }
            if (_isUIDisplay) {
                _bulletsRemain.text = " ∞ ";
            }
               
        } else if (collision.CompareTag("PutGun")) {
            Destroy(collision.gameObject);
            GetGun();
        } else if (collision.CompareTag("PutHoleMaker") && isGetGun) {
            if (_isUIDisplay) {
                _hmUI.gameObject.SetActive(true);
                _hmUI.Select();
                _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
            }
            
            isGetHoleMaker = true;
            Destroy(collision.gameObject);
            SoundManagerV2.Instance.PlaySE(12);
            equipment = Equipment.HoleMaker;
            hmBullets += 100;
            
        } else if (collision.CompareTag("RestartPoint")) {
            restartPosition = transform.position;
            restartHP = HP;
            restartCameraPosition = cam.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!(0 < HP)) return;
        if (collision.CompareTag("ResidualAcid")) {
            if (acidDamageTime <= 0) {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var halfX = _sprite.bounds.extents.x;
                var _vec = new Vector3(-halfX, 0f, 0f); // これは左上
                var _unvec = new Vector3(halfX, 0f, 0f); // これは右上
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.x >= _pos.x && transform.position.x <= _unpos.x) {
                    acidDamageTime += _acidDamageRate;
                    Damage(_acidDamage, true);
                    SoundManagerV2.Instance.PlaySE(4);
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                }
            }
        } else if(collision.CompareTag("WallReAcid")) {
            if (acidDamageTime <= 0) {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(0f, -_halfY, 0f); // これは上
                var _unvec = new Vector3(0f, _halfY, 0f); // これは下
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.y >= _pos.y && transform.position.y <= _unpos.y) {
                    acidDamageTime += _acidDamageRate;
                    Damage(_acidDamage, true);
                    SoundManagerV2.Instance.PlaySE(4);
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                }
            }
        }
    }

    // 以下アニメーションに関する
    void SetState(State state, bool first = false){  //アニメーションレイヤーのステート
        this.state = state;
        //　最初の設定でなければスムーズフラグをオンにする
        if (!first)
        {
            smoothFlag = true;
        }
    }

    public void AnimStop()
    {
        anicount = 0.0f;
        animator.SetBool("Stand", true);
        animator.SetBool("Wait", false);
        animator.SetBool("Run", false);
        animator.SetBool("JumpStart", false);
        animator.SetBool("JumpStart-run", false);
        animator.SetBool("JumpUp-run", false);
        animator.SetBool("JumpUp", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("JumpEnd", false);
        rb.velocity = Vector2.zero;
        
    }


    // リスタート時の処理
    private IEnumerator Restart(float interval)
    {
        yield return new WaitForSeconds(interval);
        // リトライ回数足す
        sm.RetryCnt++;
        sm.TotalRetryCnt++;

        // startで取得しておいたリスタートポイントたちのメソッド実行
        foreach (var rps in restartPoints)
        {
            RestartPoint rp = rps.GetComponent<RestartPoint>();
            rp.TurnOnSpawner();
        }

        // 位置を最後に通ったリスタートポイントの位置に転送、カメラもその位置に。
        transform.position = restartPosition;
        cam.transform.position = restartCameraPosition;

        // リジッドボディの加速度もゼロにする
        rb.velocity = Vector2.zero;

        // HPをリスタートポイント通過時のHPに戻し、HPバーのvalueにその値を代入する。
        _HPbar.GetComponent<PlayerHPbarScript>().isStartFunctionCalledAfter = false;
        HP = restartHP;
        _HPbar.value = HP;
        _HPbar.GetComponent<PlayerHPbarScript>().isStartFunctionCalledAfter = true;

        // リスタートを一度でも通ったことがあったら銃をもたせる。
        // でなければ（Stage1でリスタートポイント通過前に死亡）銃未所持状態に戻す
        if (startPosition != restartPosition || isStartGetGun)
        {
            isGetGun = true;
            equipment = Equipment.Handgun;
        }
        else
        {
            isGetGun = false;
            equipment = Equipment.None;
            weight0 = 0f;
            weight1 = 0f;
            weight2 = 0f;
            weight3 = 0f;
            weight4 = 0f;
            SetState(State.None, first: true);
            AnimStop();
            
        }
    }

    public void GetGun()
    {
        if (_isUIDisplay) {
            _handgunUI.gameObject.SetActive(true);
            _handgunUI.Select();
            _bulletsRemain.enabled = true;
        }
            
        isGetGun = true;
        if (state != State.Shot2){
            SetState(State.Shot2);
        }
        SoundManagerV2.Instance.PlaySE(12);
        equipment = Equipment.Handgun;
    }

}