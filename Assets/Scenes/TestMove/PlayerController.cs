using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController: MonoBehaviour
{

    [LabelText("移动速度")]
    public float moveSpeed = 15f;
    [LabelText("斜向移动速率")]
    public float standardValue = 1f;
    [LabelText("移动时速度切换lerp")]
    public float speedLerp = 0.1f;
    [LabelText("停止时速度切换lerp")]
    public float speedStopLerp = 0.3f;
    [LabelText("奔跑转向是否停止奔跑")]
    public bool isRunningSwerveStop = true;
    [LabelText("冲刺转向是否停止冲刺")]
    public bool isSprintSwerveStop = false;

    [Header("加速度")]
    [LabelText("冲刺加速系数")]
    public float sprintSpeedCoefficient = 2f;
    [LabelText("冲刺加速时长")]
    public float sprintSpeedTime = 1f;
    [LabelText("奔跑速度系数")]
    public float maxSpeed = 1.5f;
    /// <summary>
    /// 基础移动速率 = 1
    /// </summary>
    private float baseSpeedCoefficient = 1f;
    [LabelText("冲刺速度Lerp")]
    public float sprintSpeedLerp = 0.01f;
    [LabelText("奔跑速度Lerp")]
    public float runningSpeedLerp = 0.1f;
    [LabelText("移动速度Lerp")]
    public float moveSpeedLerp = 0.1f;
    [LabelText("停止时速度Lerp")]
    public float stopMoveSpeedLerp = 0.1f;

    [Header("转向Lerp")]
    [LabelText("冲刺转向Lerp")]
    public float sprintSwerveLerp = 0.01f;
    [LabelText("奔跑转向Lerp")]
    public float runningSwerveLerp = 0.01f;
    [LabelText("行走转向Lerp")]
    public float moveSwerveLerp = 0.01f;
    [LabelText("转向所需时长")]
    public float swervingTime = 0.2f;
    [LabelText("冲刺完成是否继续奔跑")]
    public bool sprintRunningKeep = false;

    /// <summary>
    /// 前进方向【W】
    /// </summary>
    private Vector3 forward = new Vector3(0, 0, 1);
    /// <summary>
    /// 是否正在转向
    /// </summary>
    private bool isSwerving = false;
    /// <summary>
    /// 开始转向时间
    /// </summary>
    private float startSwerving;
    /// <summary>
    /// 加速开始时间
    /// </summary>
    private float momentSpeedStartTime;
    private const string LeftShift = "Fire3";
    /// <summary>
    /// 当前移动方向
    /// </summary>
    private Vector3 moveDir;
    /// <summary>
    /// 上次移动方向
    /// </summary>
    private Vector3 lastMoveDir = new Vector3();
    private new Rigidbody rigidbody;
    /// <summary>
    /// 当前冲刺速度系数，默认为1
    /// </summary>
    private float currentSpeedCoefficient = 1;
    /// <summary>
    /// 是否正在移动
    /// </summary>
    private bool isMove = false;
    /// <summary>
    /// 是否正在奔跑
    /// </summary>
    private bool isRunning = false;
    /// <summary>
    /// 是否正在冲刺
    /// </summary>
    private bool isSprint = false;
    /// <summary>
    /// 当前速度切换lerp
    /// </summary>
    private float currentSpeedLerp = 0.1f;
    /// <summary>
    /// 角色状态
    /// </summary>
    private int playerStatus = PlayerControllerConstant.StopStatus;

    public int PlayerStatus { get => playerStatus; set => playerStatus = value; }

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        UpdateSpeedUp();
        UpdateMoveDir();
        UpdatePlayerStatus();
    }
    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 更新移动速度
    /// </summary>
    private void UpdateMoveDir()
    {
        float standard = 1;
        Vector3 curDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (!isSwerving && Vector3.Dot(curDir, lastMoveDir) < 0)
        {
            isSwerving = true;
            startSwerving = Time.time;
            Debug.Log("正在转向");
            // 转向停止冲刺
            isSprint = isSprint && !isSprintSwerveStop;
            // 转向停止奔跑
            isRunning = isRunning && !isRunningSwerveStop;
        }
        if(curDir.sqrMagnitude == 0 && !Input.GetButton(LeftShift))
        {
            isMove = isRunning = false;
        }
        else if (curDir.sqrMagnitude == 1f)
        {
            isMove = true;
            currentSpeedLerp = speedLerp;
        } else
        {
            isMove = true;
            standard = standardValue;
            currentSpeedLerp = speedLerp;
        }
        if (isSwerving)
        {
            if (isSprint)
            {
                currentSpeedLerp = sprintSwerveLerp;
            } else if (isRunning)
            {
                currentSpeedLerp = runningSwerveLerp;
            } else if (isMove)
            {
                currentSpeedLerp = moveSwerveLerp;
            } else
            {
                currentSpeedLerp = speedStopLerp;
            }
            if (Time.time - startSwerving > swervingTime)
            {
                // 转向结束
                isSwerving = false;
                Debug.Log("转向结束");
            }
        }
        moveDir = Vector3.Lerp(moveDir, curDir.normalized * standard * currentSpeedCoefficient, currentSpeedLerp);
        //Debug.Log("moveDir:" + moveDir);
        lastMoveDir = moveDir;
    }

    void UpdatePlayerStatus ()
    {
        if (isMove)
        {
            playerStatus = PlayerControllerConstant.MoveStatus;
        } else
        {
            playerStatus = PlayerControllerConstant.StopStatus;
        }

        if (isRunning)
        {
            playerStatus = PlayerControllerConstant.RunningStatus;
        }
        if (isSprint)
        {
            playerStatus = PlayerControllerConstant.SprintStatus;
        }
    }
    
    /// <summary>
    /// 更新冲刺加速度
    /// 左Shift加速
    /// </summary>
    void UpdateSpeedUp()
    {
        // 按下左Shift 开始冲刺
        if (Input.GetButtonDown(LeftShift))
        {
            momentSpeedStartTime = Time.time;
            isSprint = isRunning = isMove = true;
        }
        // 松开左Shift 停止冲刺
        if (Input.GetButtonUp(LeftShift))
        {
            // 提前松手，不再奔跑
            if (Time.time - momentSpeedStartTime < sprintSpeedTime)
            {
                isRunning = false;
            } else
            {
                isRunning = isRunning && sprintRunningKeep;
            }
        }

        // 冲刺时间完成
        if (isSprint && Time.time - momentSpeedStartTime > sprintSpeedTime)
        {
            Debug.Log("冲刺正常结束");
            isSprint = false;
            if (Input.GetButton(LeftShift))
            {
                isRunning = true;
            }
            if (moveDir.magnitude == 0)
            {
                isRunning = isMove = false;
            }
        }

        if (isSprint)
        {
            currentSpeedCoefficient = Mathf.Lerp(currentSpeedCoefficient, sprintSpeedCoefficient, sprintSpeedLerp);
        } else if (isRunning)
        {
            currentSpeedCoefficient = Mathf.Lerp(currentSpeedCoefficient, maxSpeed, runningSpeedLerp);
        } else if (isMove)
        {
            currentSpeedCoefficient = Mathf.Lerp(currentSpeedCoefficient, baseSpeedCoefficient, moveSpeedLerp);
        }
        else
        {
            currentSpeedCoefficient = Mathf.Lerp(currentSpeedCoefficient, baseSpeedCoefficient, stopMoveSpeedLerp);
        }
        //LogDetails();
    }

    void LogDetails ()
    {
        //Debug.Log("正在冲刺：" + isRunning);
        //Debug.Log("是否停止加速：" + isSprintStop);
    }
}
