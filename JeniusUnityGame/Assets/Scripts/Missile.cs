using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //ȸ���ϸ鼭 ���ư�����
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
