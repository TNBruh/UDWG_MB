using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static internal GameManager singleton = null;
    static internal Scene currentScene;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip startClip, loopClip;
    [SerializeField]
    GameObject stageManagerPrefab;
    StageManager stageManager;

    //main menu buttons
    GameObject startButtonObj;
    GameObject creditsButtonObj;
    GameObject exitButtonObj;
    Button startButton;
    Button creditsButton;
    Button exitButton;


    private void Start()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            currentScene = SceneManager.GetActiveScene();

            StartCoroutine(ProcessScene(currentScene));

            StartCoroutine(PlaySong());

            
        } else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ProcessScene(Scene scene, Coroutine prevCoroutine = null)
    {
        yield return prevCoroutine;
        yield return new WaitUntil(() => scene.isLoaded);

        int sceneInd = scene.buildIndex;

        switch (sceneInd)
        {
            case 1: //main menu
                startButtonObj = GameObject.FindGameObjectWithTag("StartButton");
                creditsButtonObj = GameObject.FindGameObjectWithTag("CreditsButton");
                exitButtonObj = GameObject.FindGameObjectWithTag("ExitButton");
                startButton = startButtonObj.GetComponent(typeof(Button)) as Button;
                creditsButton = creditsButtonObj.GetComponent(typeof(Button)) as Button;
                exitButton = exitButtonObj.GetComponent(typeof(Button)) as Button;

                startButton.onClick.AddListener(EnterStage);
                creditsButton.onClick.AddListener(EnterCredits);
                exitButton.onClick.AddListener(Sussy);

                break;
            case 2: //game
                stageManager = Instantiate(stageManagerPrefab).GetComponent(typeof(StageManager)) as StageManager;
                yield return new WaitForFixedUpdate();
                stageManager.Begin();
                break;
            case 3: //credits
                exitButtonObj = GameObject.FindGameObjectWithTag("ExitButton");
                exitButton = exitButtonObj.GetComponent(typeof(Button)) as Button;

                exitButton.onClick.AddListener(EnterMain);
                break;
        }
    }

    IEnumerator CleanScene(Scene scene)
    {
        yield return new WaitUntil(() => scene.isLoaded);

        int sceneInd = scene.buildIndex;

        switch (sceneInd)
        {
            case 1: //main menu
                break;
            case 2: //game
                //clean up
                //order the stage manager to clean
                yield return StartCoroutine(stageManager.CleanUp());
                Debug.Log("cleaned stage");
                break;
            case 3: //credits
                break;
            case 4:
                break;
        }
    }

    IEnumerator ChangeScene(int sceneInd)
    {
        Scene previousScene = SceneManager.GetActiveScene();
        Coroutine cleanUp = StartCoroutine(CleanScene(previousScene));

        yield return cleanUp;

        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(sceneInd);
        yield return new WaitUntil(() => asyncLoadScene.isDone);
        Scene currentScene = SceneManager.GetActiveScene();

        yield return new WaitUntil(() => currentScene.isLoaded);
        StartCoroutine(ProcessScene(currentScene, cleanUp));
        Time.timeScale = 1;

    }

    public void EnterStage()
    {
        ChangeSceneAPI(2);
    }

    public void EnterCredits()
    {
        ChangeSceneAPI(3);
    }

    public void EnterMain()
    {
        ChangeSceneAPI(1);
    }

    public void Sussy()
    {
        Application.Quit();
    }

    internal Coroutine ChangeSceneAPI(int sceneInd)
    {
        return StartCoroutine(ChangeScene(sceneInd));
    }

    IEnumerator PlaySong()
    {
        audioSource.clip = startClip;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.clip = loopClip;
        audioSource.Play();
        audioSource.loop = true;

    }
}
