using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class WaveManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject spawnPoint;
        [SerializeField] private GameObject enemy;
        [SerializeField] private GameObject enemy2;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform[] spawnPointsLeft;
        [SerializeField] private Transform[] spawnPointsRight;
        //[SerializeField] private int waveCount = 3;
        [SerializeField] private int maxEnemiesCount = 10;
        [SerializeField] private int minEnemiesCount = 3;
        [SerializeField] private float timeBetweenEnemies = 3.5f;
        [SerializeField] private int enemiesToKill = 50;
        [SerializeField] private bool spawn2enemies = false;
        private int _enemiesKilled = 0; // compteur de kill
        private bool _isRunning = true; // Si une wave doit spawn
        private GameObject actualEnemy; // pour choisir type d'ennemi par vague
        [SerializeField] private Transform player;
        [SerializeField] private GameObject boss;
        private GameManager _gameManager;
        public static WaveManager instance;
        //private Camera _cam;
        //private (float, float) _camSize;
        #endregion

        #region Builtin Methods
        void Awake(){
            if(instance == null) instance = this;
            else Destroy(gameObject);
        }
        void Start()
        {
            //_cam = Camera.main;
            //_camSize = (_cam.orthographicSize * _cam.aspect, _cam.orthographicSize);
            
            //Instantiate(spawnPoint, new Vector3(_camSize.Item1))
            _gameManager = GameManager.instance;
            StartCoroutine(Waves());
        }

        void Update()
        {
            
        }
        #endregion

        #region Custom Methods

        IEnumerator Waves(){
            while(_isRunning){ // Si condition de fin de wave non obtenue

                if(_gameManager.gameState == GameState.Resume){ // Si jeu en route

                    Transform baseSpawn;
                    Transform endSpawn;

                    if(Random.Range(0, 2) == 0){ // Prends spawn et point de fin aleatoire
                        baseSpawn = spawnPointsLeft[Random.Range(0, spawnPointsLeft.Length)];
                        endSpawn = spawnPointsRight[Random.Range(0, spawnPointsRight.Length)];
                    }else{
                        baseSpawn = spawnPointsRight[Random.Range(0, spawnPointsRight.Length)];
                        endSpawn = spawnPointsLeft[Random.Range(0, spawnPointsLeft.Length)];
                    }
                    if(spawn2enemies){ // Prends ennemi aleatoire si voulu
                        if(Random.Range(0, 2) == 0){
                            actualEnemy = enemy2;
                        }else{
                            actualEnemy = enemy;
                        }
                    }else{
                        actualEnemy = enemy;
                    }

                    int randomEnemiesCount = Random.Range(minEnemiesCount, maxEnemiesCount); // fait spawn un nombre d'ennemis aleatoires
                    for(int j = 0; j < randomEnemiesCount; j++){
                        if(_gameManager.gameState == GameState.Pause)
                            while(_gameManager.gameState == GameState.Pause){
                                yield return new WaitForSeconds(timeBetweenEnemies);
                            }
                        GameObject newEnemy = Instantiate(actualEnemy, baseSpawn.position, Quaternion.identity);
                        newEnemy.transform.SetParent(enemyParent);
                        newEnemy.GetComponent<EnemyController>().pointToGo = endSpawn;
                        Destroy(newEnemy, 15);
                        yield return new WaitForSeconds(timeBetweenEnemies);
                    }

                    if(_enemiesKilled >= enemiesToKill){ // Si condition de din de wave arrete les waves
                        _isRunning = false;
                    }

                    yield return new WaitForSeconds(10);
                }
            }
            boss.GetComponent<EnemyController>().ActivateBoss(); // fait spawn le boss
        }

        public void AddCount(){ // Ajoute 1 kill
            _enemiesKilled += 1;
        }

        #endregion
    }
}