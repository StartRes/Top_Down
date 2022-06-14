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
        //Сгенерировать бонус с заданной вероятностью
        if(Random.value <= e.powerUpDropChance)
        {
            //Выбрать тип бонуса 
            //ВЫбрать один из элементов в powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Установить соответсвующий типа WeaponType
            pu.SetType(puType);
            //Поместить в место, где находился разрушенный корабль
            pu.transform.position = e.transform.position;
        }
    }

    private void Awake()
    {
        S = this;
        //Записать в bndcheck ссыылку на компонент BondsCheck этого игрового объекта
        bndCheck = GetComponent<BoundsChecl>();
        //Вызвать Enemy Spawn() один раз (в2 секунды при значении при умолчанию)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefination>();
        foreach(WeaponDefination def in weaponDefinations)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        //Выбрать случайный шаблон Enemy Для создания
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        //Разместить вражеский корабль над экраном в случайной позиции x
        float enemyPadding = enemyDefaultPadding;
        if(go.GetComponent<BoundsChecl>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsChecl>().radius);
        }

        //Установить начальные координаты созданного вражеского корабля
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //Снова вызвать SpawnEnemy
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }
    public void DelayedRestart (float delay)
    {
        //Вызывает метод Restart() через delay секунд
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        //Перезагрузить _Scene0 что бы перезагрузить игру
        SceneManager.LoadScene("_Scene0");
    }
    
    ///<summary>
    ///Статическая функция, возвращающая WeaponDefination из статического защищеного поля WEAP_DICT класса Main
    ///</summary>
   //<returns> Экзепляр WeaponDefintation или если нет такого определения для указаного WepaonType, Возвращает новый экземпляр WeponDefination c типом none </returns>
    static public WeaponDefination GetWeaponDefination (WeaponType wt)
    {
        //Проверить наличие указанного ключа в словаре
        //Попытка извлечь значение по отсутвующему ключу выозвет ошибку, потому важна инструкция ниже
        if(WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }

        //Следующая инструкция возвращает новый экземпляр WeaponDefination с типом оружия WeaponType.none. что означает неудачную попытку найти определение WeaponDefination
        return (new WeaponDefination());
    }

 }

    

