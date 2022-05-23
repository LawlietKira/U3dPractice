using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Keyboard : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var key = Input.GetAxisRaw("Jump");

           Debug.LogWarning(key);
    }
}
