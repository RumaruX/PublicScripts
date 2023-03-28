using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInformation : MonoBehaviour
{

    [SerializeField] private string info;
    [SerializeField] private GameObject bulle;
    [SerializeField] private Text infoText;

    private bool _isActive = false;

    void Start(){

        bulle.SetActive(false);

    }

    public void Activate(){ // Si interaction, montre le texte

        if(_isActive == false){

            _isActive = true;
            bulle.SetActive(true);
            infoText.text = "";
            StartCoroutine(ShowText());

        }

    }

    IEnumerator ShowText(){ // Montre le texte avec attente entre chaque lettre

        foreach(char letter in info){
            infoText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(10);
        bulle.SetActive(false);
        _isActive = false;

    }

}
