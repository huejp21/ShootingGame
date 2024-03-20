using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public Text enemyCountUI;
    public Text playTimeUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
	}

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    // Update is called once per frame
    void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        string totalEnemyCount = spawner.currentWave.infinite ? "Infinite" : spawner.currentWave.enemyCount.ToString();
        enemyCountUI.text = "Enemy: " + totalEnemyCount + "/" + spawner.enemyKilledCount.ToString();
        System.TimeSpan playTime = System.DateTime.Now - spawner.gameStartTime;
        playTimeUI.text = "Play Time:" 
            + playTime.Hours.ToString("00") + ":"
            + playTime.Minutes.ToString("00") + ":"
            + playTime.Seconds.ToString("00") + ":"
            + playTime.Milliseconds.ToString("000");


        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] OverTenNumbers = { };
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

        newWaveTitle.text = "- Wave" + numbers[waveNumber - 1] + " - ";
        string enemyCountString = spawner.waves[waveNumber - 1].infinite ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount.ToString();
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver()
    {
        //StartCoroutine(Fade(Color.clear, Color.black, 1.0f));
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0, 0.95f), 1.0f));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        enemyCountUI.gameObject.SetActive(false);
        playTimeUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1.0f / speed + delayTime;


        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;
            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-227, 187, animatePercent);
            yield return null;

        }
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1.0f / time;
        float percent = 0;

        while (percent < 1.0f)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        //Application.LoadLevel("Game");
        ScoreKeeper.ResetScore();
        SceneManager.LoadScene("Game");
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
