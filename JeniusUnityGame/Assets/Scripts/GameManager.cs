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
    public float playTime; //�����̿�ð�, �����÷��̽ð� ���� ���
    public bool isBattle; //���� �ο�� �������� �Ǵ��ϴ� ����
    public int enemyCntA; //���� ���������� ���� ���� ��
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
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); //Int -> Str ���� (�������� n,nnn)
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
        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }
    
}
