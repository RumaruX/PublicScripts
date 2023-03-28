using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class Coin : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private float propulsionStrength = 5; // Force de propulsion

        private Rigidbody _rb; // Rigidbody de la piece

        #endregion

        #region Builtin Methods
        void OnEnable()
        {

            // Fait un effet de propulsion quand la piece est créée

            _rb = GetComponent<Rigidbody>();
            _rb.velocity = new Vector3(Random.Range(-propulsionStrength, propulsionStrength), 0, Random.Range(-propulsionStrength, propulsionStrength));
            _rb.angularVelocity = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));

        }

        void Update()
        {
            _rb.velocity = _rb.velocity / 1.05f; // Permets de raletir la pièce quand elle se déplace pour un meilleur magnet
        }
        #endregion

        #region Custom Methods
        
        #endregion
    }
}
