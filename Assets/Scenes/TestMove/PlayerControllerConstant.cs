using System.Collections;
using UnityEngine;

public class PlayerControllerConstant: MonoBehaviour
{
    /// <summary>
    /// 静止状态
    /// </summary>
    public const int StopStatus = 0;
    /// <summary>
    /// 冲刺状态
    /// </summary>
    public const int SprintStatus = 1;
    /// <summary>
    /// 奔跑状态
    /// </summary>
    public const int RunningStatus = 2;
    /// <summary>
    /// 移动状态
    /// </summary>
    public const int MoveStatus = 3;


    public static void LogStatueCN (int status)
    {
        switch (status)
        {
            case StopStatus:
                Debug.Log("停止");
                break;
            case SprintStatus:
                Debug.Log("冲刺");
                break;
            case RunningStatus:
                Debug.Log("奔跑");
                break;
            case MoveStatus:
                Debug.Log("移动");
                break;
            default:
                break;
        }
    }
}
