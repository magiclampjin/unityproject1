using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f); //3초 뒤에 폭탄 터지도록
        rigid.velocity = Vector3.zero; // 굴러가다가 터지므로 속도가 남아있을텐데, 이 속도도 없애주어야 함.
        rigid.angularVelocity = Vector3.zero; //회전 속도 또한 없애주어여함.
        meshObj.SetActive(false); //보이는 mesh는 빼줘야함.
        effectObj.SetActive(true);

        //부피가 있는 raycast -> SphereCast //범위에 걸린 모든 적들에게 수류탄 피격을 주어야하므로 SphereCastAll 이용.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,  
                                                    15 /*반지름*/, 
                                                    Vector3.up /*쏘는 방향*/, 
                                                    0f /*ray쏘는길이, 5로 설정하게 되면 구체 형태를 그대로 위로 쏘아올리는 형태 -> 주변에만 영향을 줄것이니 0으로 설정*/,
                                                    LayerMask.GetMask("Enemy")); 
        //수류탄에 맞은 피격체가 있다면? 신호를 주어야함.
        foreach(RaycastHit hitObj in rayHits){ //배열 안에 있는 raycasthit를 한개씩 가져옴.
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
