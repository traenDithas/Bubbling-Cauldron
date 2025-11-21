using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Drag a Panel or Text object here that says 'PAUSED' (optional).")]
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    void Update()
    {
        // Use ESCAPE key to toggle pause on PC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // FREEZE TIME
            if(pauseMenuUI != null) pauseMenuUI.SetActive(true);
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1f; // RESUME TIME
            if(pauseMenuUI != null) pauseMenuUI.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }
}