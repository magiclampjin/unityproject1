using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour //카메라가 캐릭터를 따라가도록
{
    public Transform target; //따라갈 타켓
    public Vector3 offset; //카메라 위치 고정

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
