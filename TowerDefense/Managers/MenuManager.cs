using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private GameObject options;

    #endregion

    #region Properties

    #endregion

    #region Built-in Methods

    #endregion

    #region Custom Methods

    public void RestartCurrentScene(){ // Relance la scene actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void LevelSelectNumber(int n){ // Selectionne la scene
        SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void LevelSelection(){ // Active le menu de selection de level
        levelSelection.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Options(){ // Active le menu des options
        options.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void InGameOptions(bool isActivated){ // Active le menu des options InGame
        options.SetActive(isActivated);
        if(isActivated){
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
        }
    }

    public void BackToMenu(){ // Remets le menu principal
        levelSelection.SetActive(false);
        options.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame(){ // Quitte le jeu
        Application.Quit();
    }

    #endregion

}
