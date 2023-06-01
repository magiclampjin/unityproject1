using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2; //ȸ���Ŀ�
    float scaleValue = 0.1f; //ũ��
    bool isShoot;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTime());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTime() //�⸦ ������ �ð�
    {
        yield return new WaitForSeconds(2.2f); //2.2�� �� ���.
        isShoot = true;
    }

    IEnumerator GainPower() //�⸦ ����
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //while�� ���鼭 ���������� �ӵ��� ���� ���̱� ������ ForceMode.Acceleration ��� 
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
