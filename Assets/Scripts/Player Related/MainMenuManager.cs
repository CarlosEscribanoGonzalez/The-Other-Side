using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterVolume;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Dropdown micDropdown;
    private PlayerMovement player;
    private SideChanger sideChanger;
    private IntensityMultiplier[] mults;
    private bool paused = false;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        sideChanger = GameObject.FindObjectOfType<SideChanger>();
        sideChanger.sideChanged += ToggleBackgroundColor;
        mults = GameObject.FindObjectsOfType<IntensityMultiplier>();
        cursor.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        UpdateAvailableMics();
    }

    private void Update()
    {
        DeselectOnRelease();
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (player.GetCanMove()) //Si se puede mover se pausa
        {
            Cursor.lockState = CursorLockMode.None;
            cursor.SetActive(false);
            pauseMenu.SetActive(true);
            background.gameObject.SetActive(true);
            player.SetCanMove(false);
            paused = true;
        }
        else if(paused)
        {
            Resume();
        }
    }

    private void DeselectOnRelease() //Deselecciona el último elemento de la UI seleccionado para que se le pueda volver a hacer Hover
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (EventSystem.current.currentSelectedGameObject != null) EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void StartGame()
    {
        sideChanger.PerformChange();
        Cursor.lockState = CursorLockMode.Locked;
        cursor.SetActive(true);
        background.color = new Color(0, 0, 0, 0.8f);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SoundSliderChanged(float value)
    {
        masterVolume.SetFloat("Volume", value);
    }

    public void LightSliderChanged(float value)
    {
        foreach(IntensityMultiplier m in mults)
        {
            if(m != null) m.AdjustLight(value, true);
        }
    }

    public void Resume()
    {
        cursor.SetActive(true);
        player.SetCanMove(true);
        pauseMenu.SetActive(false);
        settings.SetActive(false);
        background.gameObject.SetActive(false);
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitSettings()
    {
        if (paused) pauseMenu.SetActive(true);
        else mainMenu.SetActive(true);
    }

    public void OnDeath()
    {
        pauseMenu.SetActive(false);
        settings.SetActive(false);
        background.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ToggleBackgroundColor(object s, EventArgs e)
    {
        if (player.transform.position.y < 0) background.color = new Color(1, 1, 1, 0.2f);
        else background.color = new Color(0, 0, 0, 0.8f);
    }

    public void UpdateAvailableMics() //Llamado cada vez que se le da a settings para recargar los micrófonos por si se ha añadido uno
    {
        string currentMic = GameObject.FindObjectOfType<MicrophoneManager>().GetMicrophone();
        int prevValue = 0;
        micDropdown.ClearOptions();
        String[] devices = Microphone.devices;
        for(int i = 0; i < devices.Length; i++)
        {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData(devices[i]);
            if (devices[i] == currentMic) prevValue = i;
            micDropdown.options.Add(newData);
        }
        micDropdown.value = prevValue;
        micDropdown.RefreshShownValue();
        SetMicrophone();
    }

    public void SetMicrophone()
    {
        GameObject.FindObjectOfType<MicrophoneManager>().SetMicrophone(micDropdown.options[micDropdown.value].text);
    }
}
