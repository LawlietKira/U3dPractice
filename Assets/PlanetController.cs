using Sirenix.OdinInspector;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [LabelText("实际发动机推力")]
    public float engineThrust;
    [LabelText("发动机推数量")]
    public int engineNumber;
    [LabelText("发动机启动")]
    public bool startThrust;
    [LabelText("发动机速度方向")]
    public bool reverse;

    private Planet planet;
    private Universe universe;
    // 缩放后的推力
    private float _engineThrust;


    // Start is called before the first frame update
    private void Start()
    {
        planet = transform.GetComponent<Planet>();
        universe = planet.Universe;
        _engineThrust = engineThrust * universe._weightScale * universe._distanceScale;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController();
    }

    private void PlayerController()
    {
        if (startThrust)
        {
            PlanetThrust();
        } else {
            planet.ThrustDir = Vector3.zero;
        }
        //Debug.Log(planet.ThrustDir.magnitude);
    }

    void PlanetThrust()
    {
        //planet.ThrustDir = (planet.transform.position - universe.planets[0].transform.position).normalized * _engineThrust * engineNumber;
        planet.ThrustDir = planet.Rigidbody.velocity.normalized * _engineThrust * engineNumber * (reverse ? 1 : -1);
    }
}
