using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControll : MonoBehaviour {

    [Header("General parameters")]
    [Tooltip("count of player life. If life count = 0, game over")]
    public int lifeCount;
    [Tooltip("Player controller script here")]
    public PlayerController player;
    [Tooltip("Player inventory script")]
    public Inventory inventory;
    [Tooltip("Enemy gameobject")]
    public Enemy enemy;
    [Tooltip("Player spawn point")]
    public Transform playerSpawnPoint;
    [Tooltip("Enemy spawn point")]
    public Transform enemySpawnPoint;
    [Tooltip("Screen fade gameobject")]
    public AudioSource dangerAmbient;
    private bool pause;
    [Tooltip("Hide mouse cursor")]
    public bool hideCursor;

    [Header("UI Settings")]
    public Animation fadeScreen;
    public Animation bloodScreen;
    public Animation lifesCountScreen;
    public string bloodPulsingAnimName;
    public string bloodFadeAnimName;
    public string fadeOutAnimName;
    public string fadeInAnimName;
    public string fadeDieAnimName;
    public string fadeHideDieAnimName;
    public GameObject gameControllPanel;
    public GameObject mobileControllPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Image dropImage;
    public Image standImage;
    public Image crouchImage;
    public Image hidePlaceExitImage;
    public Image interactImage;
    [SerializeField] AdvertizementScript advertisementsk;

    private void Start()
    {
        if(hideCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Time.timeScale = 1.0f;
        player.locked = true;
        enemy.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        fadeScreen.Play("Fade");
        lifesCountScreen.gameObject.SetActive(true);
        lifesCountScreen.Play("LifesCount");
        lifesCountScreen.transform.GetChild(0).GetComponent<Text>().text = lifeCount.ToString();
        StartCoroutine(WaitRestart());


        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            player.mouseSensetivity = 50f;
            sensitivitySlider.value = 50f;
        }

        
    }

    private void Update()
    {
        AmbientChange();
        ControllGame();
    }

    private void ControllGame()
    {
        if(CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!pause)
        {      
            pause = true;
            pausePanel.SetActive(pause);
            Time.timeScale = 0.0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pause = false;
            pausePanel.SetActive(pause);
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt("HasSaveGame", 0);
        gameOverPanel.SetActive(true);
        advertisementsk.ShowAd();
    }

    public void GameWin()
    {
        gameControllPanel.SetActive(false);
        gameWinPanel.SetActive(true);
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
        PlayerPrefs.SetInt("HasSaveGame",0);
        advertisementsk.ShowAd();
    }

    public void MainMenuExit(int saveGame)
    {
        if(saveGame == 0)
        {
            PlayerPrefs.SetInt("HasSaveGame", 0);
            PlayerPrefs.SetString("SceneName", "MainMenu");
            SceneManager.LoadScene("LoadScene");
        }

        if (saveGame == 1)
        {
            PlayerPrefs.SetInt("HasSaveGame", 1);
            PlayerPrefs.SetString("SceneName", "MainMenu");
            SceneManager.LoadScene("LoadScene");
        }
    }

    private void AmbientChange()
    {
        if (enemy.seePlayer)
        {
            if (dangerAmbient.volume < 1)
            {
                dangerAmbient.volume += Time.deltaTime / 2f;
            }

        }
        else
        {
            if (dangerAmbient.volume > 0)
            {
                dangerAmbient.volume -= Time.deltaTime / 8f;
            }
        }
   
    }

    public void Respawn()
    {
        enemy.gameObject.SetActive(true);
        player.clampXaxis.x = 0;
        player.clampXaxis.y = 0;
        player.clampXaxis.x = -90;
        player.clampXaxis.y = 90;
        player.clampByY = false;
        player.hidePlace = null;
        player.transform.position = playerSpawnPoint.position;
        player.transform.rotation = playerSpawnPoint.rotation;
        enemy.transform.position = enemySpawnPoint.position;
        enemy.transform.rotation = enemySpawnPoint.rotation;
        player.locked = false;
        player.lockedMovement = false;
        player.cameraTransform.localRotation = new Quaternion(0,0,0,0);
        player.cameraAnimation.Play(player.cameraIdleAnimName);
        enemy.RestartEnemyStats();
        ScreenFade(0);

    }

    public void ScreenFade(int state)
    {
        if(state == 0)
        {
            fadeScreen.Play(fadeOutAnimName);
        }

        if(state == 1)
        {
            fadeScreen.Play(fadeInAnimName);
        }

        if(state == 2)
        {
           
            StartCoroutine(WaitKillAnim(3f));
        }

        if (state == 3)
        {

            StartCoroutine(WaitKillAnim(4f));
        }
    }

    public void ScreenBlood(int state)
    {
        if(state == 0)
        {
            bloodScreen.Play(bloodFadeAnimName);
        }

        if(state == 1)
        {
            bloodScreen.Play(bloodPulsingAnimName);
        }
    }

    private IEnumerator WaitKillAnim(float killTime)
    {
        yield return new WaitForSeconds(killTime);
        fadeScreen.Play(fadeInAnimName);
        StartCoroutine(WaitFadeAnim(fadeInAnimName));
    }

    private IEnumerator WaitFadeAnim(string name)
    {
        yield return new WaitForSeconds(fadeScreen[name].length);
        if (lifeCount > 1)
        {
            lifeCount -= 1;
            lifesCountScreen.gameObject.SetActive(true);
            lifesCountScreen.Play("LifesCount");
            lifesCountScreen.transform.GetChild(0).GetComponent<Text>().text = lifeCount.ToString();
            StartCoroutine(WaitRestart());
            advertisementsk.ShowAd();
        }
        else
        {
            GameOver();
        }

    }

    private IEnumerator WaitRestart()
    {
        yield return new WaitForSeconds(3f);
        lifesCountScreen.gameObject.SetActive(false);
        Respawn();
    }

    public void ConfigureApply()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");

    }
}

