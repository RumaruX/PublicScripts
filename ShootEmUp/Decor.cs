using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class Decor : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float moveSpeed; // Vitesse de mouvement
        [SerializeField] private GameObject myParticles; // Nuages
        private GameManager _gameManager;

        #endregion

        #region Builtin Methods
        void Start(){
            _gameManager = GameManager.instance;
        }

        void Update()
        {
            if(_gameManager.gameState == GameState.Resume) // Fonctionne si le jeu est en cours (pas de pause, pas gagné, pas perdu)
                transform.Translate(0, 0, moveSpeed * -1 * Time.deltaTime); // Deplacement de la ville
                if(gameObject.name == "City"){
                    if(transform.position.z <= -18.5){
                        transform.position = new Vector3(0, transform.position.y, 0);
                    }
                }
                // Arrete et joue les particules quand il le faut (pause / en train de jouer)
                if(myParticles.GetComponent<ParticleSystem>().isPlaying == false){
                    myParticles.GetComponent<ParticleSystem>().Play();
                }
            if(_gameManager.gameState != GameState.Resume)
                if(myParticles.GetComponent<ParticleSystem>().isPlaying == true){
                    myParticles.GetComponent<ParticleSystem>().Pause();
                }

        }
        #endregion

        #region Custom Methods

        #endregion
    }
}