using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticles : MonoBehaviour
{

    void Start() //Arret des particules
    {
        gameObject.GetComponent<ParticleSystem>().Stop();
    }
    
}
