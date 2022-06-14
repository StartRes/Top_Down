using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    //НЕобычное, но удобное применение Vector2. x хранит минимальное значение, а y максимальное значние для метода Random.Range()
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;//Время в секундах существования PowerUp
    public float fadeTime = 4f;//Second it will then fade

    [Header("Set Dynamicly")]
    public WeaponType type; //Тип бонуса
    public GameObject cube; //Ссылка на вложенный куб
    public TextMesh letter; //ССылка на TextMesh
    public Vector3 rotPerSecond; //Скорость вращения
    public float birthTime;

    private Rigidbody rigid;
    private BoundsChecl bndChck;
    private Renderer cubeRend;

    private void Awake()
    {
        //Получить ссылку на куб
        cube = transform.Find("Cube").gameObject;
        //Получить ссылки на TextMesh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndChck = GetComponent<BoundsChecl>();
        cubeRend = cube.GetComponent<Renderer>();

        //Выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; //Полоучить случайную скорость XYZ
        //Random.onUnitSphere возвращает вектор, указывающий на случайную точку, находящуюся на поверхности сферы с радиусом 1м и с центром в начале координат
        vel.z = 0; //Отобразить vel на плоскости XY
        vel.Normalize(); //Нормализация устанавливает длинну Vector3 равной 1м
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //Установить угол поворота этого игрового объекта равным R:[0,0,0]
        transform.rotation = Quaternion.identity;
        //Quaternion.identyty равноценно отсутсвие поворота
        //Выбрать случайную скорость вращения для вложенного куба с использованием RotMinMax.x и rotMinMax.y
        rotPerSecond = new Vector3 (Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x,rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;


    }

    private void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        //Эффект расстворения куба PowerUp с течением времени
        //Со значениями по умолчанию бонус существует 10 секунд, а затем растворяется в течении 4 секунд
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //В течении lifeTime секунд значение u будет <= 0. Затем оно станет положительным и через fadeTime секунд станет больше 1.

        //Если u>=1, уничтожает бонус
        if(u>=1)
        {
            Destroy(this.gameObject);
            return;
        }

        //ИСпользовать u для определения альфа-значения куба и буквы
        if (u>0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            //Буква тоже должна расствориться, но медлинне
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        if(!bndChck.isOnsScreen)
        {
            //Если бонус полностью вышел за границу экрана, уничтожить его
            Destroy(gameObject);
        }
    }

    public void SetType (WeaponType wt)
    {
        //Получить WeaponDefination из Main
        WeaponDefination def = Main.GetWeaponDefination(wt);
        //Установить цвет дочернего куба
        cubeRend.material.color = def.color;
        //leter.color = def.color Букву тоже можно окрасить в тот же цвет
        letter.text = def.letter; //Установить отображаемую букву
        type = wt; //В заключение установить фактический тип
    }

    public void AbsorbedBy(GameObject target)
    {
        //Эта функция вызывается классом Hero, когда игрок подбирает бонус
        //Можно было бы реализовать эффект поглощения боуса, уменьшая его размер в течение нескольких кадров, но пока просто уничтожим
        Destroy(this.gameObject);
    }
}
