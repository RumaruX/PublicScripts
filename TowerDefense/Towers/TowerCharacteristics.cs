using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCharacteristics : MonoBehaviour
{

    #region Variables

    [Header("Tower Characteristics")]
    [SerializeField] private int baseCost;
    [SerializeField] private int sellingCost;
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject towerLevelup;
    [SerializeField] private GameObject rangeShow;

    private GameObject _towerSpawner;

    public float AttackRange{
        get{
            return attackRange;
        }
    }

    public GameObject TowerSpawner{
        get{
            return _towerSpawner;
        }
        set{
            _towerSpawner = value;
        }
    }

    public int Cost{
        get{
            return baseCost;
        }
    }

    public int SellCost{
        get{
            return sellingCost;
        }
    }

    public int UpgradeCost{
        get{
            if(towerLevelup){
                return towerLevelup.GetComponent<TowerCharacteristics>().Cost;
            }else{
                return 0;
            }
        }
    }

    private int _currentLevel;

    #endregion

    #region Properties

    #endregion

    #region Builtin Methods

    void Start()
    {
        GetComponent<SphereCollider>().radius = attackRange;
        rangeShow.GetComponent<Renderer>().sharedMaterial = new Material(rangeShow.GetComponent<Renderer>().sharedMaterial);
        rangeShow.GetComponent<Renderer>().sharedMaterial.SetFloat("_Range", attackRange/20);
        rangeShow.SetActive(false);
    }

    public virtual void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    public void ActivateRangeVisibility(bool activate = true){ // Active le shader de la range
        rangeShow.SetActive(activate);
    }

    public void LevelUp(){ // Upgrade la tour
        GameObject newTower = Instantiate(towerLevelup, transform.position, Quaternion.identity);
        newTower.GetComponent<TowerCharacteristics>().TowerSpawner = _towerSpawner;
        Destroy(gameObject);
    }

    #endregion

}
