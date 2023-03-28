using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Variables

    [SerializeField] private int baseHealth = 100;
    [SerializeField] private float baseSpeed = 3;
    [SerializeField] private int damages = 1;
    [SerializeField] private int moneyAtDeath = 2;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform coinPrefab;

    private Transform _enemyTarget;
    private int _health;
    private UnityEngine.AI.NavMeshAgent _agent;

    private bool _isAffectedSpeed = false;
    private Transform _slowTower;

    private GameManager _gameManager;
    private SoundManager _soundManager;

    #endregion

    #region Properties

    public Transform Target{
        get{
            return _enemyTarget;
        }
        set{
            _enemyTarget = value;
        }
    }

    public float Speed{
        get{
            return baseSpeed;
        }
        set{
            baseSpeed = value;
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed = baseSpeed;
        }
    }

    public bool AffectedSpeed{
        get{
            return _isAffectedSpeed;
        }
        set{
            _isAffectedSpeed = value;
        }
    }

    public Transform SlowTower{
        get{
            return _slowTower;
        }
        set{
            _slowTower = value;
        }
    }

    public Transform TargetPoint{
        get{
            return targetPoint;
        }
    }

    #endregion

    #region Built-in Methods

    void Start()
    {
        _health = baseHealth;
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.speed = baseSpeed;
        _gameManager = GameManager.instance;
        _soundManager = SoundManager.instance;
    }

    void Update()
    {
        _agent.SetDestination(_enemyTarget.position); // Set la destination
    }

    void OnTriggerEnter(Collider col){ // Frappe la base
        if(col.gameObject.layer == LayerMask.NameToLayer("Base")){
            GetComponent<Collider>().enabled = false;
            _gameManager.RemoveLife(damages);
            _gameManager.CheckWin();
            DestroyGO();
        }
    }

    #endregion

    #region Custom Methods

    public void DestroyGO(){ // Destruction de l'ennemi
        Destroy(gameObject);
    }

    public float GetRemainingDistance() // Obtention distance restante avant la base
    {
        float distance = 0;
        Vector3[] corners = _agent.path.corners;

        if (corners.Length > 2)
        {
            for (int i = 1; i < corners.Length; i++)
            {
                    Vector2 previous = new Vector2(corners[i - 1].x, corners[i - 1].z);
                    Vector2 current = new Vector2(corners[i].x, corners[i].z);

                    distance += Vector2.Distance(previous, current);
            }
        }
        else 
        {
            distance = _agent.remainingDistance;
        }

        return distance;
    }

    public void RemoveHp(int damages){ // Retire de la vie a l'ennemi
        _health -= damages;
        if(_health <= 0){ // Si vie sous 0, le tue et drop de l'argent (une ou 2 pieces)
            _soundManager.KillEnemy();
            GetComponent<Collider>().enabled = false;
            _gameManager.CheckWin();
            if(Random.Range(0, 2) == 0){
                Transform newCoin;
                newCoin = Instantiate(coinPrefab, transform.position + new Vector3(Random.Range(0, 1), 0.4f, Random.Range(0, 1)), Quaternion.identity);
                newCoin.GetComponent<CoinController>().SetValue(Mathf.FloorToInt(moneyAtDeath/2f));
                newCoin.eulerAngles = new Vector3(90, 0, 0);
                newCoin.parent = _gameManager.CoinParent;
                newCoin = Instantiate(coinPrefab, transform.position + new Vector3(Random.Range(0, 1), 0.4f, Random.Range(0, 1)), Quaternion.identity);
                newCoin.GetComponent<CoinController>().SetValue(Mathf.CeilToInt(moneyAtDeath/2f));
                newCoin.eulerAngles = new Vector3(90, 0, 0);
                newCoin.parent = _gameManager.CoinParent;
            }else{
                Transform newCoin;
                newCoin = Instantiate(coinPrefab, transform.position + new Vector3(Random.Range(0, 1), 0.4f, Random.Range(0, 1)), Quaternion.identity);
                newCoin.GetComponent<CoinController>().SetValue(moneyAtDeath);
                newCoin.eulerAngles = new Vector3(90, 0, 0);
                newCoin.parent = _gameManager.CoinParent;
            }
            DestroyGO();
        }
    }

    #endregion

}
