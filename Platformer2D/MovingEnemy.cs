using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{

    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform player;

    private bool _toRight = true;
    private Animator _anim;

    void Start()
    {
        
        _anim = GetComponent<Animator>();

    }

    void Update() // Deplace l'ennemi du point 1 au point 2 en le faisant tourner et le faisant attaquer si proche du joueur
    {
        
        if(gameObject.GetComponent<BoxCollider2D>() != null){
            if(_toRight){
                transform.Translate(1 * Time.deltaTime, 0, 0);
                if(transform.position.x + 1 >= pos2.position.x){
                    _toRight = false;
                    Vector3 scale = transform.localScale;
                    scale.x *= -1;
                    transform.localScale = scale;
                }
            }
            if(!_toRight){
                transform.Translate(-1 * Time.deltaTime, 0, 0);
                if(transform.position.x - 1 <= pos1.position.x){
                    _toRight = true;
                    Vector3 scale = transform.localScale;
                    scale.x *= -1;
                    transform.localScale = scale;
                }
            }
            if(Mathf.Abs(player.position.x - transform.position.x) <= 2){
                _anim.SetTrigger("Attack");
            }
        }

    }
}
