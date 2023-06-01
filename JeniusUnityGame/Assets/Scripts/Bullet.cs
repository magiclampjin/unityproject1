using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")//ź�ǰ� �ٴڿ� �ε����� ���
        {
            Destroy(gameObject, 3); //3�� �ڿ� ź�� �����.
        }

        if (collision.gameObject.tag == "Wall")//�Ѿ��� ���� �ε����� ���
        {
            Destroy(gameObject); //�ٷ� �Ѿ� �����
        }
    }
}
