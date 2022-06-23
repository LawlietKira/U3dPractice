using Sirenix.OdinInspector;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [LabelText("行星名字")]
    public string planetName;
    [LabelText("重量")]
    public float weight;
    [LabelText("星球半径")]
    public float radius;
    [LabelText("公转半径")]
    public float corotationRadius;

    [Header("卫星")]
    [LabelText("所拥有的卫星")]
    public Planet[] secondaryPlanets;
    [LabelText("是否为卫星")]
    public bool isSecondaryPlanet = false;

    private new Rigidbody rigidbody;

    [LabelText("太阳引力")]
    public float gravitation;
    [LabelText("发动机推力")]
    public float thrust;
    [LabelText("离日距离")]
    public float distance;
    [LabelText("最大距离")]
    public float maxDistance;
    [LabelText("最小距离")]
    public float minDistance;
    [LabelText("最大速度")]
    public float maxSpeed;
    [LabelText("最小速度")]
    public float minSpeed;
    [LabelText("速度变化率")]
    public float speedRatio;
    private float lastSpeed;

    // 当前受力
    private Vector3 currentForce = Vector3.zero;
    // 所处宇宙
    private Universe universe;

    private Planet[] otherPlanets;
    // 速度方向
    private Vector3 moveDir = Vector3.zero;
    // 引擎推力
    private Vector3 thrustDir;

    public float speed;

    public Universe Universe { get => universe; set => universe = value; }
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }
    public Vector3 ThrustDir { get => thrustDir; set => thrustDir = value; }

    private void Awake()
    {
        // 行星半径初始化
        transform.localScale = Vector3.one * radius;
        // 所处宇宙，拿到对应常量系数
        universe = transform.GetComponentInParent<Universe>();
        // 其他相互影响星球
        otherPlanets = universe.planets;
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.mass = weight;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isSecondaryPlanet)
        {
            // 如果不是卫星，以太阳为中心旋转
            InitPosition(otherPlanets[0]);
            InitVelocity(otherPlanets[0]);
            foreach (Planet secPlanet in secondaryPlanets)
            {
                // 如果是卫星，以当前星球为中心
                //secPlanet.corotationRadius *= 2;
                secPlanet.InitPosition(this);
                secPlanet.InitVelocity(this);
            }
            maxSpeed = minSpeed = rigidbody.velocity.magnitude / universe.TimeScale / universe._distanceScale;
            distance = maxDistance = minDistance = (transform.position - otherPlanets[0].transform.position).magnitude / universe._distanceScale / 1000;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = Vector3.one * radius * universe.radiusScale * 0.00001f;
        CalculatePlanetForce();
        speed = rigidbody.velocity.magnitude / universe.TimeScale / universe._distanceScale;
        speedRatio = (speed - lastSpeed) / speed * 1000;
        lastSpeed = speed;
        UpdateSpeed();
        distance = (transform.position - otherPlanets[0].transform.position).magnitude / universe._distanceScale / 1000;
    }


    public void TimeScaleUpdate ()
    {
        rigidbody.velocity = moveDir * universe.TimeScale;
    }

    public void SetVelocity()
    {
        moveDir = rigidbody.velocity / universe.TimeScale;
        
    }

    public void CalculatePlanetForce()
    {
        currentForce = Vector3.zero;
        foreach (Planet p in otherPlanets)
        {
            CalculatePlanetForceBy2Planet(this, p);
        }
        if (name.Equals("Earth"))
        {
            gravitation = currentForce.magnitude;
            thrust = ThrustDir.magnitude;
        }
        rigidbody.AddForce((currentForce + ThrustDir) * Mathf.Pow(universe.TimeScale, 2));
    }

    public void CalculatePlanetForceBy2Planet(Planet p1, Planet p2)
    {
        if (!p1.Equals(p2))
        {
            Vector3 dir = p2.transform.localPosition - p1.transform.localPosition;
            
            float f = universe.UniversalGravitation * p1.weight * p2.weight / dir.sqrMagnitude;
            currentForce += dir.normalized * f;
        }
    }

    /// <summary>
    /// 初始化位置
    /// </summary>
    /// <param name="targetPlanet"></param>
    void InitPosition (Planet targetPlanet)
    {
        transform.localPosition = targetPlanet.transform.localPosition + new Vector3(corotationRadius, 0, 0);
    }

    /// <summary>
    /// 以planet为中心，进行速度初始化
    /// </summary>
    /// <param name="targetPlanet"></param>
    void InitVelocity(Planet targetPlanet)
    {
        if (targetPlanet != this)
        {
            Vector3 initMove = Vector3.Cross(targetPlanet.transform.localPosition - transform.localPosition, Vector3.down).normalized * Mathf.Sqrt(universe.UniversalGravitation * targetPlanet.weight / corotationRadius);
            moveDir = initMove + targetPlanet.moveDir;
            rigidbody.velocity = moveDir * universe.TimeScale;
        }
    }

    void UpdateSpeed()
    {
        minSpeed = Mathf.Min(minSpeed, speed);
        maxSpeed = Mathf.Max(maxSpeed, speed);
        minDistance = Mathf.Min(minDistance, distance);
        maxDistance = Mathf.Max(maxDistance, distance);
    }
}
