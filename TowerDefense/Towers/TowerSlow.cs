using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlow : TowerCharacteristics
{

    #region Variables

    [Header("Tower Effects")]
    [SerializeField] private float speedImpact = 0.75f; // Impact de la vitesse (multiplicateur)
    [SerializeField] private string effectName = "Slow enemies";

    private Dictionary<Transform, float> targets = new Dictionary<Transform, float>(); // Dictionnaire des vitesses initiales des ennemis

    #endregion

    #region Properties

    public string Effect{ // Description de l'effet
        get{
            return effectName;
        }
    }

    public float AttackSpeed{ // Multiplicateur de vitesse
        get{
            return speedImpact;
        }
    }

    #endregion

    #region Built-in Methods

    private void OnTriggerEnter(Collider col){ // Si un ennemi n'est pas affecte par une slow tower et qu'il entre dans sa zone d'attaque, enregistre sa speed initiale et le ralentit
        if(col.gameObject.layer != LayerMask.NameToLayer("Enemies")){
            return;
        }else{
            if(!col.transform.GetComponent<Enemy>().AffectedSpeed){
                targets.Add(col.transform, col.transform.GetComponent<Enemy>().Speed);
                col.transform.GetComponent<Enemy>().AffectedSpeed = true;
                col.transform.GetComponent<Enemy>().Speed = speedImpact * col.transform.GetComponent<Enemy>().Speed;
                col.transform.GetComponent<Enemy>().SlowTower = transform;
            }
        }
    }

    private void OnTriggerExit(Collider col){ // Si un ennemi est affecte par cette slow tower et qu'il quitte sa zone d'attaque, remets sa speed initiale
        if(col.gameObject.layer != LayerMask.NameToLayer("Enemies")){
            return;
        }else{
            if(col.transform.GetComponent<Enemy>().AffectedSpeed){
                if(col.transform.GetComponent<Enemy>().SlowTower == transform){
                    col.transform.GetComponent<Enemy>().Speed = targets[col.transform];
                    col.transform.GetComponent<Enemy>().AffectedSpeed = false;
                    targets.Remove(col.transform);
                }
            }
        }
    }

    #endregion

    #region Custom Methods

    public void BeforeDestroy(){ // Remets la speed initiale des ennemis
        foreach(Transform enemy in targets.Keys){
            if(enemy != null){
                enemy.GetComponent<Enemy>().Speed = targets[enemy];
                enemy.GetComponent<Enemy>().AffectedSpeed = false;
            }
        }
    }

    #endregion

}
