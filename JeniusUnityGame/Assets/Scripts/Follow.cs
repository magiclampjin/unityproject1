using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour //ī�޶� ĳ���͸� ���󰡵���
{
    public Transform target; //���� Ÿ��
    public Vector3 offset; //ī�޶� ��ġ ����

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
