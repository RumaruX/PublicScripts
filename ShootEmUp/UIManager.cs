using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class UIManager : MonoBehaviour
    {
        #region Variables
        public static UIManager instance;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject wonMenu;
        [SerializeField] private GameObject gameOverMenu;
        private GameManager _gameManager;
        #endregion

        #region Builtin Methods
        void Awake(){
            if(instance == null) instance = this;
            else Destroy(gameObject);
        }
        void Start()
        {
            _gameManager = GameManager.instance;
        }

        void Update() // Affiche les menus etc
        {
            if(_gameManager.gameState == GameState.Pause){
                pauseMenu.SetActive(true);
            }else{
                pauseMenu.SetActive(false);
            }
            if(_gameManager.gameState == GameState.Won){
                wonMenu.SetActive(true);
            }
            if(_gameManager.gameState == GameState.GameOver){
                gameOverMenu.SetActive(true);
            }
        }
        #endregion

        #region Custom Methods

        public void UpdateCoinsText(int coinCount){ // Affiche nombre de pieces
            coinsText.text = coinCount.ToString();
        }

        public void LoadScore(float score){ // Affiche score
            scoreText.text = score.ToString();
        }

        #endregion
    }
}