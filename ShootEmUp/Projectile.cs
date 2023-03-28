using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class Projectile : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float bulletType = 1;
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private int damages = 1;
        private GameManager _gameManager;

        #endregion

        #region Builtin Methods

        void Start(){
            _gameManager = GameManager.instance;
        }

        void Update() // Deplacement
        {
            if(_gameManager.gameState == GameState.Resume)
                transform.Translate(0, 1 * Time.deltaTime * moveSpeed, 0);

        }

        void OnTriggerStay(Collider col){ // Fait des degats si reste en collision en tant que laser
            if(col.gameObject.tag == "Enemy" && gameObject.tag == "Bullet")
            {
                if(bulletType == 3){
                    col.gameObject.GetComponent<HealthController>().RemoveLife(damages + col.gameObject.GetComponent<HealthController>().Health / 500);
                    return;
                }
            }
        }
        void OnTriggerEnter(Collider col) // Detruit miunition et fait des degats si besoin
        {
           if(col.gameObject.name == "BulletEnd")
           {
               if(bulletType == 3){
                   return;
               }
               Destroy(gameObject);
           }
           if(col.gameObject.tag == "Enemy" && gameObject.tag == "Bullet")
           {
               if(bulletType == 3){ // Laser
                   col.gameObject.GetComponent<HealthController>().RemoveLife(damages * Time.deltaTime);
                   return;
               }
               col.gameObject.GetComponent<HealthController>().RemoveLife(damages); // Basique
               if(bulletType == 2 && col.gameObject.name != "Boss"){ //Type de munition 2 se dvise en 3
                    GameObject newBullet = Instantiate(gameObject, new Vector3(col.gameObject.transform.position.x + 2, col.gameObject.transform.position.y, col.gameObject.transform.position.z), Quaternion.identity);
                    newBullet.transform.rotation = Quaternion.Euler(90, 90, 0);
                    newBullet.GetComponent<Projectile>().bulletType = 1;
                    newBullet.transform.SetParent(transform.parent);
                    Destroy(newBullet, 10);
                    newBullet = Instantiate(gameObject, new Vector3(col.gameObject.transform.position.x - 2, col.gameObject.transform.position.y, col.gameObject.transform.position.z), Quaternion.identity);
                    newBullet.transform.rotation = Quaternion.Euler(90, -90, 0);
                    newBullet.GetComponent<Projectile>().bulletType = 1;
                    newBullet.transform.SetParent(transform.parent);
                    Destroy(newBullet, 10);
                    newBullet = Instantiate(gameObject, new Vector3(col.gameObject.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z + 2), Quaternion.identity);
                    newBullet.transform.rotation = Quaternion.Euler(90, 0, 0);
                    newBullet.GetComponent<Projectile>().bulletType = 1;
                    newBullet.transform.SetParent(transform.parent);
                    Destroy(newBullet, 10);
               }
               Destroy(gameObject);
           }
           if(col.gameObject.tag == "Player" && gameObject.tag == "EnemyBullet") // Retire vie si player touché
           {
               col.gameObject.GetComponent<HealthController>().RemoveLife(damages);
               Destroy(gameObject);
           }
        }
        #endregion

        #region Custom Methods

        public void ChangeType(int n){ // Change type de munition
            bulletType = n;
        }

        #endregion
    }
}