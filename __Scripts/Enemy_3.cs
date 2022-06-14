using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    //���������� �������� Enemy 3 ������������ ����� �������� ������������ ������ ����� �� ����� ��� ���� ������
    [Header("Set in Inspector: Enemy 3")]
    public float lifeTime = 5;

    [Header("Set Dynamicly: Enemy 3")]
    public Vector3[] points;
    public float birthTime;

    //����� Start() ������ �������� ��� ����� �����
   
    void Start()
    {
        points = new Vector3[3]; //���������������� ������ �����

        //��������� ������� ��� ���������� � Main.SpawnEnemy()
        points[0] = pos;

        //���������� xMin � xMax ��� ��, ��� ��� ������ Main.SpawnEnemy()
        float xMin = -bndChck.camWidth + bndChck.radius;
        float xMax = bndChck.camWidth - bndChck.radius;

        Vector3 v;
        //�������� ������� ������� ����� ���� ������ ������� ������
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndChck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        //�������� ������� �������� ����� ���� ������� �������
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        //�������� � birthTime ������� �����
        birthTime = Time.time;
    }


    public override void Move()
    {
        //������ ����� ������������ �� ������ �������� u ����� 0 � 1
        float u = (Time.time - birthTime) / lifeTime;
        if(u>1)
        {
            //���� ��������� Enemy 3 �������� ���� ��������� ����
            Destroy(this.gameObject);
            return;
        }

        //��������������� ������ ����� �� ���� ������
        Vector3 p01, p12;
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;

    }
    
}
