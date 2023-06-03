using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Ammo,Coin,Grenade, Heart, Weapon}; //������ Ÿ�� enum (�׳� int��, float�� ���� Ÿ����)
    public Type type; //������ ���� ����
    public int value; //������ ����

    Rigidbody rigid;
    SphereCollider sphereCollider;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>(); //GetComponent()�Լ��� ������Ʈ�� �������̸� ù��° ������Ʈ�� ������. (���� ���� �ִ� ������Ʈ)
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //������ ���ڸ� ȸ��(���ڰ�)  
    }

    //�ѵ� �̺�Ʈ
    void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Floor")//�������� �����ǰ� �ٴڿ� ��Ҵ�.
        {
            rigid.isKinematic = true; //rigidbody�� ���̻� �ܺ� ����ȿ���� ���� �������� ���ϵ��� ����
            sphereCollider.enabled = false;
        }
    }
}
