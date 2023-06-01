using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //회전하면서 나아가도록
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
