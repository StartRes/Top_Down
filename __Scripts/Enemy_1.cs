using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector")]
    //Число секунд полного цикла синусоиды
    public float waveFraquency = 2;
    //Ширина синусоиды в метрах
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; //начально значение координаты x
    private float birthTime;


    // Метод старт подходит так как не используеться супер классом Enemy
    void Start()
    {
        //Установить начальную координату x объекта Enemy_1
        x0 = pos.x;
        birthTime = Time.time;
    }

    //Переопределить функцию Move() суперкласса Enemy
    public override void Move()
    {
        // так как pos - это свойство, нельзя напрямую изменить pos.x
        //поэтому получим pos в виде Vector3 доступного для изменения
        Vector3 tempPos = pos;
        //значение Theta измениться с течением времени
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFraquency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        //base.Move() обрабатывает движение вних, вдоль оси Y 
        base.Move();
        //print(bndChck.isOnsScreen);
        

    }

   
}
