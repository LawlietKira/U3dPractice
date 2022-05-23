using UnityEngine;

public class Gravity : MonoBehaviour
{
    
    public float gravity = -10;

    public void Attract(GameObject obj) //吸引物体的方法，传入需要吸引的物体
    {
        // 玩家坐标
        Transform body = obj.GetComponent<Transform>();
        // 星球到玩家的向量
        Vector3 gravityUp = (body.position - transform.position).normalized;
        // 被吸引玩家的正上方
        Vector3 bodyUp = body.up;
        // 吸引物体，重力大小为负数，方向变为物体到星球方向
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
        // 让一个轴从参数一的方向旋转到参数二(世界空间)的方向，这里是让物体上方旋转到重力方向，
        // 但是一个轴无法决定物体的旋转状态，* body.rotation得到物体需要旋转的目标方向
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        // 进行插值，旋转
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
