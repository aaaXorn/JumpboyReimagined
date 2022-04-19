using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] string next_scene;

    [SerializeField] Image image;
    [SerializeField] Text text;

    void Start()
    {
        //começa o load
        StartCoroutine(LoadAsync(next_scene));
    }

    IEnumerator LoadAsync(string sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(next_scene);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            image.fillAmount = progress;
            text.text = progress * 100f + "%";

            yield return null;
        }
    }
}
