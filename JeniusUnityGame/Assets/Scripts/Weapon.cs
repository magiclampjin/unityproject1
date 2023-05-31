using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //근접공격 - 망치
    //원거리공격 - 총
    public enum Type { Melee, Range}; //무기종류 type
    public Type type; // 실제 들고있는 무기의 타입
    public int damage; //무기의 공격력
    public float rate; //무기의 공격속도
    public BoxCollider meleeArea; //무기의 공격범위
    public TrailRenderer trailEffect; //무기 휘두르는 효과

    public void Use() //무기사용
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); //같은 코루틴을 새로 시작하기 위해서 현재 동작중인 코루틴을 종료해 로직이 꼬이지 않도록 하는 역할
            StartCoroutine("Swing"); //근접무기면 휘두르기.
        }
    } 

    IEnumerator Swing() //box collider와 trail renderer를 켜고, 일정시간이 지나면 다시 끄기.
    {
        //yield //결과를 전달하는 키워드

        //1프레임
        //yield return null; //1프레임 대기
        //2프레임
        //yield return null; //yield 키워드 여러 개 사용해 시간차 로직 작성 가능 
        //3프레임

        //이런 식으로 프레임 사이에 대기를 함.
        //프레임말고도 사용가능! 0.1초 쉬려면?
        //yield return new WaitForSeconds(0.1f); //주어진 수치(시간)만큼 기다리는 함수
        //중간에 그만두고 싶으면 yield break; 사용 (코루틴 탈출) -> 아래 로직이 더 있으면 아래 로직들이 비활성화됨. 조심히 사용할 것.

        yield return new WaitForSeconds(0.1f); //0.1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

    }

    //box collider와 trail renderer를 켜고, 일정시간이 지나면 다시 끄기.
    //invoke 여러번 대신 코루틴(Co-Routine) 사용.
    // Use()함수를 메인루틴이라고 함.
    // 메인루틴에서 Swing()함수 호출
    // Swing()함수를 서브루틴이라고 함.
    // 서브루틴이 끝나면 다시 메인루틴인 Use()로 돌아가 아래있는 로직을 순차적으로 수행함.

    // 그런데, Swing()함수가 코르틴이라면 조금 다르다.
    // 코루틴 함수: 메인루틴 + 코루틴 (Swing()이 호출되는 순간부터 동시 실행)
    // Use() 메인루틴 + Swing() 코루틴 - 코는 게임할 때 협동하는 게임에서 (Co-op)이라고 부름(함께라는 뜻)
    // 사용방법은? void 지우고 IEnumerator라는 열거형 함수 클래스 사용
    // 코루틴은 yield가 꼭 하나 이상 필요.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
