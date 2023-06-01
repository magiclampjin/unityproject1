using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2; //회전파워
    float scaleValue = 0.1f; //크기
    bool isShoot;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTime());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTime() //기를 모으는 시간
    {
        yield return new WaitForSeconds(2.2f); //2.2초 후 쏘세요.
        isShoot = true;
    }

    IEnumerator GainPower() //기를 모음
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //while문 돌면서 지속적으로 속도를 높일 것이기 때문에 ForceMode.Acceleration 사용 
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
