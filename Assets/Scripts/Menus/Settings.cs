using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public void SetFullScreen(bool isFullScren)
    {
        Screen.fullScreen = isFullScren;
    }

    public void SetWindowedScreen(bool isWindowed)
    {
        Screen.fullScreen = isWindowed;
    }

    public void Back()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}
