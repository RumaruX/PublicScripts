using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private int wallet = 10;
    [SerializeField] private int baseHealth = 20;
    [SerializeField] private Transform coinParent;

    private SoundManager _soundManager;

    private int _health = 20;
    private bool _holdingBlueprint = false;
    private GameObject _actualBlueprint;
    public bool HoldingBlueprint{
        get{
            return _holdingBlueprint;
        }
        set{
            _holdingBlueprint = value;
        }
    }
    public GameObject ActualBlueprint{
        get{
            return _actualBlueprint;
        }
        set{
           _actualBlueprint = value;
        }
    }
    public Transform CoinParent{
        get{
            return coinParent;
        }
    }

    public static GameManager instance;
    private UIManager _UIManager;
    private WaveManager _waveManager;
    private TowerManager _towerManager;

    #endregion

    #region Properties

    #endregion

    #region Builtin Methods

    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
            return;
        }else{
            instance = this;
        }
    }

    void Start()
    {
        _UIManager = UIManager.instance;
        _waveManager = GetComponent<WaveManager>();
        _towerManager = GetComponent<TowerManager>();
        _UIManager.UpdateMoney(wallet);
        _soundManager = SoundManager.instance;

        _health = baseHealth;
        _UIManager.UpdateLife(baseHealth);
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    public void AddMoney(int money){ // Ajoute de l'argent
        wallet += money;
        _UIManager.UpdateMoney(wallet);
        _towerManager.ShowStats();
        _soundManager.Money();
    }

    public void CheckWin(){ // Check si gagne
        if(_waveManager.FinishedWaves){
            if(_waveManager.RemainingEnemies <= 1){
                Win();
            }
        }
    }

    public void RemoveMoney(int money){ // Retire de l'argent
        if((wallet - money) >= 0){
            wallet -= money;
            _UIManager.UpdateMoney(wallet);
        }else{
            Debug.Log("ERROR : CANT BUY");
        }
    }

    public bool CanBuy(int money){ // Check si il peut acheter ou non
        if(money <= wallet){
            return true;
        }else{
            return false;
        }
    }

    public void AddLife(int health){ // Ajoute de la vie
        _health += health;
        if(_health > baseHealth){
            _health = baseHealth;
        }
        _UIManager.UpdateLife(_health);
    }

    public void RemoveLife(int health){ // Retire de la vie
        _health -= health;
        _UIManager.UpdateLife(_health);
        if(_health <= 0){ // Si hp a 0, perds
            _health = 0;
            _UIManager.UpdateLife(_health);
            GameOver();
        }
        else if(_waveManager.FinishedWaves){ // Si tue tous les ennemis et fini toutes les vagues, gagne
            if(_waveManager.RemainingEnemies <= 1){
                Win();
            }
        }
    }

    private void GameOver(){ // Perdu
        _UIManager.GameOver();
        _waveManager.EndGame();
        //Time.timeScale = 0;
    }

    private void Win(){ // Gagne
        _UIManager.Win();
        _waveManager.EndGame();
        //Time.timeScale = 0;
    }

    #endregion
}
