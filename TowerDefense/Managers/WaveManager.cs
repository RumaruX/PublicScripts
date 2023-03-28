using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private int nbWaves = 3;
    [SerializeField] private int nbEnemies = 10;
    [SerializeField] private float spawnCooldown = 1f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Transform enemySpawner;
    [SerializeField] private Transform enemyTarget;
    [SerializeField] private Transform enemyParent;

    [SerializeField] private GameObject[] possibleEnemies;
    [SerializeField] private GameObject boss;

    private int _currentWave = 0;

    public bool FinishedWaves{
        get{
            return _currentWave >= nbWaves;
        }
    }

    public int RemainingEnemies{
        get{
            return enemyParent.childCount;
        }
    }

    #endregion

    #region Properties

    #endregion

    #region Built-in Methods

    void Start()
    {
        StartCoroutine(RunWave());
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    IEnumerator RunWave(){ // Spawn les vagues d'ennemis
        _currentWave++;

        if(_currentWave < nbWaves){ // Si il faut faire une vague
            // Choisis les ennemis a spawn 
            Dictionary<GameObject, int> enemyDict = new Dictionary<GameObject, int>();
            foreach(GameObject enemy in possibleEnemies){
                enemyDict.Add(enemy, 0);
            }
            for(int i = 0; i < nbEnemies; i++){
                enemyDict[possibleEnemies[Random.Range(0, possibleEnemies.Length)]] += 1;
            }
            // Spawn les ennemis par types un par un
            foreach(GameObject enemy in enemyDict.Keys){
                for(int i = 0; i < enemyDict[enemy]; i++){
                    GameObject newEnemy = Instantiate(enemy, enemySpawner.position, Quaternion.identity);
                    newEnemy.GetComponent<Enemy>().Target = enemyTarget;
                    newEnemy.transform.name = "Enemy : " + i.ToString();
                    newEnemy.transform.eulerAngles += new Vector3(0, -90, 0);
                    newEnemy.transform.parent = enemyParent;
                    yield return new WaitForSeconds(spawnCooldown);
                }
            }

            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(RunWave());

        }else{ // Si derniere vague fait spawn le boss
            BossSpawn();
        }

        yield return null;
    }

    void BossSpawn(){ // Spawn du boss
        GameObject newEnemy = Instantiate(boss, enemySpawner.position, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().Target = enemyTarget;
        newEnemy.transform.name = "BOSS";
        newEnemy.transform.parent = enemyParent;
    }

    public void EndGame(){ // Supprime les ennemis et stoppe tout
        StopAllCoroutines();
        for(int i = 0; i < enemyParent.childCount; i++){
            Destroy(enemyParent.GetChild(i).gameObject);
        }
    }

    #endregion

}
