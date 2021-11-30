using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class uiControler : MonoBehaviour
{
    public GameObject menuGO;
    public GameObject menuPausa;
    public GameObject menuGanar;

    private void Start()
    {
        menuGO = GameObject.Find("MenuGameOver");
        menuPausa = GameObject.Find("MenuPausa");
        menuGanar = GameObject.Find("MenuGanar");
        menuPausa.SetActive(false);
        menuGanar.SetActive(false);
        menuGO.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(0);
    }

    public void VolverAJugar()
    {
        menuPausa.SetActive(false);
    }

    public void GameOver()
    {
        menuGO.SetActive(true);
    }

    public void Ganar()
    {
        menuGanar.SetActive(true);
    }

    public void CapturarTeclaPausa(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (menuPausa.activeInHierarchy)
            {
                menuPausa.SetActive(false);
            }
            else
            {
                menuPausa.SetActive(true);
            }
        }
    }
}
