using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Menu : MonoBehaviour
{
    bool start = false;
    bool pause = false;

    bool strobing = false;

    float buttonStrobDelay = 0.5f;

    GameObject startMenu;
    GameObject startButton;

    GameObject pauseMenu;

    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        start = gameManager.start;

        

        startMenu = GameObject.Find("StartMenu");
        startButton = GameObject.Find("StartButton");

        pauseMenu = GameObject.Find("PauseMenu");

        if (!start) {
            startMenu.SetActive(false);
            pauseMenu.SetActive(false);

            return;
        }

        startMenu.SetActive(true);
        pauseMenu.SetActive(false);

    }

    void StrobButton() {
        strobing = true;

        if (!start) {
            startButton.SetActive(false);
            return;
        }

        if (startButton.activeSelf) {
            startButton.SetActive(false);
        } else {
            startButton.SetActive(true);
        }

        Invoke("StrobButton", buttonStrobDelay);
    }

    // Update is called once per frame
    void Update()
    {
       if (start) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                startMenu.SetActive(false);

                gameManager.InitGame();

                start = false;
                gameManager.start = start;
            }
            if (!strobing) {
                StrobButton();
            }
        }
       if (pause) {
            pauseMenu.SetActive(true);
        } 
    }
}
