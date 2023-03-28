using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintController : MonoBehaviour
{

    #region Variables

    [SerializeField] private GameObject tower;
    [SerializeField] private Material towerMaterial;
    private int _cost = 999;

    private int _groundLayerMask = 1<<6;
    private int _spawnerLayerMask = 1<<7;

    private GameManager _gameManager;

    #endregion

    #region Properties

    #endregion

    #region Builtin Methods

    void Start()
    {
        _gameManager = GameManager.instance;
        _cost = tower.GetComponent<TowerCharacteristics>().Cost;
        _gameManager.HoldingBlueprint = true;
        _gameManager.ActualBlueprint = gameObject;
    }

    void Update()
    {
        // Placement du bblueprint
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, _groundLayerMask)){
            transform.position = hit.point;
        }

        if(Input.GetMouseButton(1)){ // Le supprime si click droit
            _gameManager.HoldingBlueprint = false;
            Destroy(gameObject);
        }

        // Changement du materiel
        towerMaterial.color = new Color(1, 0, 0, towerMaterial.color.a);

        // Si peut acheter et sur un bon endroit, change le materiel, et si clic gauche avec ces conditions, place la tour et retire l'argent
        if(Physics.Raycast(ray, out hit, 100, _spawnerLayerMask)){
            transform.position = hit.collider.transform.position;
            if(!hit.collider.GetComponent<TowerSpawner>().IsBusy){
                if(_gameManager.CanBuy(_cost)){
                    towerMaterial.color = new Color(0, 0.9f, 1, towerMaterial.color.a);
                    if(Input.GetMouseButton(0)){
                        _gameManager.RemoveMoney(_cost);
                        GameObject newTurret = Instantiate(tower, hit.collider.transform.position, Quaternion.identity);
                        newTurret.GetComponent<TowerCharacteristics>().TowerSpawner = hit.collider.gameObject;
                        hit.collider.GetComponent<TowerSpawner>().IsBusy = true;
                        _gameManager.HoldingBlueprint = false;
                        Destroy(gameObject);
                    }
                }else{
                    towerMaterial.color = new Color(1, 0, 0, towerMaterial.color.a);
                }
            }else{
                towerMaterial.color = new Color(1, 0, 0, towerMaterial.color.a);
            }
        }
    }

    #endregion

    #region Custom Methods

    #endregion

}
