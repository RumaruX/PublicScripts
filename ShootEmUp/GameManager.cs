using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public enum GameState{
        Resume,
        Pause,
        GameOver,
        Won
    }
    public class GameManager : MonoBehaviour
    {
        #region Variables

        public static GameManager instance;
        public GameState gameState; // Etat du jeu
        private UIManager _UIManager;
        private int _coinCount = 0;
        private float _score = 0;
        private float[] _multiplicateur = new float[] {1, 1.2f, 1.5f, 1.8f, 2, 2.5f, 3, 3.8f, 4.5f, 6, 8};
        private int _currentMultiplicateur = 0;
        [SerializeField] private float _multiplicateurBaseTimeLeft = 2.5f;
        private float _multiplicateurTimeLeft = 0;
        private int CoinCount => _coinCount;

        #endregion

        #region Builtin Methods
        void Awake(){
            if(instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            _UIManager = UIManager.instance;
            gameState = GameState.Resume;
            _multiplicateurTimeLeft = _multiplicateurBaseTimeLeft;
        }

        void Update()
        {

        }
        #endregion

        #region Custom Methods

        public void Pause(bool isPause){ // Mets le jeu en pause ou le remets en route
            if(isPause)
                gameState = GameState.Pause;
            else
                gameState = GameState.Resume;
        }

        public void GameOver(){ // Gameover
            gameState = GameState.GameOver;
        }

        public void AddCoins(int n = 1){ // Ajoute piece
            _coinCount += n;
            _UIManager.UpdateCoinsText(_coinCount);
        }

        public void RemoveCoins(int n = 1){ // Enleve piece
            _coinCount -= n;
            if(_coinCount < 0) _coinCount = 0;
            _UIManager.UpdateCoinsText(_coinCount);
        }

        public void Scoring(int val){ // Ajoute score avec multiplicateur
            StopAllCoroutines();
            StartCoroutine(AddScore(val));
            StartCoroutine(Multiplicateur());
        }

        IEnumerator Multiplicateur(){ // Multiplicateur de score
            if(_currentMultiplicateur + 1 < _multiplicateur.Length){
                _currentMultiplicateur++;
            }
            _multiplicateurTimeLeft = _multiplicateurBaseTimeLeft;
            while(_multiplicateurTimeLeft > 0){
                _multiplicateurTimeLeft -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _currentMultiplicateur = 0;
        }

        IEnumerator AddScore(float val){ // Ajoute score
            _score += _multiplicateur[_currentMultiplicateur] * val;
            _UIManager.LoadScore((float)_score);
            yield return null;
        }

        public void Win(){ // Gagne
            gameState = GameState.Won;
        }

        #endregion
    }
}