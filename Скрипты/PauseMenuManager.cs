using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("External Panels")]
    public GameObject shopPanel;  // Объект магазина (панель с ShopUIManager)

    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button saveButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings Panel Controls")]
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeLabel;
    public Slider mouseSensitivitySlider;
    public TextMeshProUGUI mouseSensitivityLabel;
    public Slider cameraFOVSlider;
    public TextMeshProUGUI cameraFOVLabel;

    [Header("Graphics Settings")]
    public Toggle shadowsToggle;
    public Dropdown qualityDropdown;

    [Header("Performance Settings")]
    public Toggle fpsToggle;
    public GameObject fpsDisplay;

    private bool isPaused = false;
    private PlayerController playerController;

    // Возвращает true, если активна панель паузы или настройки
    public bool IsPauseActive => pausePanel.activeSelf || settingsPanel.activeSelf;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        // Поиск экземпляра PlayerController
        playerController = Object.FindFirstObjectByType<PlayerController>();

        // Инициализация UI элементов и подписка на события
        musicVolumeSlider.value = AudioListener.volume;
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        musicVolumeLabel.text = (AudioListener.volume * 100).ToString("F0") + "%";

        mouseSensitivitySlider.value = playerController.mouseSensitivity;
        mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        mouseSensitivityLabel.text = playerController.mouseSensitivity.ToString("F1");

        if (Camera.main != null)
        {
            cameraFOVSlider.value = Camera.main.fieldOfView;
            cameraFOVSlider.onValueChanged.AddListener(UpdateCameraFOV);
            cameraFOVLabel.text = Camera.main.fieldOfView.ToString("F0");
        }

        shadowsToggle.isOn = QualitySettings.shadows != ShadowQuality.Disable;
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        if (fpsToggle != null)
        {
            fpsToggle.isOn = (fpsDisplay != null && fpsDisplay.activeSelf);
            fpsToggle.onValueChanged.AddListener(UpdateFPSToggle);
        }

        resumeButton.onClick.AddListener(ResumeGame);
        saveButton.onClick.AddListener(SaveGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        // Обработка нажатия клавиши Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool shopOpen = (shopPanel != null && shopPanel.activeSelf);
            bool inventoryOpen = InventoryUI.IsOpen; // Используем новое состояние инвентаря
            bool pauseOpen = pausePanel.activeSelf;
            bool settingsOpen = settingsPanel.activeSelf;

            if (shopOpen || inventoryOpen || pauseOpen || settingsOpen)
            {
                // Если магазин открыт – закрываем его
                if (shopOpen)
                    ShopUIManager.Instance.CloseShop();

                // Если инвентарь открыт – закрываем его
                if (inventoryOpen)
                    InventoryUI.Instance.CloseInventory();

                // Закрываем панель настроек, если открыта
                if (settingsOpen)
                    settingsPanel.SetActive(false);

                // Если панель паузы открыта – снимаем паузу
                if (pauseOpen)
                    ResumeGame();

                // Если ни магазин, ни инвентарь, ни настройки не открыты, делаем курсор неактивным
                if (!shopOpen && !inventoryOpen && !settingsOpen)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else
            {
                // Если ни одна из панелей не открыта, переключаем паузу
                TogglePause();
            }
        }
    }

    void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UISoundManager.Instance?.PlayOpenCloseSound();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        // Если ни магазин, ни инвентарь не открыты, делаем курсор неактивным
        if ((shopPanel == null || !shopPanel.activeSelf) && !InventoryUI.IsOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        UISoundManager.Instance?.PlayOpenCloseSound();
    }

    void SaveGame()
    {
        Debug.Log("Сохранить игру (заглушка)");
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    void QuitGame()
    {
        Debug.Log("Выйти из игры");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Обработчики событий UI

    void UpdateMusicVolume(float volume)
    {
        AudioListener.volume = volume;
        musicVolumeLabel.text = (volume * 100).ToString("F0") + "%";
    }

    void UpdateMouseSensitivity(float sensitivity)
    {
        if (playerController != null)
        {
            playerController.mouseSensitivity = sensitivity;
            mouseSensitivityLabel.text = sensitivity.ToString("F1");
        }
    }

    void UpdateCameraFOV(float fov)
    {
        if (Camera.main != null)
        {
            Camera.main.fieldOfView = fov;
            cameraFOVLabel.text = fov.ToString("F0");
        }
    }

    void UpdateFPSToggle(bool isOn)
    {
        if (fpsDisplay != null)
        {
            fpsDisplay.SetActive(isOn);
        }
    }
}
