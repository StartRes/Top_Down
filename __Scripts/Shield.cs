using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationPerSecond = 0.1f;

    [Header("Set Dynamicly")]
    public int levelShown = 0;

    //Скрытые переменные, не появляющиеся в инспекторе
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Прочитать текущую мощность защитного поля из объекта одиночки Hero
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        //Если она отличается от LevelShown
        if(levelShown != currLevel)
        {
            levelShown = currLevel;
       
        //Скорректировать смещение в текстуре, чтобы отобразить поле с другой мощностью
        mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        //Поворачивать поле в каждом кадре с постоянной скоростью
        float rZ = -(rotationPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);

    }
}
