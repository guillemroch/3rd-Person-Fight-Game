using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingScreen : MonoBehaviour{
    [SerializeField] private VideoPlayer _videoPlayer;


    public void LoadLevel(int sceneIndex) {
        gameObject.SetActive(true);
        _videoPlayer.Play();

        StartCoroutine(LoadLevelAsync(sceneIndex));

    }

    IEnumerator LoadLevelAsync(int sceneIndex) {
         AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

         while (!loadOperation.isDone) {
             yield return null;
         }
    }
}
