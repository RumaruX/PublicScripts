using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class LevelManager : MonoBehaviour
    {
        #region Variables



        #endregion

        #region Builtin Methods
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region Custom Methods

        public void LoadGameplayScene(){
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        public void LoadGameplay2Scene(){
            SceneManager.LoadScene("Gameplay2", LoadSceneMode.Single);
        }

        public void LoadMenuScene(){
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }

        public void End(){
            Application.Quit();
        }

        #endregion
    }
}
