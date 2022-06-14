using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    [Header("Set in inspector")]
    static Dictionary<WeaponType, WeaponDefination> WEAP_DICT;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;

    public WeaponDefination[] weaponDefinations;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };

    private BoundsChecl bndCheck;

    public void ShipDestroyed (Enemy e)
    {
        //������������� ����� � �������� ������������
        if(Random.value <= e.powerUpDropChance)
        {
            //������� ��� ������ 
            //������� ���� �� ��������� � powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //������� ��������� PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //���������� �������������� ���� WeaponType
            pu.SetType(puType);
            //��������� � �����, ��� ��������� ����������� �������
            pu.transform.position = e.transform.position;
        }
    }

    private void Awake()
    {
        S = this;
        //�������� � bndcheck ������� �� ��������� BondsCheck ����� �������� �������
        bndCheck = GetComponent<BoundsChecl>();
        //������� Enemy Spawn() ���� ��� (�2 ������� ��� �������� ��� ���������)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefination>();
        foreach(WeaponDefination def in weaponDefinations)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        //������� ��������� ������ Enemy ��� ��������
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        //���������� ��������� ������� ��� ������� � ��������� ������� x
        float enemyPadding = enemyDefaultPadding;
        if(go.GetComponent<BoundsChecl>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsChecl>().radius);
        }

        //���������� ��������� ���������� ���������� ���������� �������
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //����� ������� SpawnEnemy
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }
    public void DelayedRestart (float delay)
    {
        //�������� ����� Restart() ����� delay ������
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        //������������� _Scene0 ��� �� ������������� ����
        SceneManager.LoadScene("_Scene0");
    }
    
    ///<summary>
    ///����������� �������, ������������ WeaponDefination �� ������������ ���������� ���� WEAP_DICT ������ Main
    ///</summary>
   //<returns> �������� WeaponDefintation ��� ���� ��� ������ ����������� ��� ��������� WepaonType, ���������� ����� ��������� WeponDefination c ����� none </returns>
    static public WeaponDefination GetWeaponDefination (WeaponType wt)
    {
        //��������� ������� ���������� ����� � �������
        //������� ������� �������� �� ������������ ����� ������� ������, ������ ����� ���������� ����
        if(WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }

        //��������� ���������� ���������� ����� ��������� WeaponDefination � ����� ������ WeaponType.none. ��� �������� ��������� ������� ����� ����������� WeaponDefination
        return (new WeaponDefination());
    }

 }

    

