using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private AudioSource btnAudioSource;

    public void Start()
    {
        btnAudioSource = GetComponents<AudioSource>()[1];
    }

    public void Play() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ButtonHover()
    {
        btnAudioSource.Play();
    }
}
