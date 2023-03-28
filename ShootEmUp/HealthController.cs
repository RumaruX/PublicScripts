using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class HealthController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int baseHealth = 3; // Vie de base
        [SerializeField] private Slider healthSlider; // Slider de vie
        [SerializeField] private int scoreToGive; // Score a donner a la mort
        [SerializeField] private GameObject[] powerups; // Liste de powerups
        [SerializeField] private GameObject shield; // Object shield
        public GameObject coinModel; // Prefab de piece
        public Transform coinParent;
        private GameManager _gameManager;
        private float _shieldHealth = 0; // nombre de shield
        private float _health;
        public float Health => _health;
        private SoundManager _soundManager;
        private WaveManager _waveManager;

        #endregion

        #region Builtin Methods
        void Start()
        {

            _health = baseHealth;
            // Obtient le parent des pieces pour ranger
            GameObject[] parentList = GameObject.FindGameObjectsWithTag("CoinParent");
            foreach(GameObject g in parentList){
                coinParent = g.transform;
            }
            // Change la valeur max du slider a la vie max
            healthSlider.maxValue = baseHealth;
            healthSlider.value = baseHealth;
            _gameManager = GameManager.instance;
            _soundManager = SoundManager.instance;
            _waveManager = WaveManager.instance;

        }

        void Update()
        {

            //healthSlider.gameObject.transform.position = new Vector3(gameObject.transform.position.x, healthSlider.gameObject.transform.position.y, gameObject.transform.position.z - 3.25f);

        }
        #endregion

        #region Custom Methods

        public void SetSliderActive(){ // Active le slider (pour boss)
            healthSlider.gameObject.SetActive(true);
        }

        public void ActivateShield(){ // Active shield (joueur)
            shield.SetActive(true);
            _shieldHealth += 1;
            if(_shieldHealth > 3) _shieldHealth = 3;
        }

        public void RemoveLife(float n){ // retire vie ou 1 shield
            if(gameObject.tag == "Enemy") _health -= n;
            else{
                if(_shieldHealth == 0) _health -= n;
                else _shieldHealth -= 1;
                if(_shieldHealth <= 0){
                    _shieldHealth = 0;
                    shield.SetActive(false);
                }
            }
            if(_health <= 0){ // Si vie = 0, tue l'entite
                //Destroy(healthSlider.gameObject);
                if(gameObject.tag == "Enemy"){
                    for(int i = 0; i < GetComponent<EnemyController>().coinsToGenerate; i++){
                        GameObject coin = Instantiate(coinModel, transform);
                        coin.transform.SetParent(coinParent);
                    }
                    GetComponent<ParticleSystem>().Play();
                    Destroy(GetComponent<SphereCollider>());
                    for(int c = 0; c < transform.childCount; c++){
                        Destroy(transform.GetChild(c).gameObject);
                    }
                    if(GetComponent<EnemyController>().EnemyType == 3){
                        healthSlider.gameObject.SetActive(false);
                    }
                    Destroy(gameObject, 0.5f);
                    if(gameObject.name == "Boss"){
                        _gameManager.gameState = GameState.Won;
                    }
                    _waveManager.AddCount();
                }
                else{
                    Destroy(gameObject);
                    _gameManager.gameState = GameState.GameOver;
                }
                _gameManager.Scoring(scoreToGive);
                if(Random.Range(1, 101) <= 5 && gameObject.tag == "Enemy"){
                    GameObject powerup = Instantiate(powerups[Random.Range(0, powerups.Length)], transform);
                    powerup.transform.SetParent(coinParent);
                }
                _soundManager.Explosion(); // Son d'explosion
            }
            healthSlider.value = _health; // Affiche la valeur de la vie dans le slider
        }

        public void AddLife(float n){ // Ajute de la vie
            _health += n;
            if(_health > baseHealth) _health = baseHealth;
            healthSlider.value = _health;
        }

        #endregion
    }
}