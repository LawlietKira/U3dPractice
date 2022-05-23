using UnityEngine;

public class ViewController : MonoBehaviour
{
    private Quaternion rotation;
    private Vector3 lastPosition;

    private void Start()
    {
        rotation = transform.rotation;
        lastPosition = Input.mousePosition;
    }

    void Update()
    {
        Vector3 curPosition = Input.mousePosition;
        //Debug.LogWarning(Input.mousePosition);
    }
}
