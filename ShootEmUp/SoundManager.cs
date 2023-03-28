using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class SoundManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private AudioClip shieldSound;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioClip lifeSound;
        public static SoundManager instance;
        private AudioSource _audioSource;
        #endregion

        #region Builtin Methods
        void Awake(){
            if(instance == null) instance = this;
            else Destroy(gameObject);
        }
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {

        }
        #endregion

        #region Custom Methods

        public void Shoot(){ // Son de tir
            _audioSource.PlayOneShot(shootSound);
        }

        public void Coin(){ // Son de piece
            _audioSource.PlayOneShot(coinSound);
        }

        public void Shield(){ // Son de shield
            _audioSource.PlayOneShot(shieldSound);
        }

        public void Life(){ // Son de vie gagnee
            _audioSource.PlayOneShot(lifeSound);
        }

        public void Explosion(){ // Son d'explosion
            _audioSource.PlayOneShot(explosionSound);
        }

        #endregion
    }
}