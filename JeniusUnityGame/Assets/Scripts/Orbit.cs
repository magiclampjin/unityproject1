using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //수류탄이 공전할 중심
    public float orbitSpeed; //공전할 속도
    Vector3 offset; //플레이어와 수류탄 사이 거리(고정값)

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
        //offset은 현재 수류탄 위치에서 타겟 위치를 뺀 값.
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset; //target.position이 계속 변화함. RotateAround는 목표가 움직이면 일그러지는 단점이 있음.  -> 이를 보완
        //직접 update에 적용.

        //대상을 주위로 회전하는 함수는 RotateAround
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); // 두번째 매개변수는 회전축, 세번째 매개변수는 회전하는 수치
        offset = transform.position - target.position; //또 이동을 했을테니 offset 다시 업데이트
    }
}
