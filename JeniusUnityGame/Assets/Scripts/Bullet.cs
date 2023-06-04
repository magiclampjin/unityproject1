using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee; //���������ΰ�?
    public bool isRock; //BossRock�ΰ�? (���� ������ ź�ǰ� ����� �� ���� ������� �̸� �����ϱ� ���� �÷��� ����

    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor" )//ź�ǰ� �ٴڿ� �ε����� ���
        {
            Destroy(gameObject, 3); //3�� �ڿ� ź�� �����.
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && !isMelee)//�Ѿ��� ���� �ε����� ���
        {
            Destroy(gameObject); //�ٷ� �Ѿ� �����
        }

        else if(gameObject.tag == "Bullet")
        {
            Destroy(gameObject, 10); //����� ��� 10�� �Ŀ� �˾Ƽ� �Ѿ� �����
        }
    }
}
