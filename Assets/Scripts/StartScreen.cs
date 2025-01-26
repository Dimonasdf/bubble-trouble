using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private GameplayCutscene gameManager;
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject uiScraper;

    [SerializeField] private float canvasHideTime = 0.5f;
    [SerializeField] private float canvasHideSpeed = 20f;

    public void StartGame()
    {
        startGameButton.interactable = false;
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        var timeLeft = canvasHideTime;

        while (timeLeft > 0)
        {
            var deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;
            gameObject.transform.position -= Vector3.up * canvasHideSpeed * deltaTime;

            yield return null;
        }

        gameManager.StartGame();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {

    //    }
    //}

    public void Quit()
    {
        Application.Quit();
    }
}
