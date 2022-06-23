using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Planet lookPlanet;
    public float height;
    public float moveLerp;

    private float scroll = 2;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeight2();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,lookPlanet.transform.position + Vector3.up * height, moveLerp);
    }

    private void UpdateHeight ()
    {
        scroll -= Input.GetAxis("Mouse ScrollWheel") * scroll * 0.5f;
        if (scroll < 5)
        {
            scroll = 5;
        } else if (scroll > 125)
        {
            scroll = 125;
        }
        height = Mathf.Lerp(height, scroll, 0.05f);
    }

    private void UpdateHeight2()
    {
        scroll -= Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 1)
        {
            scroll = 1;
        }
        else if (scroll > 15)
        {
            scroll = 15;
        }
        height = Mathf.Lerp(height, Mathf.Pow(scroll, 2), 0.05f);
    }
}
