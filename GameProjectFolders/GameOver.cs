using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class GameOver : MonoBehaviour
{
    public GameObject congratulationsMessage;
    public GameObject gameOverMessage;
    public GameObject gameOverMenu;
    public Player player;
    public VolumeProfile volumeProfile;
    private Vignette vignette;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead)
        {
            gameOverMenu.SetActive(true);
            gameOverMessage.SetActive(true);
        }

        if (player.hitPortal)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameOverMenu.SetActive(true);
            congratulationsMessage.SetActive(true);
        }
    }
    
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

}
