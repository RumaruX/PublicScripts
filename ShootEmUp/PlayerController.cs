using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts
{
    public class PlayerController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler // Ajout pour smartphone
    {
        #region Variables

        [Header("Move Properties")]
        [SerializeField] private float moveSpeed = 2; // Vitesse du joueur
        [SerializeField] private float dragOffsetZ = 0;
        [SerializeField] private float dragLerpMultiplier = 1;
        [Header("Bullets Properties")]
        [SerializeField] private float muniSpeed = 1;
        private float _actualMuniSpeed;

        [SerializeField] private GameObject shooter;
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject laser;
        [SerializeField] private Transform bulletParent;
        [SerializeField] private Transform firstShoot;
        [SerializeField] private Transform secondShootL;
        [SerializeField] private Transform secondShootR;
        public int _shootVer = 1; // Type de tir

        private PointerEventData _data;
        private InputController _inputs; // Controlleur d'inputs
        private Camera _cam;
        private Vector3 _wantedPos;
        private bool _isDragging = false;
        private bool _canShoot = true;
        private bool _canBeHit = true;
        private bool _canMove = true;
        private SoundManager _soundManager;
        private GameManager _gameManager;
        private int _bulletType = 1;

        //private int _shootVer = 1; // Type de tir

        /*
        Tir 1 : 1 munition au centre
        Tir 2 : Une munition de chaque cote
        Tir 3 : Une munition de chaque cote et devant
        */

        #endregion

        #region Builtin Methods
        void Start()
        {

            _inputs = GetComponent<InputController>(); // Obtension du controlleur d'inputs
            _cam = Camera.main;
            _actualMuniSpeed = muniSpeed;
            _soundManager = SoundManager.instance;
            _gameManager = GameManager.instance;

        }

        void Update()
        {
            if(_gameManager.gameState == GameState.Resume)
                Move();
                Shoot();

        }
        #endregion

        #region Custom Methods

        void Shoot() // Tire avec mode de tir
        {
            if(_gameManager.gameState == GameState.Resume)
                if(_inputs.IsShooting && _canShoot){
                    if(_shootVer == 1)
                    {
                        CreateBullet(firstShoot);
                    }
                    if(_shootVer == 2)
                    {
                        CreateBullet(secondShootL);
                        CreateBullet(secondShootR);
                    }
                    if(_shootVer == 3 || _shootVer == 4)
                    {
                        CreateBullet(firstShoot);
                        CreateBullet(secondShootL);
                        CreateBullet(secondShootR);
                    }
                    if(_shootVer != 5)
                        _soundManager.Shoot();
                    if(_shootVer == 5)
                    {
                        StartCoroutine(Laser(firstShoot));
                    }
                    _canShoot = false;
                    StartCoroutine(ShootTimer());
                }

        }

        void CreateBullet(Transform pos){ // Cree une munition
            GameObject newBullet = Instantiate(bullet, pos);
            newBullet.transform.SetParent(bulletParent);
            newBullet.GetComponent<Projectile>().ChangeType(_bulletType);
            if(_shootVer == 4){
                if(pos.gameObject.name != "FirstAttack"){
                    if(pos.gameObject.name.Substring(12, 1) == "L"){
                        newBullet.transform.Rotate(0, 0, 10);
                        newBullet.GetComponent<Projectile>().ChangeType(_bulletType);
                    }else{
                        newBullet.transform.Rotate(0, 0, -10);
                        newBullet.GetComponent<Projectile>().ChangeType(_bulletType);
                    }
                }
            }
        }

        public void ChangeAttackType(){ // Change type d'attaque
            _shootVer += 1;
            if(_shootVer == 6){
                _shootVer = 1;
            }
        }

        public void ChangeBulletType(){ // Change type de munition
            _bulletType += 1;
            if(_bulletType == 3){
                _bulletType = 1;
            }
        }

        public void ActivateSpeedBoost(){ // Active boost vitesse d'attaque
            if(_actualMuniSpeed != muniSpeed) StopCoroutine(SpeedBoostTimer());
            StartCoroutine(SpeedBoostTimer());
            _actualMuniSpeed = muniSpeed / 4;
        }

        IEnumerator SpeedBoostTimer(){ // Timer de boost de vitesse d'attaque

            yield return new WaitForSeconds(6);
            for(int i = 0; i < 4; i++){
                yield return new WaitForSeconds(1);
            }
            _actualMuniSpeed = muniSpeed;

        }

        IEnumerator Laser(Transform pos){ // Attaque laser
            _canMove = false;
            GameObject newBullet = Instantiate(laser, pos);
            newBullet.transform.SetParent(bulletParent);
            Destroy(newBullet, 10);
            for(int i = 0; i < 180; i++){
                if(newBullet.transform.position.x == transform.position.x)
                    newBullet.transform.localScale += new Vector3(0, 0.135f, 0);
                shooter.transform.Rotate((Mathf.Abs(Mathf.Abs(i) - 180)), 0, 0);
                yield return new WaitForSeconds(0.001f);
                if(i == 90){
                    shooter.transform.Rotate(-90, 0, 0);
                }
            }
            shooter.transform.rotation = Quaternion.Euler(-90, -90, 0);
            _shootVer = 4;
            _canMove = true;
        }

        IEnumerator ShootTimer() // Timer shoot
        {
            yield return new WaitForSeconds(_actualMuniSpeed);
            _canShoot = true;
        }

        void Move() // Deplacement du personnage
        {
            if(_canMove){
                _wantedPos = transform.position;
                if(_data != null && _isDragging)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(_cam.ScreenPointToRay(_data.position), out hit, 100, 1<<31)) // Si player touché
                    {
                        _wantedPos = Vector3.Lerp(transform.position, hit.point + Vector3.forward * dragOffsetZ, Time.deltaTime * dragLerpMultiplier * moveSpeed);
                        transform.position = new Vector3(_wantedPos.x, 0, _wantedPos.z);
                    }
                }
                else
                {
                    transform.Translate(_inputs.Horizontal * moveSpeed * Time.deltaTime, 0, _inputs.Vertical * moveSpeed * Time.deltaTime);
                    //_wantedPos = transform.position + (Vector3.right * _inputs.Horizontal + Vector3.forward * _inputs.Vertical).normalized * moveSpeed * Time.deltaTime;
                }
                //transform.position = new Vector3(_wantedPos.x, 0, _wantedPos.z);
            }

        }

        void OnTriggerEnter(Collider col){

            if(col.tag == "Enemy" && _canBeHit == true){ // retire de la vie si touche par un ennemi

                GetComponent<HealthController>().RemoveLife(1);
                _canBeHit = false;
                StartCoroutine(Invincibility(1f));

            }

        }

        // Deplacement par clic
        public void OnBeginDrag(PointerEventData eventdata)
        {
            _data = eventdata;
        }

        public void OnEndDrag(PointerEventData eventdata)
        {
            _data = eventdata;
        }

        public void OnDrag(PointerEventData eventdata)
        {
            _data = eventdata;
        }

        public void OnPointerDown(PointerEventData eventdata)
        {
            _data = eventdata;
            _isDragging = true;
        }

        public void OnPointerUp(PointerEventData eventdata)
        {
            _data = eventdata;
            _isDragging = false;
        }

        #endregion

        #region Custom Methods

        IEnumerator Invincibility(float timer){ // temps invincible si touché
            yield return new WaitForSeconds(timer);
            _canBeHit = true;
        }

        #endregion
    }
}