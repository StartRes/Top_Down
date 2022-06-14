using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsChecl bndCheck;
    private Renderer rend;

    [Header("Set Dynamicly")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    //Это общедоступной свойство маскирует поле _type и обрабатывает операции присваивания ему нового значения

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsChecl>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(bndCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    ///<summary>
    ///Изменяет скрытое поле _type и устанавлиает цвет этого снаряда, как определено в WeaponDefintations
    ///</summary>
    /// <param name="eType" > Тип WeaponType используемого оружия
    public void SetType(WeaponType eType)
    {
        //Установить _type
        _type = eType;
        WeaponDefination def = Main.GetWeaponDefination(_type);
        rend.material.color = def.projectileColor;
    }


}
