using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text Death;
    private int _death = 0;

    private void Awake()
    {
        Debug.Assert(Instance == null, "Singleton");
        Instance = this;
        Application.targetFrameRate = 60;
    }

    public void NotifyDead()
    {
        AddDeath();
        StartCoroutine(ReloadScene(_death));
    }
    public void AddDeath()
    {
        Death.text = $"Morts: {++_death}";
    }

    private IEnumerator ReloadScene(int death)
    {
        const string sceneName = "MainScene";
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }
}