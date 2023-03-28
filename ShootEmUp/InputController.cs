using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class InputController : MonoBehaviour
    {
        #region Variables

        // Creation des variables pour les inputs

        private float _horizontal;
        private float _vertical;
        private bool _shooting;
        private GameManager _gameManager;

        public float Vertical => _vertical;
        public float Horizontal => _horizontal;
        public bool IsShooting => _shooting;

        #endregion

        #region Builtin Methods

        void Start(){
            _gameManager = GameManager.instance;
        }

        void Update() // Obtension des valeurs
        {

            _horizontal = Input.GetAxis("Horizontal"); // Deplacement horizontal
            _vertical = Input.GetAxis("Vertical"); // Deplacement verrtical
            if(Input.GetAxis("Jump") == 1){ // Si touche entree pour attaquer
                _shooting = true;
            }else _shooting = false;
            if(Input.GetKeyDown(KeyCode.P)){
                if(_gameManager.gameState == GameState.Resume){
                    _gameManager.gameState = GameState.Pause;
                }else{
                    _gameManager.gameState = GameState.Resume;
                }
            }
            if(Input.GetKeyDown(KeyCode.X)){ // Type d'attaque qui change
                GetComponent<PlayerController>().ChangeAttackType();
            }
            if(Input.GetKeyDown(KeyCode.C)){ // Type de munition qui change
                GetComponent<PlayerController>().ChangeBulletType();
            }

        }
        #endregion

        #region Custom Methods

        #endregion
    }
}