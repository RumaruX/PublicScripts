using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    
    #region Variables

    private bool _jump = false;
    private bool _interaction = false;
    private Vector2 _movement = Vector2.zero;

    #endregion

    #region Properties

    public bool Jump => _jump;
    public bool Interaction => _interaction;
    public Vector2 Movement => _movement;

    #endregion

    #region Builtin Methods

    void Update(){ // Inputs du joueur
        _movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _jump = Input.GetButton("Jump");
        _interaction = Input.GetButton("Submit");
    }
    
    #endregion

}
