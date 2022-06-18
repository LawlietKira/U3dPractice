using Sirenix.OdinInspector;
using UnityEngine;

public class SecondaryPlanet : MonoBehaviour
{
    [LabelText("卫星")]
    public Planet[] secondaryPlanet;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Planet planet in secondaryPlanet)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
