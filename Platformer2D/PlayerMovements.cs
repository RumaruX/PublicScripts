using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{

    #region Variables

    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private float speed = 300;
    [SerializeField] private float acceleration = 0.05f;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private Transform cam;

    private bool _isGrounded = false;
    private PlayerInteractions _interactions;
    private bool _facingRight = true;
    private Animator _anim;
    private Rigidbody2D _rb;
    private PlayerInputs _inputs;
    private Vector2 velocity;

    #endregion

    #region Custom Methods

    void GroundCheck(){ // Check si le sol est en dessous du joueur (si il touche le sol)

        _isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, _groundCheckRadius, groundLayer);
        foreach(Collider2D col in colliders){
            if(col.gameObject != gameObject){
                _isGrounded = true;
            }
        }

    }

    void Movement(){ // Mouvements du joueur

        Vector2 targetVelocity = new Vector2(_inputs.Movement.x * speed * Time.deltaTime, _rb.velocity.y);
        _rb.velocity = Vector2.SmoothDamp(_rb.velocity, targetVelocity, ref velocity, acceleration);
        _anim.SetBool("Move", true);
        if((_inputs.Movement.x < 0 && _facingRight) || (_inputs.Movement.x > 0 && !_facingRight)){
            Flip();
        }
        if(_inputs.Movement.x == 0){
            _anim.SetBool("Move", false);
        }

    }
 
    void Flip(){ // Change vers ou le joueur regarde

        _facingRight = !_facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }

    void Jump(){ // Saute

        if(_inputs.Jump && _isGrounded == true){
            _rb.velocity = new Vector3(0, jumpForce, 0);
        }

        if(_isGrounded){
            _anim.SetBool("Jump", false);
        }else{
            _anim.SetBool("Jump", true);
        }

    }

    #endregion

    #region Builtin Methods

    void Start(){

        _anim = GetComponent<Animator>();
        _inputs = GetComponent<PlayerInputs>();
        _interactions = GetComponent<PlayerInteractions>();
        _rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate(){

        if(!_interactions.IsDead){ // Check si le joueur est mort

            GroundCheck();

            Jump();

            Movement();

        }
        
    }

    #endregion

}
