using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Menu panel
    public GameObject menuCam;//게임 시작 전 main화면을 비추는 캠
    public GameObject gameCam;//게임 중 화면을 비추는 캠
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;//다음 스테이지로 넘어가는 포탈

    public int stage; //현재 stage 수
    public float playTime; //로비 이용시간, 게임플레이시간 따로 계산
    public bool isBattle; //현재 싸우는 중인지를 판단하는 변수
    public int enemyCntA; //현재 스테이지의 남은 몬스터 수
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones; //몬스터 스폰 위치
    public GameObject[] enemies; //소환할 몬스터 프리펩
    public List<int> enemyList; //소환할 몬스터 정보

    public GameObject menuPanel;//게임 시작화면
    public GameObject gamePanel;//게임 화면
    public GameObject overPanel;//게임오버 화면
    public Text maxScoreTxt;//게임 최고점수 표시할 텍스트

    //Game panel
    public Text scoreTxt;//게임 점수 표시할 텍스트
    public Text stageTxt;//게임 스테이지 정보 표시할 텍스트
    public Text playTimeTxt;//게임 플레이타임 표시할 텍스트

    public Text playerHealthTxt;//플레이어 체력 표시할 텍스트
    public Text playerAmmoTxt;//플레이어 탄약 수 표시할 텍스트
    public Text playerCoinTxt;//플레이어 코인 수 표시할 텍스트

    public Image Weapon1Img;//무기소지 시 게임화면에 표시하기 위한 이미지
    public Image Weapon2Img;
    public Image Weapon3Img;
    public Image WeaponRImg;

    public Text enemyATxt;//남은 몬스터 수 표시할 텍스트
    public Text enemyBTxt;
    public Text enemyCTxt;

    public RectTransform bossHealthGroup;//보스 출몰 시 화면 상단에 보스 체력을 표시하기 위함.
    public RectTransform bossHealthBar;
    public Text curScoreTxt;//게임오버 화면에서 현재 점수를 표시할 텍스트
    public Text bestTxt;//게임오버 화면에서 현재 점수가 최고 점수이면 best 출력

    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore",0)); //PlayerPrefs 유니티에서 제공하는 간단한 저장 기능, 최고점수 기록, Int -> Str 변경 (문자형식 n,nnn), 없으면 0 반환 
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreTxt.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0); //scene이 1개뿐이라서 0으로 입력해도 무방함.
    }

    //스테이지 시작, 종료 함수
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones) //게임시작 시 몬스터 스폰존 모두 활성화
            zone.gameObject.SetActive(true); 

        isBattle = true;
        StartCoroutine(InBattle());
    }
    
    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones) //게임종료 시 몬스터 스폰존 모두 비활성화
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0 && stage != 0) //stage 5단위마다 보스소환
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int idx = 0; idx < stage; idx++)
            {
                int ran = Random.Range(0, 3); //소환할 몬스터
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ran = Random.Range(0, 4); //소환할 스폰 위치
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ran].position, enemyZones[ran].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemyList.RemoveAt(0); //첫번째 List 항목 지움.
                enemy.manager = this;
                yield return new WaitForSeconds(5f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) //몬스터가 1마리라도 남아있으면 계속 진행
        {
            yield return null; // while()문에 yield return null;를 넣으면 하나의 프레임으로 생각 -> update()와 유사한 기능을 수행.
        }

        //몬스터를 모두 죽이면 4초 후 스테이지 종료
        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    public void Update()
    {
        if (isBattle) //각 프레임이 소비한 시간 = deltTime
        {
            playTime += Time.deltaTime;
        }
    }

    //매프레임마다 정보를 받아가면서 UI에 등록
    public void LateUpdate() //LateUpdate(): Update()가 끝난 후 호출되는 함수(생명주기)
    {

        //상단UI
        scoreTxt.text = string.Format("{0:n0}", player.score); //현재 점수
        stageTxt.text = "STAGE " + stage; //현재 스테이지

        //소수점으로 계산된 플레이시간을 00:00:00의 형식에 맞게 변경
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second); //현재 플레이시간

        //플레이어 UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth; //현재 체력
        playerCoinTxt.text = string.Format("{0:n0}", player.coin); //현재 코인

        //현재 탄약
        if(player.equipWeapon == null) //플레이어에게 무기가 없는 경우
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else if(player.equipWeapon.type == Weapon.Type.Melee) //해머를 든 경우 탄약 수 0으로 표기
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else
        { // 장전된 탄약수 / 소지하고 있는 총 탄약수 표기
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }

        //무기 UI
        //무기 아이콘은 보유 상황에 따라 알파값만 변경
        Weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        Weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        Weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        WeaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //몬스터 관련 UI
        //현재 스테이지에 남아있는 몬스터 수
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //Boss 체력
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200; //Boss가 없을 때는 boss 체력창 없애기
        }           
    }   
}
