using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Variables

    public static SoundManager instance;

    [SerializeField] private AudioSource[] UISounds;
    [SerializeField] private AudioSource[] attackSounds;
    [SerializeField] private AudioSource[] ambianceSounds;
    [SerializeField] private AudioSource[] backgroundSounds;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource earnMoney;
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource laserShoot;
    [SerializeField] private AudioSource killEnemy;

    private OptionsManager _optManager;
    

    #endregion

    #region Properties

    #endregion

    #region Built-in Methods

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }else{
            instance = this;
        }
    }

    void Start(){
        _optManager = OptionsManager.instance;
        _optManager.GetSoundManager();
    }

    #endregion

    #region Custom Methods

    // Update les sons dans les AudioSources

    public void UpdateUISounds(float n){
        foreach(AudioSource audio in UISounds){
            audio.volume = n/100f;
        }
    }

    public void UpdateAttackSounds(float n){
        foreach(AudioSource audio in attackSounds){
            audio.volume = n/100f;
        }
    }

    public void UpdateAmbianceSounds(float n){
        foreach(AudioSource audio in ambianceSounds){
            audio.volume = n/100f;
        }
    }

    public void UpdateBackgroundSounds(float n){
        foreach(AudioSource audio in backgroundSounds){
            audio.volume = n/100f;
        }
    }

    // Sons a jouer

    public void ButtonClick(){
        buttonClick.PlayOneShot(buttonClick.clip);
    }

    public void Money(){
        earnMoney.PlayOneShot(earnMoney.clip);
    }

    public void Shoot(){
        shoot.PlayOneShot(shoot.clip);
    }

    public void LaserShoot(){
        laserShoot.PlayOneShot(laserShoot.clip);
    }

    public void KillEnemy(){
        killEnemy.PlayOneShot(killEnemy.clip);
    }

    #endregion
}
