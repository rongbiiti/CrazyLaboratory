using UnityEngine;
using System.Collections;

/// <summary>
/// カメラが少し遅れてプレイヤーに追従するようにしている。
/// Y軸はキャラが画面上の一定の位置に来たときに追従している。
/// また、Y軸を強制的に補正させるトリガーに侵入したときは
/// その補正を追加で行っている。
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField, CustomLabel("カメラ移動速度")] private float _cameraSpeed = 5.0f;
    [SerializeField, CustomLabel("ポーズメニュー")] private GameObject _pauseMenu;
    [SerializeField, CustomLabel("ステージ左端のX座標")] private float _stage_edge_x;
    [SerializeField, CustomLabel("ステージ右端のX座標")] private float _stage_edge_x_right;
    [SerializeField, CustomLabel("チートON")] private bool _isCheatEnable;
    private GameObject player;
    private PlayerController pc;
    private Camera cam;
    private Rigidbody2D prb;
    private Vector3 offset = Vector3.zero;
    private bool isFloarChange;
    private bool isFocasUnder;
    private float YAxisFixTime;
    private float setYAxisFixTime = 1f;
    private float focasOffset;
    private Vector2 zoomPreviousPosition;    // ズーム前位置
    private float zoomPreviousSize;     // ズーム前サイズ
    private Vector2 zoomPoint;      // ズーム用位置
    private float zoomSizeTarget;   // ズーム時目標サイズ
    private float zoomtime;     // ズームに使う秒数
    private bool isZoom;        // ズーム中か
    private float startTime;     // ズーム中に使う
    private float playerMoveTimeCount;

    private void Awake()
    {
        if (!_isCheatEnable)
        {
            Destroy(GetComponent<CheatMenu>());
        }
        if (!GetComponent<CameraShake>()) {
            gameObject.AddComponent<CameraShake>();
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
        cam = GetComponent<Camera>();
        pc = player.GetComponent<PlayerController>();
        prb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space)) {
        //    GetComponent<RadialBlurSc>().RadialBlur(1.5f, 0.3f);
        //}
        
    }

    private void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        Vector3 viewPos = cam.WorldToViewportPoint(player.transform.position);
        if (viewPos.y > 0.75f && !isFocasUnder) {
            newPosition.y = player.transform.position.y - offset.y;
        } else if ((viewPos.y < 0.25f && !isFocasUnder) || pc.IsGhost) {
            newPosition.y = player.transform.position.y + offset.y;
        }

        if(prb.velocity.x < -2.1f || 2.1f < prb.velocity.x) {
            playerMoveTimeCount += Time.deltaTime;
        } else {
            playerMoveTimeCount = 0;
            offset.x = 0;
        }

        // Xがステージの端より内側だったらプレイヤーのX座標を追いかける
        if ((_stage_edge_x <= player.transform.position.x && _stage_edge_x_right >= player.transform.position.x)
            || (_stage_edge_x <= transform.position.x && _stage_edge_x_right >= transform.position.x))
        {
            if(0.2f < playerMoveTimeCount || viewPos.x < 0.47f || 0.53f < viewPos.x)
            newPosition.x = player.transform.position.x + offset.x;
        }
        
        newPosition.z = player.transform.position.z + offset.z;
        if (isZoom) {
            newPosition.x = zoomPoint.x;
            newPosition.y = zoomPoint.y;
            float diff = Time.timeSinceLevelLoad - startTime;
            float rate = diff / zoomtime;
            transform.position = Vector3.Lerp(transform.position, newPosition, rate);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSizeTarget, rate);
        } else {
            if (!pc.IsGhost) {
                transform.position = Vector3.MoveTowards(transform.position, newPosition, 18 * Time.deltaTime);
            } else {
                newPosition.x = player.transform.position.x + offset.x;
                newPosition.y = player.transform.position.y + offset.y;
                newPosition.z = player.transform.position.z + offset.z;
                transform.position = Vector3.Lerp(transform.position, newPosition, 6f * Time.deltaTime);
            }
        }

        
        if (isFloarChange) {
            FloorChange(newPosition);
        }

        if (isFocasUnder)
        {
            FocusUnder(newPosition);
        }
    }

    private void FloorChange(Vector3 newPosition)
    {
        YAxisFixTime -= Time.deltaTime;
        var position = player.transform.position;
        newPosition.y = position.y + offset.y;
        newPosition.z = position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 3f * Time.deltaTime);
        if(YAxisFixTime <= 0f) {
            isFloarChange = false;
        }
    }

    private void FocusUnder(Vector3 newPosition)
    {
        var position = player.transform.position;
        newPosition.y = position.y - focasOffset;
        newPosition.z = position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 2.5f * Time.deltaTime);
    }

    public void SetIsFloorChange()
    {
        if (isFloarChange) return;
        isFloarChange = true;
        YAxisFixTime = setYAxisFixTime;
    }

    public void SetIsFocusUnder(bool flag, float offset = 0f)
    {
        isFocasUnder = flag;
        focasOffset = offset;
    }

    public void EventCamera(Vector2 position, float zoomSize, float zoomTime)
    {
        zoomPreviousPosition = transform.position;
        zoomPreviousSize = cam.orthographicSize;
        zoomPoint = position;
        zoomSizeTarget = zoomSize;
        zoomtime = zoomTime;
        isZoom = true;
        startTime = Time.timeSinceLevelLoad;
    }

    public void EventCameraEnd(float zoomTime)
    {
        zoomPoint = zoomPreviousPosition;
        zoomSizeTarget = zoomPreviousSize;
        zoomtime = zoomTime;
        StartCoroutine(ZoomFlagFalse(zoomTime));
        startTime = Time.timeSinceLevelLoad;
    }

    private IEnumerator ZoomFlagFalse(float falseTime)
    {
        yield return new WaitForSeconds(falseTime);
        isZoom = false;
    }
}