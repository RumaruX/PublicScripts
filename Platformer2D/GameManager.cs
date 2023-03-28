using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private int _playerLifeCount = 3;
    [SerializeField] private int _coinCount = 0;
    [SerializeField] private Image background;
    [SerializeField] private Text defeatText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image defeat;
    [SerializeField] private Image[] lifeList;

    private float _temps = 300.5f;
    public int PlayerLifeCount => _playerLifeCount;
    public int CoinCount => _coinCount;

    public static GameManager instance;
    #endregion

    #region Builtin Methods

    void Start(){ // Cacher gameOver

        background.color = new Color(0, 0, 0, 0);
        defeat.color = new Color(1, 1, 1, 0);
        defeatText.color = new Color(1, 1, 1, 0);

    }

    void Update(){ // Timer

        if(_playerLifeCount != 0){
            if(_temps > 0){
                _temps -= 1 * Time.deltaTime;
                scoreText.text = "Score : " + _coinCount.ToString();
                float _sec = _temps;
                float _min = Mathf.Floor(_sec/60);
                _sec = _sec - _min * 60;
                timerText.text = "Timer : " + _min.ToString() + "minutes, " + Mathf.Round(_sec).ToString() + " secondes";
            }else{
                RemoveLife(3);
            }
        }

    }

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }
    #endregion

    #region Custom Methods

    IEnumerator GameOver(){ // Montrer gameover

        float alpha = 0;
        while(alpha < 1){
            alpha += 0.005f;
            background.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        alpha = 0;
        while(alpha < 1){
            alpha += 0.01f;
            defeat.color = new Color(1, 1, 1, alpha);
            defeatText.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(10);
        StartCoroutine(MakingInvisible());
    }

    IEnumerator MakingInvisible(){ // Retourner au menu + fin gameover avec fondu

        float alpha = 1;
        while(alpha > 0){
            alpha -= 0.01f;
            defeat.color = new Color(1, 1, 1, alpha);
            defeatText.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Ending();

        SceneManager.LoadScene("Menu");

    }

    public void Ending(){ // Detruit l'instance pour ne pas faire bug

        Destroy(instance);
        
    }

    public void RemoveLife(int count = 1){ // Enleve des vies
        _playerLifeCount--;
        for(int x = 3 - _playerLifeCount; x > 0; x--){
            lifeList[3 - x].enabled = false;
        }
        if(_playerLifeCount<=0){
            StartCoroutine(GameOver());
        }
    }

    public void AddLife(int count = 1){ // Ajoute des vies (non utilise)
        _playerLifeCount += count;
        if(_playerLifeCount > 3){
            _playerLifeCount = 3;
        }
        for(int x = 0; x < _playerLifeCount; x++){
            lifeList[x].enabled = true;
        }
    }

    public void RemoveCoin(int count = 1){ // Enleve des pieces (non utilise)
        _coinCount--;
        if(_coinCount<=0){
            _coinCount = 0;
        }
    }

    public void AddCoin(int count = 1){ // Ajoute des pieces
        _coinCount += count;
    }
    #endregion
    
}
