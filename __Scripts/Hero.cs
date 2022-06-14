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
    //Объявление нового делгата типа WeaponFireDelegate. 
    public delegate void WeaponFireDelegate();
    //Создать поле типа WeaponFireDelegate с именем fireDelegate
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
            S = this; //Сохранить ссылку одиночку
        }else
        {
            Debug.LogError("Hero.Awake() - Attemp to assign sceond Heros.s!");
        }
        // fireDelegate += TempFire;

        //Очистить массив weapons b yfxfnm buhe c 1 ,kfcnthjv
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }
    
    // Update is called once per frame
    void Update()
    {
        //Извлечь информацию из класса Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //Изменить transform.position, опираясь на информацию по осям
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;


        //Повернуть корабль, что бы придать озузения динамизма
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        //Позволить кораблю выстрелить
       // if(Input.GetKeyDown(KeyCode.Space))
       // {
       //     TempFire();
      //  }
        //Произвести выстрел из всех видов оружия вызовом fireDelegate
        //Сначала проверить нажатие клавиши Axis("Jump")
        //Затем убедиться что значение FireDelegate не равно null что бы избежать ошибки
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

        //Гарантировать невозможность повторного столкновения с тем же объктом
        if(go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go; ;
        //Если защитное поле столкнулось с врагом, уменьшить уровень защиты на 1 и уничтожить врага
        if(go.tag == "Enemy") 
        {
            shieldLevel--;
            Destroy(go);

        }
        else if (go.tag == "PowerUp")
        {
            //Если защитное поле столкнулось с бонусом
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
            //Если уровень поля упал до нуля и ниже
            if(value < 0)
            {
                Destroy(this.gameObject);
                //Сообщить объекту Main.S о необходимости перезапустить игру
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
                if (pu.type == weapons[0].type) //Если оружие того же типа
                {
                    Wepon w = GetEmptyWeaponSlot();
                

                    if (w != null)
                    {
                        //Установить в po.type 
                        w.SetType(pu.type);
                    }
                }else
                {
                    //Если оружие другого типа
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
   
    
}
