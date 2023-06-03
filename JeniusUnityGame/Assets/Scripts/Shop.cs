using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; //���� UI
    public Animator anim; //�������� �ִϸ��̼�

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText; //���� ���ڸ� �� ���� text �޽��� ����
    public string[] talkData;
    public AudioSource sellSound;

    Player enterPlayer; //������ ĳ����

    public void Enter(Player player) //���� ����
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //ȭ�� ���߾ӿ� ���� UI ��ġ
    }

    public void Exit() //���� ����
    {
        anim.SetTrigger("doHello"); //�������� �λ� �ִϸ��̼�
        uiGroup.anchoredPosition = Vector3.down * 1000; //���� UI ���� ��ġ��(ȭ�� ��)
    }

    public void Buy(int index) //���� ������ ����, index�� � ������������ ��Ÿ��.
    {
        int price = itemPrice[index]; //�����Ϸ��� �������� ����
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk()); //��ư�� ������ ������ �� �˰����� ���̴� ���� ����
            StartCoroutine(Talk());
            return;
        }
        else if(itemObj[index].name == "Item Heart" && enterPlayer.health == enterPlayer.maxHealth) //ü���� �������ִ� ��� ü�±��� �Ұ���
        {
            return;
        }

        sellSound.Play();
        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3); //���� ��ġ
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
        //������ �������� ������ ��ġ���� ���� ������� (������ġ)�� ����. �� ������ �״���̴�.
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}