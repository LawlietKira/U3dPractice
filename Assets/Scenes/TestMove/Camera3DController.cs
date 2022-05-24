using Sirenix.OdinInspector;
using UnityEngine;

public class Camera3DController : MonoBehaviour
{
    [LabelText("角色")]
    public GameObject Player;
    [LabelText("相机")]
    public new Camera camera;
    [LabelText("默认相机偏移")]
    public Vector3 cameraOffset = new Vector3(0, 5, -15);
    [LabelText("默认相机角度旋转")]
    public Vector3 cameraRotate = new Vector3(15, 0, 0);
    [LabelText("相机聚焦目标")]
    public GameObject cameraLookUpTarget;

    [Header("相机位移Lerp")]
    [LabelText("移动时的相机Lerp")]
    public bool useCameraLerp = true;
    [LabelText("移动时的相机Lerp")]
    public float playerMoveLerp = 0.1f;
    [LabelText("奔跑时的相机Lerp")]
    public float playerRunningLerp = 0.1f;
    [LabelText("冲刺时的相机Lerp")]
    public float playerSringtLerp = 0.1f;
    [LabelText("停止时的相机Lerp")]
    public float playerStopLerp = 0.01f;
    [LabelText("相机Lerp变化时的Lerp")]
    public float cameraLerpChange = 0.05f;

    [Header("鼠标灵敏度")]
    [LabelText("鼠标左右灵敏度")]
    public float cameraLeftRightCoefficient = 1;
    [LabelText("鼠标上下灵敏度")]
    public float cameraUnDownCoefficient = 1;


    [LabelText("地面")]
    public GameObject ground;
    private Rigidbody myRigidbody;
    private PlayerController playerController;
    private float currentLerp = 1;
    private Vector3 _lastPoint = new Vector3();
    private Vector3 _currentPosition = new Vector3();
    private Vector3 groundScale;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(camera.transform.forward);
        // 获取物体刚体
        myRigidbody = Player.transform.GetComponent<Rigidbody>();
        // 取消刚体碰撞旋转影响
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        // 取消世界空间重力影响
        //myRigidbody.useGravity = false;

        playerController = Player.transform.GetComponent<PlayerController>();

        currentLerp = playerStopLerp;
        //UpdateCameraPosition();

        transform.localPosition = Player.transform.localPosition;
        _currentPosition = _lastPoint = Input.mousePosition;

        groundScale = ground.transform.localScale;
        groundScale.y = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCameraPosition();
        //UpdateCameraRotation();
    }


    private void Update()
    {
        DrawLine();
    }
    void DrawLine ()
    {
        int s = 10;
        Vector3 position = ground.transform.position;
        for (int i = 0; i < groundScale.x; i++)
        {
            Vector3 startPoint = position - groundScale * s / 2 + new Vector3(0, 0, i * s);
            Debug.DrawLine(startPoint, startPoint + new Vector3(groundScale.x * s, 0, 0), Color.black);
        }
        for (int i = 0; i < groundScale.z; i++)
        {
            Vector3 startPoint = position - groundScale * s / 2 + new Vector3(i * s, 0, 0);
            Debug.DrawLine(startPoint, startPoint + new Vector3(0, 0, groundScale.z * s), Color.black);
        }
        Debug.DrawLine(cameraLookUpTarget.transform.position + Vector3.down, cameraLookUpTarget.transform.position + Vector3.up, Color.red);
    }

    void UpdateCameraLerp()
    {
        if (useCameraLerp)
        {
            switch (playerController.PlayerStatus)
            {
                case PlayerControllerConstant.StopStatus:
                    currentLerp = playerStopLerp;
                    break;
                case PlayerControllerConstant.SprintStatus:
                    currentLerp = Mathf.Lerp(currentLerp, playerSringtLerp, cameraLerpChange);
                    break;
                case PlayerControllerConstant.RunningStatus:
                    currentLerp = Mathf.Lerp(currentLerp, playerRunningLerp, cameraLerpChange);
                    break;
                case PlayerControllerConstant.MoveStatus:
                    currentLerp = Mathf.Lerp(currentLerp, playerMoveLerp, cameraLerpChange);
                    break;
                default:
                    currentLerp = playerStopLerp;
                    break;
            }
        } else
        {
            currentLerp = 1;
        }
        //PlayerControllerConstant.LogStatueCN(playerController.PlayerStatus);
    }

    /// <summary>
    /// 更新相机的位置
    /// </summary>
    void UpdateCameraManagerPosition ()
    {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position, currentLerp);
    }

    /// <summary>
    /// 相机视角控制
    /// </summary>
    void UpdateCameraPosition()
    {
        //Debug.Log(Vector3.Lerp(camera.transform.position, Player.transform.position + cameraOffset, lerp));
        UpdateCameraLerp();
        UpdateCameraManagerPosition();
        UpdateCameraManagerRotation();
        //UpdateCameraRotation();
        //camera.transform.position = Vector3.Lerp(camera.transform.position, Player.transform.position + cameraOffset, currentLerp);
    }

    /// <summary>
    /// 更新相机的旋转角度
    /// </summary>
    void UpdateCameraManagerRotation()
    {
        _currentPosition = Input.mousePosition;
        Vector3 diff = (_currentPosition - _lastPoint);
        transform.Rotate(new Vector3(0, UpdateCameraRotate(diff.x), 0));
        UpdateCameraRotateX(diff.y);
        Player.transform.rotation = transform.rotation;
        _lastPoint = _currentPosition;
    }

    /// <summary>
    /// 单次移动限制，限制鼠标移出移入后的最大旋转角度
    /// </summary>
    /// <param name="defaultMove"></param>
    /// <returns></returns>
    private float UpdateCameraRotate (float defaultMove)
    {
        return Reduce(defaultMove) * (cameraLeftRightCoefficient + 5) / 100;
    }

    private float Reduce (float defaultMove)
    {
        return Mathf.Sign(defaultMove) * Mathf.Min(Mathf.Abs(defaultMove), 20);
    }
    /// <summary>
    /// 判断相机与玩家夹角，控制仰角俯角的最大限制
    /// </summary>
    /// <param name="defaultMoveY"></param>
    private void UpdateCameraRotateX (float defaultMoveY)
    {
        Vector3 rotate = new Vector3(-Reduce(defaultMoveY) * (cameraUnDownCoefficient + 5) / 100, 0, 0);

        float v = Vector3.Angle(camera.transform.forward, Player.transform.up);
        if (v < 10)
        {
            if (rotate.x > 0)
                camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        } else if (v > 160)
        {
            if (rotate.x < 0)
                camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        }
        else
        {
            camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        } 
    }
}
