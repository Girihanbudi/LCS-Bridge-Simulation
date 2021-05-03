using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Slider loadingSlider;
    public Text loadingText;

    private void Start()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentSceneIndex++;
        StartCoroutine(LoadAsynchoronously(currentSceneIndex));
    }

    IEnumerator LoadAsynchoronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            loadingText.text = (progress * 100).ToString("F2", CultureInfo.InvariantCulture) + "%";

            yield return null;
        }

    }
}
