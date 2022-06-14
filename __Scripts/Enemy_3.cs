using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    //Траэктория движения Enemy 3 вычисляеться путем линейной интерполяции кривой Базье по более чем двум точкам
    [Header("Set in Inspector: Enemy 3")]
    public float lifeTime = 5;

    [Header("Set Dynamicly: Enemy 3")]
    public Vector3[] points;
    public float birthTime;

    //Метод Start() хорошо подходит для наших целей
   
    void Start()
    {
        points = new Vector3[3]; //Инициализировать массив точек

        //Начальная позиция уже определена в Main.SpawnEnemy()
        points[0] = pos;

        //Установить xMin и xMax так же, как это делает Main.SpawnEnemy()
        float xMin = -bndChck.camWidth + bndChck.radius;
        float xMax = bndChck.camWidth - bndChck.radius;

        Vector3 v;
        //Случайно выбрать среднюю точку ниже нижней границы экрана
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndChck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        //Случайно выбрать конечную точку выше верхней границы
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        //Записать в birthTime текущее время
        birthTime = Time.time;
    }


    public override void Move()
    {
        //Кривые Бахье вычисляються на основе значения u между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;
        if(u>1)
        {
            //Этот экземпляр Enemy 3 завершил свой жизненный цикл
            Destroy(this.gameObject);
            return;
        }

        //Интерполировать кривую Базье по трем точкам
        Vector3 p01, p12;
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;

    }
    
}
