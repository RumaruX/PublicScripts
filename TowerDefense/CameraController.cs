using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    #region Variables

    [SerializeField] private float moveSpeed = 5f;
    // Min et max pour le deplacement de la camera
    [SerializeField] private float screenMargin;
    [SerializeField] private Vector2 levelMarginX;
    [SerializeField] private Vector2 levelMarginY;

    [SerializeField] private bool onlyKeys = true; // Si utilise uniquement le clavier pour le deplacement

    private float _horizontal;
    private float _vertical;

    private float _horizontalMouse;
    private float _verticalMouse;
    private float _mouseOffset = 10f;

    private float _screenWidth;
    private float _screenHeight;

    private Vector3 _movement = Vector3.zero;

    #endregion

    #region Properties

    #endregion

    #region Builtin Methods

    void Start(){
        OptionsManager.instance.GetCameraController(GetComponent<CameraController>());
    }

    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _horizontalMouse = Input.mousePosition.x;
        _verticalMouse = Input.mousePosition.y;

        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        if(_horizontal != 0 || _vertical != 0){
            _movement.Set(_horizontal, 0, _vertical); // Deplacement clavier
        }else if(!onlyKeys){
            if(_horizontalMouse < _mouseOffset){ // Deplacement souris
                if(_verticalMouse < _mouseOffset){
                    _movement.Set(-1, 0, -1);
                }else if(_verticalMouse > _screenHeight - _mouseOffset){
                    _movement.Set(-1, 0, 1);
                }else{
                    _movement.Set(-1, 0, 0);
                }
            }else if(_horizontalMouse > _screenWidth - _mouseOffset){
                if(_verticalMouse < _mouseOffset){
                    _movement.Set(1, 0, -1);
                }else if(_verticalMouse > _screenHeight - _mouseOffset){
                    _movement.Set(1, 0, 1);
                }else{
                    _movement.Set(1, 0, 0);
                }
            }else{
                if(_verticalMouse < _mouseOffset){
                    _movement.Set(0, 0, -1);
                }else if(_verticalMouse > _screenHeight - _mouseOffset){
                    _movement.Set(0, 0, 1);
                }else{
                    _movement = Vector3.zero;
                }
            }
        }else{
            _movement.Set(0, 0, 0);
        }

        transform.Translate(_movement * Time.deltaTime * moveSpeed);

        // Bordures check

        if(transform.position.x < levelMarginX.x){
            Vector3 newPos = new Vector3(levelMarginX.x, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
        if(transform.position.x > levelMarginX.y){
            Vector3 newPos = new Vector3(levelMarginX.y, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
        if(transform.position.z < levelMarginY.x){
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, levelMarginY.x);
            transform.position = newPos;
        }
        if(transform.position.z > levelMarginY.y){
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, levelMarginY.y);
            transform.position = newPos;
        }
    }

    #endregion

    #region Custom Methods

    public void SetOnlyKeys(bool isOnlyKeys){ // Change le fait de n'utiliser que le clavier
        onlyKeys = isOnlyKeys;
    }

    #endregion

}
