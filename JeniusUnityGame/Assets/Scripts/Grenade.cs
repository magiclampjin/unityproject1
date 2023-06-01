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
        yield return new WaitForSeconds(3f); //3�� �ڿ� ��ź ��������
        rigid.velocity = Vector3.zero; // �������ٰ� �����Ƿ� �ӵ��� ���������ٵ�, �� �ӵ��� �����־�� ��.
        rigid.angularVelocity = Vector3.zero; //ȸ�� �ӵ� ���� �����־��.
        meshObj.SetActive(false); //���̴� mesh�� �������.
        effectObj.SetActive(true);

        //���ǰ� �ִ� raycast -> SphereCast //������ �ɸ� ��� ���鿡�� ����ź �ǰ��� �־���ϹǷ� SphereCastAll �̿�.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,  
                                                    15 /*������*/, 
                                                    Vector3.up /*��� ����*/, 
                                                    0f /*ray��±���, 5�� �����ϰ� �Ǹ� ��ü ���¸� �״�� ���� ��ƿø��� ���� -> �ֺ����� ������ �ٰ��̴� 0���� ����*/,
                                                    LayerMask.GetMask("Enemy")); 
        //����ź�� ���� �ǰ�ü�� �ִٸ�? ��ȣ�� �־����.
        foreach(RaycastHit hitObj in rayHits){ //�迭 �ȿ� �ִ� raycasthit�� �Ѱ��� ������.
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
