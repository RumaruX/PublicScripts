using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class EnemyController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int enemyType = 1; // 3 types
        /*
        1 = normal
        2 = avec tirs
        3 = boss
        */
        [SerializeField] private float speed = 5; // vitesse de déplacement
        [SerializeField] private GameObject enemyBullet; // Munition ennemie
        public Transform pointToGo; // Point ou l'ennemi doit aller
        [SerializeField] private Transform[] pointsToGo; // Point ou l'ennemi doit aller (pour le boss)
        [SerializeField] private Transform[] AttackPoints; // Point ou faire spawn les munitions
        private GameManager _gameManager;
        //[SerializeField] private Transform player;
        public int coinsToGenerate = 3; // Pieces a generer a la mort
        private bool _chosen = false; // Si le boss a choisi son 2eme point
        private bool _isActive = true; // Si le boss est actif
        private bool _canShoot = true; // Si peut tirer

        public int EnemyType => enemyType;

        #endregion

        #region Builtin Methods
        void Start()
        {
            if(enemyType == 3) _isActive = false; // Desactive pour ne pas faire venir le boss
            _gameManager = GameManager.instance;
        }

        void Update()
        {
            if(_gameManager.gameState == GameState.Resume) // Si la partie est en cours, déplace l'ennemi et lui permets de tirer
                if(pointToGo == null) return;
                if(!_isActive) return;
                GetComponent<Rigidbody>().velocity = (pointToGo.position - transform.position).normalized * speed;
                if(enemyType == 3 && transform.position.z <= 0){ // Si boss a au moins atteint le premier point
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                    if(!_chosen){ // Si boss a atteint son premier point et n'a pas choisi
                        GetComponent<CapsuleCollider>().enabled = true;
                        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        pointToGo = pointsToGo[Random.Range(0, 2)];
                        _chosen = true;
                    }
                    // Deplacement boss
                    if(transform.position.x <= -6.5f){
                        pointToGo = pointsToGo[1];
                    }else if(transform.position.x >= 6.5f){
                        pointToGo = pointsToGo[0];
                    }
                    Shoot();
                }
                if(enemyType == 2){
                    Shoot();
                }
            if(_gameManager.gameState == GameState.Pause)
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); // Freeze si fini ou en pause
            //transform.Translate((pointToGo.position.x - transform.position.x) * Time.deltaTime * speed, (pointToGo.position.y - transform.position.y) * Time.deltaTime * speed, (pointToGo.position.z - transform.position.z) * Time.deltaTime * speed);
            //float distance = Mathf.Abs(player.position.x - transform.position.x + player.position.y - transform.position.y + player.position.z - transform.position.z);
        }
        #endregion

        #region Custom Methods

        public void ActivateBoss(){ // Active le boss
            _isActive = true;
            GetComponent<HealthController>().SetSliderActive();
        }

        void Shoot() // Tire
        {

            if(_canShoot){
                foreach(Transform point in AttackPoints){
                    GameObject newBullet = Instantiate(enemyBullet, point);
                    newBullet.transform.Rotate(180, 0, 0);
                }
                _canShoot = false;
                StartCoroutine(ShootTimer());
            }

        }
        
        IEnumerator ShootTimer() // Timer de tir
        {
            if(enemyType == 2){
                yield return new WaitForSeconds(4f);
            }else{
                yield return new WaitForSeconds(1f);
            }
            _canShoot = true;
        }

        #endregion
    }
}