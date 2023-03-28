using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    #region Variables

    [SerializeField] private float speed = 10f; // Vitesse du projectile

    private Transform _target;
    private int _damages;

    #endregion

    #region Properties

    public Transform Target{
        get => _target;
        set=> _target = value;
    }

    public int Damages{
        get => _damages;
        set=> _damages = value;
    }

    #endregion

    #region Built-in Methods

    void Start()
    {
        StartCoroutine(LerpPosition(1 / speed));
    }

    void OnTriggerEnter(Collider col){ // Collision des ennemis
        if(LayerMask.LayerToName(col.gameObject.layer) == "Enemies"){
            col.gameObject.GetComponent<Enemy>().RemoveHp(_damages);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    IEnumerator LerpPosition(float duration){ // Déplacement des projectiles
        float time = 0;
        Vector3 startPos = transform.position;

        while(time < duration){
            try{
                transform.position = Vector3.Lerp(startPos, _target.position, time/duration);
                time += Time.deltaTime;
            }
            catch (System.Exception)
            {
                Destroy(gameObject);
            }
            yield return null;
        }
        
        transform.position = _target.position;

        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    #endregion

}
