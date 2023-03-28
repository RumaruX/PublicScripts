using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerInteractions : MonoBehaviour
{

    #region Variables

    [SerializeField] private float _interactionCheckRadius = 0.1f;
    [SerializeField] private Transform[] doors;
    [SerializeField] private GameObject interactionCheck;
    [SerializeField] private GameObject coin;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject checkpoint;

    private bool _canUseDoor = true;
    private bool _isDead = false;
    private GameObject _newCoin;
    private PlayerInputs _inputs;
    private GameManager _gameManager;
    private Animator _anim;

    public bool IsDead => _isDead;

    #endregion

    #region Custom methods

    IEnumerator BackToLife(){ // Fais revivre

        yield return new WaitForSeconds(3);
        transform.position = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y, transform.position.z);
        cam.position = new Vector3(transform.position.x, transform.position.y, cam.position.z);
        _anim.SetBool("Death", false);
        _isDead = false;

    }

    IEnumerator UseDoor(){ // Timer entre chaque porte
        yield return new WaitForSeconds(0.5f);
        _canUseDoor = true;
    }

    void Die(){ // Tue le joueur

        _anim.SetBool("Death", true);
        _isDead = true;
        _gameManager.RemoveLife();
        if(_gameManager.PlayerLifeCount > 0){
            StartCoroutine(BackToLife());
        }

    }

    void Interact(){ // Interactions avec le joueur

        if(_inputs.Interaction){
            Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionCheck.transform.position, _interactionCheckRadius);
            foreach(Collider2D col in colliders){
                if(col.gameObject != gameObject){
                    if(_canUseDoor == true && col.gameObject.tag == "Door"){
                        int x = 0;
                        for(x = 0; x < doors.Length; x++){
                            if(doors[x].gameObject == col.gameObject){
                                if((x+1)%2 != 0){
                                    transform.position = new Vector3(doors[x+1].position.x, doors[x+1].position.y-1.3f, transform.position.z);
                                    cam.position = new Vector3(doors[x+1].position.x, doors[x+1].position.y-1.3f, cam.position.z);
                                }else{
                                    transform.position = new Vector3(doors[x-1].position.x, doors[x-1].position.y-1.3f, transform.position.z);
                                    cam.position = new Vector3(doors[x-1].position.x, doors[x-1].position.y-1.3f, cam.position.z);
                                }
                                _canUseDoor = false;
                                StartCoroutine(UseDoor());
                            }
                        }
                    }
                    if(col.gameObject.tag == "Chest"){ // Coffre
                        col.gameObject.GetComponent<Animator>().SetTrigger("Open");
                        col.gameObject.tag = "Untagged";
                        _newCoin = Instantiate(coin, col.gameObject.transform.position, Quaternion.identity);
                        _newCoin.transform.DOMoveY(col.gameObject.transform.position.y + 1, 0.2f, false);
                    }
                    if(col.gameObject.tag == "Boss"){ // Boss
                        Destroy(col.gameObject);    
                    }
                    if(col.gameObject.tag == "barrier"){ // Barriere et NPC epee
                        if(_gameManager.CoinCount >= 5){
                            Destroy(col.gameObject);
                        }else{
                            col.gameObject.GetComponent<ShowInformation>().Activate();
                        }
                    }
                    else if(col.gameObject.GetComponent<ShowInformation>() != null){
                        col.gameObject.GetComponent<ShowInformation>().Activate();
                    }
                }
            }
            if(gameObject.tag == "Sword"){ // animation d'attaque
                _anim.SetTrigger("Attack");
            }
        }

    }

    void OnTriggerEnter2D(Collider2D col){ // Quand il y a une collision

        if(col.gameObject.tag == "Coin"){ // Piece
            _gameManager.AddCoin();
            Destroy(col.gameObject);
        }
        if(col.gameObject.tag == "killZone"){ // Zone de danger
            Destroy(col.gameObject.transform.parent.GetComponent<BoxCollider2D>());
            Destroy(col.gameObject.transform.parent.GetComponent<BoxCollider2D>());
            Destroy(col.gameObject);
            col.gameObject.transform.parent.GetComponent<Animator>().SetTrigger("Die");
        }
        if(col.gameObject.tag == "Danger" && !_isDead){
            Die();
        }
        if(col.gameObject.tag == "Checkpoint"){ // Nouveau checkpoint
            if(col.gameObject != checkpoint){
                if(checkpoint.tag == "Checkpoint"){
                    checkpoint.GetComponent<ParticleSystem>().Stop();
                }
                checkpoint = col.gameObject;
                checkpoint.GetComponent<ParticleSystem>().Play();
            }
        }
        if(col.gameObject.tag == "SecretZone"){ // Zone secrete (pour fondu)
            col.gameObject.tag = "SecretZoneAlpha";
            StartCoroutine(MakingTransparent(col.gameObject));
        }
        if(col.gameObject.tag == "End"){ // Fin de niveau
            col.gameObject.GetComponent<NextLevel>().SwitchLevel();
        }

    }

    void OnTriggerExit2D(Collider2D col){ // Reaffiche zone secrete quand le joueur n'est plus dedans

        if(col.gameObject.tag == "SecretZoneAlpha"){
            col.gameObject.tag = "SecretZone";
            StartCoroutine(MakingVisible(col.gameObject));
        }

    }

    IEnumerator MakingTransparent(GameObject hitObject){ // Fondu zone secrete 1 (affiche)

        float alpha = 1;
        while(alpha > 0.4f){
            alpha -= 0.01f;
            hitObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }

    IEnumerator MakingVisible(GameObject hitObject){ // Fondu zone secrete 2 (cache)

        float alpha = 0.4f;
        while(alpha < 1){
            alpha += 0.01f;
            hitObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }

    #endregion

    #region Builtin methods

    void Start(){

        _inputs = GetComponent<PlayerInputs>();
        _gameManager = GameManager.instance;
        _anim = GetComponent<Animator>();

    }

    void FixedUpdate(){
        
        Interact();

    }

    #endregion

}
