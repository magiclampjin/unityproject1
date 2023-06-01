using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee; //근접공격인가?
    public bool isRock; //BossRock인가? (땅에 닿으면 탄피가 사라질 때 같이 사라져서 이를 방지하기 위한 플래그 변수

    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor" )//탄피가 바닥에 부딪혔을 경우
        {
            Destroy(gameObject, 3); //3초 뒤에 탄피 사라짐.
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && !isMelee)//총알이 벽에 부딪혔을 경우
        {
            Destroy(gameObject); //바로 총알 사라짐
        }

        else if(gameObject.tag == "Bullet")
        {
            Destroy(gameObject, 10); //허공에 쏘면 10초 후에 알아서 총알 사라짐
        }
    }
}
