using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Menu panel
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public int stage;
    public float playTime; //�����̿�ð�, �����÷��̽ð� ���� ���
    public bool isBattle; //���� �ο�� �������� �Ǵ��ϴ� ����
    public int enemyCntA; //���� ���������� ���� ���� ��
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones; //���� ���� ��ġ
    public GameObject[] enemies; //��ȯ�� ���� ������
    public List<int> enemyList; //��ȯ�� ���� ����

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
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
    public Text curScoreTxt;
    public Text bestTxt;

    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); //Int -> Str ���� (�������� n,nnn)

        //PlayerPrefs ����Ƽ���� �����ϴ� ������ ���� ���, �ְ����� ���
        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore",0);
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
        SceneManager.LoadScene(0); //scene�� 1�����̶� 0���� �Է��ص� ������.
    }

    //�������� ����, ���� �Լ�
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones) //���ӽ��� �� ���� ������ ��� Ȱ��ȭ
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

        foreach (Transform zone in enemyZones) //�������� �� ���� ������ ��� ��Ȱ��ȭ
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0 && stage != 0) //stage 5�������� ������ȯ
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
                int ran = Random.Range(0, 3); //��ȯ�� ����
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
                int ran = Random.Range(0, 4); //��ȯ�� ���� ��ġ
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ran].position, enemyZones[ran].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemyList.RemoveAt(0); //ù��° List �׸� ����.
                enemy.manager = this;
                yield return new WaitForSeconds(5f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) //���Ͱ� 1������ ���������� ��� ����
        {
            yield return null; // while()���� yield return null;�� ������ �ϳ��� ���������� ���� -> update()�� ������ ����� ����.
        }

        //���͸� ��� ���̸� 4�� �� �������� ����
        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    public void Update()
    {
        if (isBattle) //�� �������� �Һ��� �ð� = deltTime
        {
            playTime += Time.deltaTime;
        }
    }

    //�������Ӹ��� ������ �޾ư��鼭 UI�� ���
    public void LateUpdate() //LateUpdate(): Update()�� ���� �� ȣ��Ǵ� �Լ�(�����ֱ�)
    {

        //���UI
        scoreTxt.text = string.Format("{0:n0}", player.score); //���� ����
        stageTxt.text = "STAGE " + stage; //���� ��������

        //�Ҽ������� ���� �÷��̽ð��� 00:00:00�� ���Ŀ� �°� ����
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second); //���� �÷��̽ð�

        //�÷��̾� UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth; //���� ü��
        playerCoinTxt.text = string.Format("{0:n0}", player.coin); //���� ����

        //���� ź��
        if(player.equipWeapon == null) //�÷��̾�� ���Ⱑ ���� ���
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else if(player.equipWeapon.type == Weapon.Type.Melee) //�ظӸ� �� ��� ź�� �� 0���� ǥ��
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else
        { // ������ ź��� / �����ϰ� �ִ� �� ź��� ǥ��
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }

        //���� UI
        //���� �������� ���� ��Ȳ�� ���� ���İ��� ����
        Weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        Weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        Weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        WeaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //���� ���� UI
        //���� ���������� �����ִ� ���� ��
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //Boss ü��
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200; //Boss�� ���� ���� boss ü��â ���ֱ�
        }           
    }   
}
