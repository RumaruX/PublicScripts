using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : TowerCharacteristics
{

    #region Variables

    [Header("Tower Attacks")]
    [SerializeField] private Transform headGO;
    [SerializeField] private Transform[] nuzzlePos;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int attackDamages;
    [SerializeField] private float attackCooldown;
    [SerializeField] private bool randomTargets = false;

    [SerializeField] private List<Transform> targets = new List<Transform>();

    private Transform _target;
    private float _closestEnemyFromBase = 100000;

    private float _lastFireTime = 0;

    private SoundManager _soundManager;

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

    private void OnTriggerEnter(Collider col){ // Si un ennemi dans la range l'ajoute a la liste
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

    private void SetTarget(){
        if(randomTargets){ // Si cible aleatoire prends un ennemi aleatoire dans la liste d'ennemis
            if(targets.Count != 0){
                _target = targets[Random.Range(0, targets.Count)];
                if(targets[0] == null){
                    targets.RemoveAt(0);
                    SetTarget();
                    return;
                }
                if(headGO){
                    StartCoroutine(HeadFollow());
                }
            }else{
                _target = null;
            }
        }else{ // Cherche l'ennemi le plus proche de la base et attaque
            if(targets.Count != 0){
                if(targets[0] == null){
                    targets.RemoveAt(0);
                    SetTarget();
                    return;
                }
                _target = targets[0];
            }else{
                _target = null;
            }

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
    }

    private void CleanTargetList(){ // Clear les null
        for(int i = 0; i < targets.Count; i++){
            if(targets[i] == null){
                targets.RemoveAt(i);
            }
        }
    }

    IEnumerator HeadFollow(){ // Suis l'ennemi du regard
        while(_target){
            if(_target == null){
                SetTarget();
            }
            Fire();
            headGO.LookAt(_target);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void Fire(){ // Attaque la cible
        if(!projectile) return;

        if(Time.time >= _lastFireTime + attackCooldown){
            if(!_soundManager){
                _soundManager = SoundManager.instance;
            }
            _soundManager.Shoot();
            _lastFireTime = Time.time;
            GameObject _projectile = Instantiate(projectile, nuzzlePos[Random.Range(0, nuzzlePos.Length)].position, Quaternion.identity);
            _projectile.GetComponent<Projectile>().Target = _target.GetComponent<Enemy>().TargetPoint;
            _projectile.GetComponent<Projectile>().Damages = attackDamages;
        }
    }

    #endregion

}
