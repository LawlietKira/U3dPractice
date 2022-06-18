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

    // 当前受力
    private Vector3 currentForce = Vector3.zero;
    // 所处宇宙
    private Universe universe;

    private Planet[] otherPlanets;
    // 速度方向
    private Vector3 moveDir = Vector3.zero;

    public Universe Universe { get => universe; set => universe = value; }

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
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = Vector3.one * radius * universe.radiusScale * 0.00001f;
        CalculatePlanetForce();
        
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
        rigidbody.AddForce(currentForce * Mathf.Pow(universe.TimeScale, 2));
        
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
}
