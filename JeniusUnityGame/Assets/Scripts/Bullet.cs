using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")//탄피가 바닥에 부딪혔을 경우
        {
            Destroy(gameObject, 3); //3초 뒤에 탄피 사라짐.
        }

        if (collision.gameObject.tag == "Wall")//총알이 벽에 부딪혔을 경우
        {
            Destroy(gameObject); //바로 총알 사라짐
        }
    }
}
