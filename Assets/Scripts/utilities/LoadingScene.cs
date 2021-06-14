using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AsyncLoadScene());
    }

    IEnumerator AsyncLoadScene()
    {
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("GameScene");

        while (loadGame.progress<1)
        {
            progressBar.fillAmount = loadGame.progress;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }
}
