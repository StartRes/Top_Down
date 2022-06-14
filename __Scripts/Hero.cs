using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    private GameObject lastTriggerGo = null;
    //���������� ������ ������� ���� WeaponFireDelegate. 
    public delegate void WeaponFireDelegate();
    //������� ���� ���� WeaponFireDelegate � ������ fireDelegate
    public WeaponFireDelegate fireDelegate;

    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Wepon[] weapons;

    [Header("Set Dynamicly")]
    [SerializeField] public float _shieldLevel = 1;

    void Start ()
    {
        if(S==null)
        {
            S = this; //��������� ������ ��������
        }else
        {
            Debug.LogError("Hero.Awake() - Attemp to assign sceond Heros.s!");
        }
        // fireDelegate += TempFire;

        //�������� ������ weapons b yfxfnm buhe c 1 ,kfcnthjv
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }
    
    // Update is called once per frame
    void Update()
    {
        //������� ���������� �� ������ Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //�������� transform.position, �������� �� ���������� �� ����
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;


        //��������� �������, ��� �� ������� �������� ���������
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        //��������� ������� ����������
       // if(Input.GetKeyDown(KeyCode.Space))
       // {
       //     TempFire();
      //  }
        //���������� ������� �� ���� ����� ������ ������� fireDelegate
        //������� ��������� ������� ������� Axis("Jump")
        //����� ��������� ��� �������� FireDelegate �� ����� null ��� �� �������� ������
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
       // rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefination(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;

    }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        print("Triggered" + go.name);

        //������������� ������������� ���������� ������������ � ��� �� �������
        if(go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go; ;
        //���� �������� ���� ����������� � ������, ��������� ������� ������ �� 1 � ���������� �����
        if(go.tag == "Enemy") 
        {
            shieldLevel--;
            Destroy(go);

        }
        else if (go.tag == "PowerUp")
        {
            //���� �������� ���� ����������� � �������
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non Enemy" + go.name);
        }
    }
    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            //���� ������� ���� ���� �� ���� � ����
            if(value < 0)
            {
                Destroy(this.gameObject);
                //�������� ������� Main.S � ������������� ������������� ����
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
       
    }
    Wepon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        return (null);
    }
    void ClearWeapons()
    {
        foreach (Wepon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
    public void AbsorbPowerUp (GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            default:
                if (pu.type == weapons[0].type) //���� ������ ���� �� ����
                {
                    Wepon w = GetEmptyWeaponSlot();
                

                    if (w != null)
                    {
                        //���������� � po.type 
                        w.SetType(pu.type);
                    }
                }else
                {
                    //���� ������ ������� ����
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
   
    
}
