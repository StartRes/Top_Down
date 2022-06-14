using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Предотвращает выход игрового объекта за границы экрана.
/// Работоает только с ортографической камерой Main [0,0,0]
/// </summary>
public class BoundsChecl : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnscreen = true;

    [Header("Set Dynamicly")]
    public bool isOnsScreen = true;
    public float camWidth;
    public float camHeight;
    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    private void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnsScreen = true;
        offRight = offLeft = offUp = offDown = false;

        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }
        if(pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;

        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDown = true;
        }
        isOnsScreen = !(offRight || offLeft || offUp || offDown);
        if (keepOnscreen && !isOnsScreen)
        {
            transform.position = pos;
            isOnsScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }

        
    }
    // Рисует границы в панели Сцена с помощью OnDrawGizmos()

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
   

}
