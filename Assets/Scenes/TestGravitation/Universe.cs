using Sirenix.OdinInspector;
using UnityEngine;

public class Universe : MonoBehaviour
{
    [Header("标准值")]
    [LabelText("行星")]
    public Planet[] planets;
    [LabelText("万有引力常量")]
    public float _universalGravitation = 6.67408e-11f;
    [Header("缩放值")]
    [LabelText("重量缩放")]
    public float _weightScale;
    [LabelText("距离缩放")]
    public float _distanceScale;
    [LabelText("时间缩放，天/s")]
    public float _timeScale;

    //public float timeChange;
    //private float _timeChange;

    private float lastTiemScale;

    private int oneDaySecend = 24 * 60 * 60;

    private float timeScale;

    public float radiusScale = 5f;

    private float universalGravitation;

    public float TimeScale { get => timeScale; set => timeScale = value; }
    public float UniversalGravitation { get => universalGravitation; set => universalGravitation = value; }

    private void Awake()
    {
        lastTiemScale = _timeScale;
        //_timeChange = timeChange;
        timeScale = _timeScale * oneDaySecend;
        // 初始化万有引力
        universalGravitation = _universalGravitation * Mathf.Pow(_distanceScale, 3) / _weightScale;
        foreach (Planet planet in planets)
        {
            Rigidbody rigidbody = planet.transform.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            planet.weight *= _weightScale;
            //planet.radius = planet.radius * radiusScale;
            planet.corotationRadius *= _distanceScale;
            planet.Universe = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeScale != lastTiemScale)
        {
            foreach (Planet planet in planets)
            {
                planet.SetVelocity();
            }
            
            timeScale = _timeScale * oneDaySecend;
            lastTiemScale = _timeScale;
            foreach (Planet planet in planets)
            {
                planet.TimeScaleUpdate();
            }
        }
    }

}
