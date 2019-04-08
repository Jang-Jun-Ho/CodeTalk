using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip soundExplosion; //재생할 소리를 변수로 담았다.
    public AudioClip winSound;//승리할 때 재생할 소리를 변수로 담았다
    public AudioClip menuClicksound;
    public AudioClip loseSound;
    AudioSource myAudio; //AudioSorce 컴포넌트를 변수로 담았다.
    public static SoundManager instance;  //자기자신을 변수로 담았다.
    void Awake() //Start보다도 먼저, 객체가 생성될때 호출됩니다
    {
        if (SoundManager.instance == null) //incetance가 비어있는지 검사
        {
            SoundManager.instance = this; //자기자신을 담습니다.
        }
    }
    void Start()
    {
        myAudio = this.gameObject.GetComponent<AudioSource>(); //AudioSource 오브젝트를 변수로 담습니다.
       
    }


    public void PlaySound()
    {
        myAudio.PlayOneShot(soundExplosion); //soundExplosion을 재생합니다.
       
    }

    public void WinSound()
    {
        myAudio.PlayOneShot(winSound);
    }
    public void LostSound()
    {

        myAudio.PlayOneShot(loseSound);
    }
    public void ClickSound()
    {
        myAudio.PlayOneShot(menuClicksound);
    }

    public void MenuCheckSound()
    {
        myAudio.PlayOneShot(menuClicksound); 
    }


    void Update()
    {

    }
}