using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{

    #region Variables

    private bool _isBusy = false;

    #endregion

    #region Properties

    public bool IsBusy{
        get{
            return _isBusy;
        }
        set{
            _isBusy = value;
        }
    }

    #endregion

    #region Builtin Methods

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    #endregion

}
