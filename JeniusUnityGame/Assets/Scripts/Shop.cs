using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; //상점 UI
    public Animator anim; //상점주인 애니메이션

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText; //돈이 모자를 때 상점 text 메시지 수정
    public string[] talkData;
    public AudioSource sellSound;

    Player enterPlayer; //입장한 캐릭터

    public void Enter(Player player) //상점 입장
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //화면 정중앙에 상점 UI 배치
    }

    public void Exit() //상점 퇴장
    {
        anim.SetTrigger("doHello"); //상점주인 인사 애니메이션
        uiGroup.anchoredPosition = Vector3.down * 1000; //상점 UI 원래 위치로(화면 밖)
    }

    public void Buy(int index) //상점 아이템 구매, index는 어떤 아이템인지를 나타냄.
    {
        int price = itemPrice[index]; //구매하려는 아이템의 가격
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk()); //버튼이 여러번 눌렸을 때 알고리즘이 꼬이는 현상 방지
            StartCoroutine(Talk());
            return;
        }
        else if(itemObj[index].name == "Item Heart" && enterPlayer.health == enterPlayer.maxHealth) //체력이 가득차있는 경우 체력구매 불가능
        {
            return;
        }

        sellSound.Play();
        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3); //랜덤 위치
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
        //선택한 아이템을 정해진 위치에서 조금 벗어나도록 (랜덤위치)로 생성. 단 방향은 그대로이다.
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}