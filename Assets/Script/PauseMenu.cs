using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] CameraSystemm cameraSystem;

    InputAction pauseAction;
    bool isPaused;

    void Awake()
    {
        pauseAction = new InputAction(
            type: InputActionType.Button,
            binding: "<Keyboard>/escape"
        );

        pauseAction.performed += _ => TogglePause();
        pauseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.Disable();
    }

    void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        if (cameraSystem != null)
            cameraSystem.enabled = false;

        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        if (cameraSystem != null)
            cameraSystem.enabled = true;

        isPaused = false;
    }

    // RESTART GAME
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // EXIT GAME
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game button clicked");
    }
}