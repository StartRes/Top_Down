using System.Collections;
using System.Collections.Generic;
using UnityEngine;



///<summary>
///Part- ещё один из сериализуемый класс подобно Weapon Defination, предназначенный для хранения данных
/// </summary>
[System.Serializable]
public class Part
{
    //Значения этих трех полей должны определяться в инспекторе
    public string name; //Имя этой части
    public float health; //Степень стойкости части
    public string[] protectedBy; //Другие части заищщающе эту

    //Эти 2 поля инициализируеться автоматически в Start()
    //Кэширования, как здесь ускоряет получение необходимых данных
    [HideInInspector]
    public GameObject go; //Игровой объект этой части
    [HideInInspector]
    public Material mat; //Материал для отображения повреждений

}
public class Enemy_4 : Enemy
{


    /// <summary>
    /// Enemy_4 создается за верхней границей, выбирает случайную точку на экране
    /// и переместиться к ней. Добравшись до места, выбирает другую случайную точку и продолжает двигаеться, пока игрок не уничтожает
    /// </summary>
    // Start is called before the first frame update
    [Header("Set in Inspector: Enemy 4")]
    public Part[] parts; //Массив частей, составляющих корабль

    private Vector3 p0, p1; //Две точки для интерполяции
    private float timeStart; //Время создания этого корабля
    private float duration = 4; //Продолжительность перемещения

    void Start()
    {
        //Начальная позиция уеж выбрана в Main.SpawnEnemy(), поэтому запишем ее как начальнеое значение в p0 и p1
        p0 = p1 = pos;
        InitMovement();
    }

    void InitMovement()
    {
        //Записать в кэш игровой объект и материал каждой части в parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
        p0 = p1; //Переписать p1 в p0
        //Выбрать новую точку p1 на экране
        float widMinRad = bndChck.camWidth - bndChck.radius;
        float hgtMinRad = bndChck.camHeight - bndChck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        //Сбросить время
        timeStart = Time.time;
    }

    public override void Move()
    {
        //Этот метод переопределяет Enemy.Move() и реализует линейную интерполяцию
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);//Применимть плавное замедление
        pos = (1 - u) * p0 + u * p1;//Просто линейная интерполяция

    }

    //Эти 2 функции выполняют поиск части в массиве parts по имени или ссылке на игровой объект
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }


    //Эти функции возвращают true если данная часть уничтожена
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));

    }
    bool Destroyed(Part prt)
    {
        if (prt == null) //Если ссылка не была передана
        {
            return (true); //Вернуть true (то есть да, была уничтожена)
        }
        //Вернуть результат сравнения:prt.health <=0
        return (prt.health <= 0);

    }
    //Окрашивает в красный только одну часть а не весь корабль
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    //Переопределяет метод OnCollisionEnter из сценария Enemy.cs
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //Если корабль за границами экрана не повреждать его.
                if (!bndChck.isOnsScreen)
                {
                    Destroy(other);
                    break;
                }

                //Поразить вражеский корабль
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //Проверить защищена ли ещё эта часть корабля

                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        //Если хотябы одна из защитных частей ещё не разрушена
                        if (!Destroyed(s))
                        {
                            //Не наносить повреждения этой части
                            Destroy(other);//Уничтожить снаряд ProjectileHEro, выйти не повреждая Enemy_4
                            return;
                        }
                    }
                }
                //Эта часть не защищена, нанести ей повреждения.
                //Получить разрушаемую силу из Projectile.type и Main.WEAP_DICT
                prtHit.health -= Main.GetWeaponDefination(p.type).damageOnHit;
                //Показывать эффект попадания в часть
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    //Вместо разрушения корабля, деактивировать уничтоженну часть
                    prtHit.go.SetActive(false);

                }
                //Проверить был ли корабль полностью разрушен
                bool allDestroyed = true; //Предположить что разрушен;
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt)) //Если какая-то часть ещё существует

                    {
                        allDestroyed = false; // ЗАписать False в allDestroyed
                        break;
                    }

                }
                if(allDestroyed) //Если корабль разрушен полностью
                {
                    //Уведомить объект-одиночку Main что корабль разрушен
                    Main.S.ShipDestroyed(this);
                    //Уничтожить корабль обьект Enemy
                    Destroy(this.gameObject);

                }
                Destroy(other); //Уничтожить снаряд ProjectileHero
                break;
        }   
    }
  
}




