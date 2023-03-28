using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAfterAttack : TowerCharacteristics
{

    #region Variables

    [Header("Tower Attacks")]
    [SerializeField] private Transform headGO;
    [SerializeField] private Transform nuzzlePos;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int attackDamages = 5;
    [SerializeField] private float attackCooldown = 0.3f;
    [Header("Tower Effect after attack")]
    [SerializeField] private int afterAttackDamages = 2;
    [SerializeField] private float afterCooldown = 0.5f;
    [SerializeField] private int afterAttacksNumber = 00;

    [SerializeField] private List<Transform> targets = new List<Transform>();

    private Transform _target;
    private float _closestEnemyFromBase = 100000;

    private float _lastFireTime = 0;

    private Dictionary<GameObject, Dictionary<string, int>> enemiesToDamage = new  Dictionary<GameObject, Dictionary<string, int>>(); // Pour faire les degats quand la tourelle n'attaque plus

    #endregion

    #region Properties

    public int AttackDamages{
        get{
            return attackDamages;
        }
    }

    public float AttackSpeed{
        get{
            return attackCooldown;
        }
    }

    #endregion

    #region Built-in Methods

    public override void Update(){
        base.Update();

        if(_target == null){
            SetTarget();
        }
    }

    private void OnTriggerEnter(Collider col){ // Si un ennemi n'existe plus le supprime de la liste
        if(col.gameObject.layer != LayerMask.NameToLayer("Enemies")){
            return;
        }else{
            targets.Add(col.transform);
            SetTarget();
        }
    }

    private void OnTriggerStay(Collider col){ // Si un ennemi n'existe plus le supprime de la liste
        if(col.gameObject.layer != LayerMask.NameToLayer("Enemies")){
            return;
        }
        if(col.transform == _target){
            if(_target == null){
                targets.Remove(_target);
            }
        }
    }

    private void OnTriggerExit(Collider col){ // Si un ennemi sort de la range le retire de la liste
        if(col.gameObject.layer != LayerMask.NameToLayer("Enemies")){
            return;
        }else{
            targets.Remove(col.transform);
            if(col.transform == _target){
                SetTarget();
            }
        }
    }

    #endregion

    #region Custom Methods

    private void SetTarget(){ // Choisis le target
        if(targets.Count != 0){ // Si plus d'un ennemi dans la liste
            if(targets[0] == null){ // Si ennemi 1 est null retire de la liste
                targets.RemoveAt(0);
                SetTarget();
                return;
            }
            _target = targets[0];
        }else{
            _target = null;
        }


        // Cherche l'ennemi le plus proche de la base et attaque
        _closestEnemyFromBase = 100000;

        foreach(Transform target in targets){
            if(target != null){
                Enemy currentEnemy = target.GetComponent<Enemy>();
                float distanceToBase = currentEnemy.GetRemainingDistance();
                if(distanceToBase < _closestEnemyFromBase){
                    _closestEnemyFromBase = distanceToBase;
                    _target = target;
                }
            }else{
                targets.Remove(target);
            }
        }

        if(headGO){
            StartCoroutine(HeadFollow());
        }
    }

    private void CleanTargetList(){ // Clear les null
        for(int i = 0; i < targets.Count; i++){
            if(targets[i] == null){
                targets.RemoveAt(i);
            }
        }
    }

    IEnumerator HeadFollow(){ // Suis l'ennemi du regard
        projectile.SetActive(true);
        while(_target){
            if(_target == null){
                SetTarget();
                break;
            }
            Fire();
            headGO.LookAt(_target);
            yield return new WaitForEndOfFrame();
        }
        projectile.SetActive(false);
        yield return null;
    }

    private void Fire(){ // Attaque la cible
        if(!projectile) return;

        if(Time.time >= _lastFireTime + attackCooldown){
            _lastFireTime = Time.time;
            _target.GetComponent<Enemy>().RemoveHp(attackDamages);
            if(!enemiesToDamage.ContainsKey(_target.gameObject)){
                StartCoroutine(AfterEffect(_target.gameObject));
            }
        }
    }

    IEnumerator AfterEffect(GameObject enemy){ // Mets l'effet a la cible
        if(enemiesToDamage.ContainsKey(enemy)){
            enemiesToDamage[enemy] = new Dictionary<string, int>();
            enemiesToDamage[enemy]["damages"] = afterAttackDamages;
            enemiesToDamage[enemy]["times"] = afterAttacksNumber;
        }else{
            enemiesToDamage.Add(enemy, new Dictionary<string, int>());
            enemiesToDamage[enemy].Add("damages", afterAttackDamages);
            enemiesToDamage[enemy].Add("times", afterAttacksNumber);
        }
        while(_target == enemy.transform){
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(ApplyEffect(enemy));
    }

    IEnumerator ApplyEffect(GameObject enemy){ // Applique l'effet a la cible
        while(enemiesToDamage[enemy]["times"] > 0){
            if(enemy.transform == _target){
                StartCoroutine(AfterEffect(enemy));
                break;
            }
            yield return new WaitForSeconds(afterCooldown);
            try
            {
                enemy.GetComponent<Enemy>().RemoveHp(enemiesToDamage[enemy]["damages"]);
                enemiesToDamage[enemy]["times"] -= 1;
            }
            catch (System.Exception)
            {
                break;
                throw;
            }
        }
    }

    #endregion

}
