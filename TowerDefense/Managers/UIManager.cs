using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text life;
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject win;

    [SerializeField] private bool mainMenu = false;

    [SerializeField] private TMP_Text onlyKeysText;
    [SerializeField] private Button onlyKeys;
    [SerializeField] private Slider mainSoundSlider;
    [SerializeField] private Slider attacksSoundSlider;
    [SerializeField] private Slider UISoundSlider;
    [SerializeField] private Slider ambianceSoundSlider;
    [SerializeField] private Slider backgroundSoundSlider;

    private OptionsManager _optManager;

    public static UIManager instance;

    #endregion

    #region Properties

    public Button OnlyKeys => onlyKeys;
    public Slider MainSoundSlider => mainSoundSlider;
    public Slider AttackSoundSlider => attacksSoundSlider;
    public Slider UiSoundSlider => UISoundSlider;
    public Slider AmbianceSoundSlider => ambianceSoundSlider;
    public Slider BackgroundSoundSlider => backgroundSoundSlider;

    #endregion

    #region Builtin Methods

    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
            return;
        }else{
            instance = this;
        }
    }

    void Start()
    {
        _optManager = OptionsManager.instance;
        if(!mainMenu){
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
            gameOver.SetActive(false);
            win.SetActive(false);
            blackScreen.gameObject.SetActive(false);
        }
        // Change les valeurs des sliders et boutons et cree des listeners pour l'options manager
        onlyKeysText.text = "Only use keys = " + _optManager.IsKeysOnly.ToString();
        mainSoundSlider.value = _optManager.MainSound;
        attacksSoundSlider.value = _optManager.AttacksSound;
        UiSoundSlider.value = _optManager.UISound;
        ambianceSoundSlider.value = _optManager.AmbianceSound;
        backgroundSoundSlider.value = _optManager.BackgroundSound;
        onlyKeys.onClick.AddListener(() => _optManager.KeysOnly(onlyKeysText));
        mainSoundSlider.onValueChanged.AddListener((sliderValue) => _optManager.ChangeMainSoundValue(mainSoundSlider));
        attacksSoundSlider.onValueChanged.AddListener((sliderValue) => _optManager.ChangeAttackSoundValue(attacksSoundSlider));
        UiSoundSlider.onValueChanged.AddListener((sliderValue) => _optManager.ChangeUISoundValue(UiSoundSlider));
        ambianceSoundSlider.onValueChanged.AddListener((sliderValue) => _optManager.ChangeAmbianceSoundValue(ambianceSoundSlider));
        backgroundSoundSlider.onValueChanged.AddListener((sliderValue) => _optManager.ChangeBackgroundSoundValue(backgroundSoundSlider));
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    public void UpdateMoney(int moneyToUpdate){ // Montre l'argent
        money.text = moneyToUpdate.ToString();
    }

    public void UpdateLife(int lifeToUpdate){ // Montre la vie
        life.text = lifeToUpdate.ToString();
    }

    public void GameOver(){ // Perdu
        StartCoroutine(EndGame(false));
    }

    public void Win(){  // Gagne
        StartCoroutine(EndGame(true));
    }

    IEnumerator EndGame(bool hasWon = true){ // Fondu et ecran (en fonction de si on gagne ou non)
        blackScreen.gameObject.SetActive(true);
        for(int i = 0; i < 101; i++){
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, (float)i/100);
            yield return new WaitForSeconds(0.001f);
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1);
        if(hasWon){
            win.SetActive(true);
        }else{
            gameOver.SetActive(true);
        }
        for(int i = 0; i < 101; i++){
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1-(float)i/100);
            yield return new WaitForSeconds(0.001f);
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
        blackScreen.gameObject.SetActive(false);
    }

    #endregion

}
