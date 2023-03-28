using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    [SerializeField] private string nextScene;
    
    private GameManager _gameManager;

    void Start(){ 

        _gameManager = GameManager.instance;

    }
    
    public void SwitchLevel(){ // Change de niveau et detruit l'instance pour pas de bug

        _gameManager.Ending();
        SceneManager.LoadScene(nextScene);

    }

}
