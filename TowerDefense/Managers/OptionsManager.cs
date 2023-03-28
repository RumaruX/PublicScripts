using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{

    public static OptionsManager instance;

    private bool keysOnly = true;
    private int mainSoundLevel = 100;
    private int attacksSoundLevel = 2;
    private int UISoundLevel = 30;
    private int ambianceSoundLevel = 15;
    private int backgroundSoundLevel = 10;

    private SoundManager _soundManager;
    private CameraController _camController;

    public bool IsKeysOnly => keysOnly;
    public int MainSound => mainSoundLevel;
    public int AttacksSound => attacksSoundLevel;
    public int UISound => UISoundLevel;
    public int AmbianceSound => ambianceSoundLevel;
    public int BackgroundSound => backgroundSoundLevel;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Scripts pour changer les valeurs des variables permettant de gerer le son

    public void ChangeMainSoundValue(Slider s){
        mainSoundLevel = Mathf.RoundToInt(s.value);
        _soundManager.UpdateAttackSounds((mainSoundLevel/100f)*attacksSoundLevel);
        _soundManager.UpdateUISounds((mainSoundLevel/100f)*UISoundLevel);
        _soundManager.UpdateAmbianceSounds((mainSoundLevel/100f)*ambianceSoundLevel);
        _soundManager.UpdateBackgroundSounds((mainSoundLevel/100f)*backgroundSoundLevel);
    }

    public void ChangeAttackSoundValue(Slider s){
        attacksSoundLevel = Mathf.RoundToInt(s.value);
        _soundManager.UpdateAttackSounds((mainSoundLevel/100f)*attacksSoundLevel);
    }

    public void ChangeUISoundValue(Slider s){
        UISoundLevel = Mathf.RoundToInt(s.value);
        _soundManager.UpdateUISounds((mainSoundLevel/100f)*UISoundLevel);
    }

    public void ChangeAmbianceSoundValue(Slider s){
        ambianceSoundLevel = Mathf.RoundToInt(s.value);
        _soundManager.UpdateAmbianceSounds((mainSoundLevel/100f)*ambianceSoundLevel);
    }

    public void ChangeBackgroundSoundValue(Slider s){
        backgroundSoundLevel = Mathf.RoundToInt(s.value);
        _soundManager.UpdateBackgroundSounds((mainSoundLevel/100f)*backgroundSoundLevel);
    }

    public void KeysOnly(TMP_Text t){ // Change en deplacement clavier uniquement ou inversement
        keysOnly = !keysOnly;
        t.text = "Only use keys = " + keysOnly.ToString();
        if(_camController != null){
            _camController.SetOnlyKeys(keysOnly);
        }
    }

    public void GetSoundManager(){ // Obtention sound manager
        _soundManager = SoundManager.instance;
        _soundManager.UpdateAttackSounds((mainSoundLevel/100f)*attacksSoundLevel);
        _soundManager.UpdateUISounds((mainSoundLevel/100f)*UISoundLevel);
        _soundManager.UpdateAmbianceSounds((mainSoundLevel/100f)*ambianceSoundLevel);
        _soundManager.UpdateBackgroundSounds((mainSoundLevel/100f)*backgroundSoundLevel);
    }

    public void GetCameraController(CameraController camController){ // Obtention camera controller
        _camController = camController;
        _camController.SetOnlyKeys(keysOnly);
    }
    
}
