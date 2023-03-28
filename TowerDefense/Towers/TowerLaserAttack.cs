using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLaserAttack : TowerCharacteristics
{

    #region Variables

    [Header("Tower Attacks")]
    [SerializeField] private Transform headGO;
    [SerializeField] private Transform nuzzlePos;
    [SerializeField] private Transform laserGO;
    [SerializeField] private int attackDamages;
    [SerializeField] private float attackCooldown;
    //[SerializeField] private bool longLaser = false;
    [SerializeField] private float attackMultiply = 0.5f;

    [SerializeField] private List<Transform> targets = new List<Transform>();

    private Transform _target;
    private float _closestEnemyFromBase = 100000;
    private int _actualDamages = 0;

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
                _actualDamages = attackDamages;
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
            _actualDamages = attackDamages;
            laserGO.localScale = new Vector3(laserGO.localScale.x, laserGO.localScale.x, laserGO.localScale.z);
            laserGO.localPosition = new Vector3(0, 0, 0);
        }

        // Cherche l'ennemi le plus proche de la base et attaque
        _closestEnemyFromBase = 100000;

        foreach(Transform target in targets){
            if(target != null){
                Enemy currentEnemy = target.GetComponent<Enemy>();
                float distanceToBase = currentEnemy.GetRemainingDistance();
                if(distanceToBase < _closestEnemyFromBase){
                    _closestEnemyFromBase = distanceToBase;
                    if(_target != target){
                        _actualDamages = attackDamages;
                    }
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
        if(_actualDamages == 0){
            _actualDamages = attackDamages;
        }
        while(_target){
            if(_target == null){
                SetTarget();
            }
            laserGO.localScale = new Vector3(laserGO.localScale.x, Vector3.Distance(nuzzlePos.position, _target.GetComponent<Enemy>().TargetPoint.position)/2f, laserGO.localScale.z);
            laserGO.localPosition = new Vector3(0, 0, Vector3.Distance(nuzzlePos.position, _target.GetComponent<Enemy>().TargetPoint.position)/2f);
            Fire();
            headGO.LookAt(_target.GetComponent<Enemy>().TargetPoint);
            yield return new WaitForEndOfFrame();
        }
        _actualDamages = attackDamages;
        yield return null;
    }

    private void Fire(){ // Attaque la cible et multiplie les degats si la cible n'a pas change
        if(Time.time >= _lastFireTime + attackCooldown){
            if(_soundManager == null){
                _soundManager = SoundManager.instance;
            }else{
                _soundManager.Shoot();
            }
            _lastFireTime = Time.time;
            _target.GetComponent<Enemy>().RemoveHp(_actualDamages);
            _actualDamages = _actualDamages + Mathf.CeilToInt(_actualDamages * attackMultiply);
            Debug.Log(_actualDamages);
        }
    }

    #endregion

}
