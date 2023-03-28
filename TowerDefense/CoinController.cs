using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{

    #region Variables

    [SerializeField] private float rotationSpeed = 0.5f; // Vitesse de rotation

    private int _coinValue = 1; // Valeur de la piece

    private GameManager _gameManager;

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
        transform.eulerAngles += new Vector3(0, rotationSpeed, 0); // Rotation de la piece
    }

    void OnMouseOver(){ // Ajoute de l'argent et detruit la piece quand on la survole avec la souris
        _gameManager.AddMoney(_coinValue);
        Destroy(gameObject);
    }

    #endregion

    #region Custom Methods

    public void SetValue(int n){ // Mets la valeur de la piece
        _coinValue = n;
    }

    #endregion

}
