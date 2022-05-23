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

    [LabelText("鼠标左右灵敏度")]
    public float cameraLeftRightCoefficient = 1;
    [LabelText("鼠标上下灵敏度")]
    public float cameraUnDownCoefficient = 1;
    private Rigidbody myRigidbody;
    private PlayerController playerController;
    private float currentLerp;
    private Vector3 _lastPoint = new Vector3();
    private Vector3 _currentPosition = new Vector3();
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

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCameraPosition();
        //UpdateCameraRotation();
    }

    void UpdateCameraLerp()
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
        //PlayerControllerConstant.LogStatueCN(playerController.PlayerStatus);
    }

    void UpdateCameraManagerPosition ()
    {
        Vector3 forward = camera.transform.forward;
        Vector3 finalCameraPosition = cameraLookUpTarget.transform.position - forward.normalized * 10;
        //Debug.Log("cameraForward:" + forward + ";" + finalCameraPosition);

        transform.position = Vector3.Lerp(transform.position, Player.transform.position, currentLerp);
        //camera.transform.localPosition = finalCameraPosition;//Vector3.Lerp(camera.transform.localPosition, finalCameraPosition, currentLerp); ;
    }

    void UpdateCameraPosition()
    {
        //Debug.Log(Vector3.Lerp(camera.transform.position, Player.transform.position + cameraOffset, lerp));
        UpdateCameraLerp();
        UpdateCameraManagerPosition();
        UpdateCameraManagerRotation();
        //UpdateCameraRotation();
        //camera.transform.position = Vector3.Lerp(camera.transform.position, Player.transform.position + cameraOffset, currentLerp);
    }

    void UpdateCameraManagerRotation()
    {
        _currentPosition = Input.mousePosition;
        Vector3 diff = (_currentPosition - _lastPoint);
        transform.Rotate(new Vector3(0, UpdateCameraRotate(diff.x), 0));
        UpdateCameraRotateX(diff.y);
        Player.transform.rotation = transform.rotation;
        _lastPoint = _currentPosition;
    }

    private float UpdateCameraRotate (float defaultMove)
    {
        return defaultMove * (cameraLeftRightCoefficient + 10) / 100;
    }


    private void UpdateCameraRotateX (float defaultMoveY)
    {
        Vector3 rotate = new Vector3(-defaultMoveY * (cameraUnDownCoefficient + 10) / 100, 0, 0);

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
