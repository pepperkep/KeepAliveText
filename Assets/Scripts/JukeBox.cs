using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{
    // Only one juke box should exist per scene, this is an efficient way of accessing it anywhere
    private static JukeBox _jukeBox;
    public static JukeBox Instance() { return _jukeBox; }

    private void Awake()
    {
        if(!overrideOldJukebox && _jukeBox != null)
            Destroy(this.gameObject);
        else
        {
            if(_jukeBox != null)
                Destroy(_jukeBox.gameObject);
            JukeBox._jukeBox = this;
        }
    }




    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource alternateMusic;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource loseSound;
    [SerializeField] private AudioSource rightWordSound;
    [SerializeField] private AudioSource wrongWordSound;
    [SerializeField] private AudioSource lowHealthSound;
    [SerializeField] private AudioSource messageSentSound;
    [SerializeField] private bool keepOnSceneChange;
    [SerializeField] private bool overrideOldJukebox = true;
    private bool altMusicOn = false;
    private bool backupPlaying = false;


    void Start()
    {
        alternateMusic.Play();
        alternateMusic.Pause();
        music.Play();

        if(keepOnSceneChange)
            DontDestroyOnLoad(this.gameObject);


    }

    public void SwitchMusic()
    {
        if(!altMusicOn)
        {
            music.Pause();
            alternateMusic.Play();
            alternateMusic.time = music.time;
        }
        else
        {
            alternateMusic.Pause();
            music.Play();
            music.time = alternateMusic.time;
        }
        altMusicOn = !altMusicOn;
    }



    public void playRightWordSound()
    {
        rightWordSound.Play();
    }

    public void playWrongWordSound()
    {
        wrongWordSound.Play();
    }
    public void playLowHealthSound()
    {
        lowHealthSound.Play();
    }
    public void pauseLowHealthSound()
    {
        lowHealthSound.Pause();
    }

    public void playMessageSentSound()
    {
        messageSentSound.Play();
    }

    public void playWinSound()
    {
        winSound.Play();
        music.Pause();
        alternateMusic.Pause();
    }

    public void playLoseSound()
    {
        loseSound.Play();
        music.Pause();
        alternateMusic.Pause();
    }


}
