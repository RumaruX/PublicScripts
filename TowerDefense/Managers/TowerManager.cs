using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private GameObject towerCanvas;

    [SerializeField] private GameObject towerMachineGunBlueprint;
    [SerializeField] private GameObject towerFlamethrowerBlueprint;
    [SerializeField] private GameObject towerLaserBlueprint;
    [SerializeField] private GameObject towerSlowBlueprint;
    [SerializeField] private GameObject towerRocketsBlueprint;

    private GameManager _gameManager;

    private GameObject _newCanvas;
    private GameObject _targetTower;

    #endregion

    #region Properties

    #endregion

    #region Builtin Methods

    void Start()
    {
        _gameManager = GameManager.instance;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){ // Si click gauche sur une tour, crée un canvas
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                if(hit.transform.tag == "Tower"){
                    if(_targetTower){ // Desctive le shader pour montrer la zone de detection de l'ancienne tour (si il y avait une ancienne tour)
                        _targetTower.GetComponent<TowerCharacteristics>().ActivateRangeVisibility(false);
                    }
                    _targetTower = hit.transform.gameObject;
                    _targetTower.GetComponent<TowerCharacteristics>().ActivateRangeVisibility(true); // Active le shader pour montrer la zone de detection
                    if(_newCanvas != null){
                        Destroy(_newCanvas); // Supprime l'ancien canvas (si il existe)
                    }
                    _newCanvas = Instantiate(towerCanvas); // Cree le nouveau canvas
                    if(HaveUpgrade()){ // Si il y a une upgrade, laisse le bouton, sinon, le cache
                        _newCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        _newCanvas.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => Upgrade());
                    }else{
                        _newCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }
                    _newCanvas.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => Sell());
                    ShowStats();
                    _newCanvas.transform.position = hit.transform.position + new Vector3(0, 1, -2.5f); // Change la position du canvas pour une meilleure visibilite
                }
            }
        }
        if(Input.GetMouseButtonDown(1)){ // Si click droit supprime le canvas et cache le shader de la zone de detection de la tour precedemment selectionnee (si ils existent)
            if(_newCanvas != null){
                Destroy(_newCanvas);
            }
            if(_targetTower){
                _targetTower.GetComponent<TowerCharacteristics>().ActivateRangeVisibility(false);
            }
        }
    }

    #endregion

    #region Custom Methods

    public void InstantiateMachineGun(){ // Cree une machineGun tower
        if(_gameManager.HoldingBlueprint){
            Destroy(_gameManager.ActualBlueprint);
        }
        GameObject.Instantiate(towerMachineGunBlueprint, Vector3.zero, Quaternion.identity);
    }

    public void InstantiateFlamethrower(){ // Cree une flameThrower tower
        if(_gameManager.HoldingBlueprint){
            Destroy(_gameManager.ActualBlueprint);
        }
        GameObject.Instantiate(towerFlamethrowerBlueprint, Vector3.zero, Quaternion.identity);
    }

    public void InstantiateLaser(){ // Cree une laser tower
        if(_gameManager.HoldingBlueprint){
            Destroy(_gameManager.ActualBlueprint);
        }
        GameObject.Instantiate(towerLaserBlueprint, Vector3.zero, Quaternion.identity);
    }

    public void InstantiateSlow(){ // Cree une slow tower
        if(_gameManager.HoldingBlueprint){
            Destroy(_gameManager.ActualBlueprint);
        }
        GameObject.Instantiate(towerSlowBlueprint, Vector3.zero, Quaternion.identity);
    }

    public void InstantiateRockets(){ // Cree une slow tower
        if(_gameManager.HoldingBlueprint){
            Destroy(_gameManager.ActualBlueprint);
        }
        GameObject.Instantiate(towerRocketsBlueprint, Vector3.zero, Quaternion.identity);
    }

    bool CanUpgrade(){ // Check si il y a assez d'argent pour acheter l'upgrade
        return _gameManager.CanBuy(_targetTower.GetComponent<TowerCharacteristics>().UpgradeCost);
    }

    bool HaveUpgrade(){ // Check si il y a une upgrade
        if(_targetTower.GetComponent<TowerCharacteristics>().UpgradeCost == 0){
            return false;
        }
        return true;
    }

    public void ShowStats(){
        if(!_newCanvas){ // Si le canvas existe
            return;
        }
        // Affiche les 2 premieres stats sur le canvas
        TowerAttack attackScript;
        _targetTower.TryGetComponent<TowerAttack>(out attackScript);
        if(attackScript){ // Base attack
            _newCanvas.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Attack : " + attackScript.AttackDamages.ToString();
            _newCanvas.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "Speed : " + attackScript.AttackSpeed.ToString();
        }else{ // Attack with effect after
            TowerAfterAttack afterAttackScript;
            _targetTower.TryGetComponent<TowerAfterAttack>(out afterAttackScript);
            if(afterAttackScript){
                _newCanvas.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Attack : " + afterAttackScript.AttackDamages.ToString();
                _newCanvas.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "Speed : " + afterAttackScript.AttackSpeed.ToString();
            }else{ // Laser Attack
                TowerLaserAttack attackLaserScript;
                _targetTower.TryGetComponent<TowerLaserAttack>(out attackLaserScript);
                if(attackLaserScript){
                    _newCanvas.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Attack : " + attackLaserScript.AttackDamages.ToString();
                    _newCanvas.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "Speed : " + attackLaserScript.AttackSpeed.ToString();
                }else{ // Slow effect
                    _newCanvas.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Effect : " + _targetTower.GetComponent<TowerSlow>().Effect;
                    _newCanvas.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "Speed : x" + _targetTower.GetComponent<TowerSlow>().AttackSpeed.ToString();
                }
            }
        }
        // Affiche la detection range
        _newCanvas.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = "Range : " + _targetTower.GetComponent<TowerCharacteristics>().AttackRange.ToString();
        // Affiche le prix de l'upgrade si il y en a une, sinon dis qu'il n'y en a pas
        if(HaveUpgrade()){
            _newCanvas.transform.GetChild(1).GetChild(3).GetComponent<TMP_Text>().text = "Upgrade cost : " + _targetTower.GetComponent<TowerCharacteristics>().UpgradeCost.ToString();
            if(CanUpgrade()){ // Si peut acheter, montre le bouton avec la couleur initiale, sinon, le montre en rouge
                _newCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }else{
                _newCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 0, 0, 0.25f);
            }
        }else{
            _newCanvas.transform.GetChild(1).GetChild(3).GetComponent<TMP_Text>().text = "No other upgrade.";
        }
        // Affiche le prix de vente
        _newCanvas.transform.GetChild(1).GetChild(4).GetComponent<TMP_Text>().text = "Selling for : " + _targetTower.GetComponent<TowerCharacteristics>().SellCost.ToString();
    }

    public void Upgrade(){
        if(HaveUpgrade()){ // Si il y a une upgrade
            if(CanUpgrade()){ // Si on a assez d'argent
                // Si slow tower, remets la speed des ennemis a leur speed initiale
                TowerSlow slowScript;
                _targetTower.TryGetComponent<TowerSlow>(out slowScript);
                if(slowScript){
                    slowScript.BeforeDestroy();
                }
                // Retire l'argent et detruit la tour et le canvas
                _gameManager.RemoveMoney(_targetTower.GetComponent<TowerCharacteristics>().UpgradeCost);
                _targetTower.GetComponent<TowerCharacteristics>().LevelUp();
                if(_newCanvas != null){
                    Destroy(_newCanvas);
                }
                // Ajout particules
            }
        }
    }

    public void Sell(){
        // Si slow tower, remets la speed des ennemis a leur speed initiale
        TowerSlow slowScript;
        _targetTower.TryGetComponent<TowerSlow>(out slowScript);
        if(slowScript){
            slowScript.BeforeDestroy();
        }
        // Ajoute l'argent et détruit la tour et le canvas
        _gameManager.AddMoney(_targetTower.GetComponent<TowerCharacteristics>().SellCost);
        _targetTower.GetComponent<TowerCharacteristics>().TowerSpawner.GetComponent<TowerSpawner>().IsBusy = false;
        Destroy(_targetTower);
        if(_newCanvas != null){
            Destroy(_newCanvas);
        }
        // Ajout particules
    }

    #endregion

}
