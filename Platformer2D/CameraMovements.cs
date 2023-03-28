using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    
    [SerializeField] private Transform player;
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetY = 1;

    void FixedUpdate() // Mouement camera avec leger decalage
    {
        
        transform.Translate((player.position.x - transform.position.x + offsetX) * Time.deltaTime*3, (player.position.y - transform.position.y + offsetY) * Time.deltaTime*3, 0);

    }

}
