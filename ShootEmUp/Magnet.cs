using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class Magnet : MonoBehaviour
    {
        #region Variables

        [SerializeField] private LayerMask layerToAttract;
        [SerializeField] private float range = 10;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float explosionRange = 10;
        [SerializeField] private float attractionStrength = 8;
        [SerializeField] private GameObject attractFX;
        
        private Collider[] colsToAttract = new Collider[300];
        private Collider[] enemiesToKill = new Collider[300];
        private GameManager _gameManager;
        private SoundManager _soundManager;
        private WaveManager _waveManager;
        private IEnumerator coroutine;

        #endregion

        #region Builtin Methods
        void Start()
        {
            _gameManager = GameManager.instance;
            _soundManager = SoundManager.instance;
            _waveManager = WaveManager.instance;
        }

        void FixedUpdate() // Attire les pieces et powerups
        {
            if(_gameManager.gameState == GameState.Resume)
                Physics.OverlapSphereNonAlloc(transform.position, range, colsToAttract, layerToAttract);
                if(colsToAttract == null) return;
                for(int i = 0; i < colsToAttract.Length; i++){
                    if(colsToAttract[i] == null) continue;
                    float distance = Vector3.Distance(colsToAttract[i].transform.position, transform.position);
                    distance = Mathf.Clamp(distance, 0, range);
                    Vector3 force =  (transform.position - colsToAttract[i].transform.position).normalized * attractionStrength * (range - distance) / range;

                    colsToAttract[i].GetComponent<Rigidbody>().AddForce(force);
                }

        }

        void OnTriggerEnter(Collider col){ // Recolte powerups et pieces
            if(col.gameObject.tag == "Coin"){ // Recolte piece
                Destroy(col.gameObject);
                _gameManager.AddCoins();
                if(coroutine != null) StopCoroutine(ActivateFX()); // Active fx du magnet
                coroutine = ActivateFX();
                StartCoroutine(ActivateFX());
                _soundManager.Coin();
            }
            if(col.gameObject.tag == "shieldPowerup"){ // Active shield
                Destroy(col.gameObject);
                GetComponent<HealthController>().ActivateShield();
                _soundManager.Shield();
            }
            if(col.gameObject.tag == "speedBoost"){ // Augmente vitesse d'attaque
                Destroy(col.gameObject);
                GetComponent<PlayerController>().ActivateSpeedBoost();
            }
            if(col.gameObject.tag == "explosion"){ // Tue les ennemis autour
                Physics.OverlapSphereNonAlloc(transform.position, explosionRange, enemiesToKill, enemyLayer);
                for(int i = 0; i < enemiesToKill.Length; i++){
                    if(enemiesToKill[i] == null) continue;
                    enemiesToKill[i].GetComponent<ParticleSystem>().Play();
                    Destroy(enemiesToKill[i].GetComponent<SphereCollider>());
                    for(int c = 0; c < enemiesToKill[i].transform.childCount; c++){
                        Destroy(enemiesToKill[i].transform.GetChild(c).gameObject);
                    }
                    Destroy(enemiesToKill[i].gameObject, 0.5f);
                    _waveManager.AddCount();
                }
                _soundManager.Explosion();
                Destroy(col.gameObject);
            }
        }
        #endregion

        #region Custom Methods

        IEnumerator ActivateFX(){ // Active effet de magnet
            attractFX.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            attractFX.SetActive(false);
            coroutine = null;
        }

        #endregion
    }
}