using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10f;
    public int score = 100;
    public float showDamageDuration = 0.1f;//������������ �������
    public float powerUpDropChance = 1f;//����������� �������� �����

    protected BoundsChecl bndChck;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;//��� ��������� �������� ������� � ��� ��������
    public bool showingDamage = false;
    public float damageDoneTime; //����� ����������� ����������� �������
    public bool notifiredOfDestruction = false;//����� ������������ �����


    private void Awake()
    {
        bndChck = GetComponent<BoundsChecl>();
        //�������� �������� � ���� ����� �������� ������� � ��� ��������
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i =0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    //��� ��������-����� ����������� ��� ����
    public Vector3 pos
    {
    
            get { return (this.transform.position); }
        
    
            set { this.transform.position = value; }
        
    }
   

    // Update is called once per frame
    void Update()
    {
        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        Move(); 
        if (bndChck != null && bndChck.offDown)
        {
           Destroy(gameObject);
                        
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;

        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //���� ��������� ������� �� ��������� ������ �� �������� ��� �����������
                if (!bndChck.isOnsScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                //�������� ��������� �������
                ShowDamage();
                //�������� ����������� ���� �� WEAP_DICT � ������ Main.
                health -= Main.GetWeaponDefination(p.type).damageOnHit;
                if (health <= 0)
                {
                    //�������� �������-�������� Main �� �����������
                    if(!notifiredOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiredOfDestruction = true;
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;

            default:
                 print("Enemy hit by non ProjectileHero" + otherGO.name);
                break;
                
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for (int i = 0; i<materials.Length; i++ )
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

}
