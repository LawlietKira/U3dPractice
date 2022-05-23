using UnityEngine;

public class Body : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public GravityArrtacter attracter; 
    void Start()
    {
        // 获取物体刚体
        myRigidbody = gameObject.GetComponent<Rigidbody>();
        // 取消刚体碰撞旋转影响
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        // 取消世界空间重力影响
        myRigidbody.useGravity = false;
    }

    private void Update()
    {
        // 传入参数，物体被吸引
        attracter.Attract(gameObject);
    }

}
