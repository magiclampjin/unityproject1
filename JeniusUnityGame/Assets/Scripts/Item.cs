using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Ammo,Coin,Grenade, Heart, Weapon}; //열거형 타입 enum (그냥 int형, float형 같은 타입임)
    public Type type; //아이템 저장 변수
    public int value; //아이템 개수

    Rigidbody rigid;
    SphereCollider sphereCollider;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>(); //GetComponent()함수는 컴포넌트가 여러개이면 첫번째 컴포넌트만 가져옴. (가장 위에 있는 컴포넌트)
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //아이템 제자리 회전(예쁘게)  
    }

    //총돌 이벤트
    void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Floor")//아이템이 생성되고 바닥에 닿았다.
        {
            rigid.isKinematic = true; //rigidbody가 더이상 외부 물리효과에 의해 움직이지 못하도록 설정
            sphereCollider.enabled = false;
        }
    }
}
