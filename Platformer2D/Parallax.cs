using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    [SerializeField] private float speed = 0;
    [SerializeField] private Transform cam;

    private float length, startPosition;

    void Start()
    {

        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        
    }

    void FixedUpdate() // Deplacement background
    {

        float temp = cam.position.x * (1 - speed);
        float distance = speed * cam.position.x;

        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if(temp > startPosition + length){
            startPosition += length;
        }else if(temp < startPosition - length){
            startPosition -= length;
        }
        
    }
    
}
