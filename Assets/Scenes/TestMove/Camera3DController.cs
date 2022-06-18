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
    [LabelText("相机距离")]
    public float cameraDistance = 7f;

    [LabelText("滚轮变化率")]
    public float deltaDistance = 0.1f;
    [LabelText("最大视角距离")]
    public float maxCamareDistance = 10f;
    [LabelText("最小视角距离")]
    public float minCamareDistance = 2f;

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
    public int cameraLeftRightCoefficient = 1;
    [LabelText("鼠标上下灵敏度")]
    public int cameraUnDownCoefficient = 1;
    [LabelText("鼠标上下移动偏差系数")]
    public float cameraUnDownOffset = 0.6f;

    [LabelText("地面")]
    public GameObject ground;
    private Rigidbody myRigidbody;
    private PlayerController playerController;
    private float currentLerp = 1;
    private Vector3 groundScale;
    private Vector3 localCameraGroundPosition;

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

        groundScale = ground.transform.localScale;
        groundScale.y = 0;

        Cursor.visible = false; //隐藏鼠标指针
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        UpdateCameraPosition();
    }
    private void Update()
    {
        // 更新相机的旋转
        UpdateCameraManagerRotation();
        UpdateDistance();
        DrawLine();
    }

    private void LateUpdate()
    {
        
        //Debug.Log("Player:" + Player.transform.position);
    }

    private Vector3 GetMouseMove()
    {
        return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    /// <summary>
    /// 画网格线
    /// </summary>
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
        // 角色画线
        Debug.DrawLine(cameraLookUpTarget.transform.position + Vector3.down, cameraLookUpTarget.transform.position + Vector3.up, Color.red);
        // 相机画线
        //Debug.DrawLine(camera.transform.position, camera.transform.position + camera.transform.forward * baseDistance, Color.red);
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
    /// 更新相机追随角色的位置
    /// </summary>
    void UpdateCameraManagerPosition ()
    {
        float a = 1 + (maxCamareDistance - cameraDistance) / (maxCamareDistance - minCamareDistance);

        transform.localPosition = Vector3.Lerp(transform.localPosition, Player.transform.localPosition, currentLerp * a);

        //Debug.Log(currentLerp);
    }

    /// <summary>
    /// 相机视角控制
    /// </summary>
    void UpdateCameraPosition()
    {
        // 更新相机Lerp数据
        UpdateCameraLerp();
        // 更新相机的位移
        UpdateCameraManagerPosition();
        //// 更新相机的旋转
        //UpdateCameraManagerRotation();
        //camera.transform.position = Vector3.Lerp(camera.transform.position, Player.transform.position + cameraOffset, currentLerp);
    }

    /// <summary>
    /// 更新相机的旋转角度
    /// </summary>
    void UpdateCameraManagerRotation()
    {
        //_currentPosition = GetMouseMove();
        Vector3 diff = GetMouseMove();
        // 相机左右旋转
        transform.Rotate(new Vector3(0, UpdateCameraRotate(diff.x), 0));
        // 判断相机与玩家夹角，控制仰角俯角的最大限制
        UpdateCameraRotateX(diff.y);
        Player.transform.rotation = transform.rotation;
    }

    /// <summary>
    /// 单次移动限制，限制鼠标移出移入后的最大旋转角度
    /// </summary>
    /// <param name="defaultMove"></param>
    /// <returns></returns>
    private float UpdateCameraRotate (float defaultMove)
    {
        return Reduce(defaultMove) * (cameraLeftRightCoefficient + 5) / 10;
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
        localCameraGroundPosition = transform.InverseTransformPoint(ground.transform.position);

        Vector3 rotate = new Vector3(-Reduce(defaultMoveY) * (cameraUnDownCoefficient + 5) / 10, 0, 0);
        // 计算相机方向，与角色头的夹角
        float v = Vector3.Angle(camera.transform.forward, Player.transform.up);
        if (v < 15)
        {
            if (rotate.x > 0)
                camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        } else if (v > 175)
        {
            if (rotate.x < 0)
                camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        }
        else
        {
            camera.transform.Rotate(rotate.x, rotate.y, rotate.z);
        }
        // 鼠标上下移动时位置偏移
        Vector3 currentCameraPosition = cameraLookUpTarget.transform.localPosition + transform.InverseTransformVector(-camera.transform.forward).normalized * cameraDistance;
        if (currentCameraPosition.y < localCameraGroundPosition.y)
        {
            currentCameraPosition = currentCameraPosition * localCameraGroundPosition.y / currentCameraPosition.y;
        }
        camera.transform.localPosition = currentCameraPosition;
    }

    /// <summary>
    /// 更新滚轮距离
    /// </summary>
    private void UpdateDistance()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            
            cameraDistance = Mathf.Lerp(cameraDistance, cameraDistance + deltaDistance, 0.1f);
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cameraDistance = Mathf.Lerp(cameraDistance, cameraDistance - deltaDistance, 0.1f);
        }

        if (cameraDistance > maxCamareDistance)
        {
            cameraDistance = maxCamareDistance;
        } else if (cameraDistance < minCamareDistance)
        {
            cameraDistance = minCamareDistance;
        }
    }
}
