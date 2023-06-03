using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Menu panel
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;

    public int stage;
    public float playTime; //상점이용시간, 게임플레이시간 따로 계산
    public bool isBattle; //현재 싸우는 중인지를 판단하는 변수
    public int enemyCntA; //현재 스테이지의 남은 몬스터 수
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;

    //Game panel
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;

    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;

    public Image Weapon1Img;
    public Image Weapon2Img;
    public Image Weapon3Img;
    public Image WeaponRImg;

    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); //Int -> Str 변경 (문자형식 n,nnn)
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
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
        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }
    
}
