using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("References")]
    public musiclibrary musicLibrary;
    public AudioSource musicSource;
    public AudioMixer mixer;

    [Header("Settings")]
    public float fadeDuration = 1f;

    private string currentScene;
    private int currentTrackIndex = 0;
    private List<string> currentPlaylist = new List<string>();
    private Coroutine playlistCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate MusicManager destroyed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("MusicManager created in scene: " + SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0f);
        mixer.SetFloat("MusicVolume", savedVolume);

        PlayMusicFromScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicFromScene(scene.name);
    }

    private void PlayMusicFromScene(string sceneName)
    {
        Debug.Log("Scene Loaded: " + sceneName);
        currentScene = sceneName;

        // Tentukan playlist sesuai scene
        currentPlaylist.Clear();

        switch (sceneName)
        {
            case "Play":
                currentPlaylist.Add("play");
                currentPlaylist.Add("play1");
                break;
            case "IntroScene":
                currentPlaylist.Add("intro1");
                currentPlaylist.Add("intro2");
                currentPlaylist.Add("intro3");
                break;
            case "SampleScene":
            default:
                Debug.Log("No playlist for scene: " + sceneName);
                return;
        }

        currentTrackIndex = 0;

        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);
        playlistCoroutine = StartCoroutine(PlayPlaylistLoop());
    }

    private IEnumerator PlayPlaylistLoop()
    {
        while (true)
        {
            if (currentPlaylist.Count == 0) yield break;

            string trackName = currentPlaylist[currentTrackIndex];
            AudioClip clip = musicLibrary.GetClipFromName(trackName);

            if (clip == null)
            {
                Debug.LogWarning("Track not found: " + trackName);
                yield break;
            }

            yield return StartCoroutine(FadeInNewMusic(clip));

            // Tunggu sampai lagu selesai
            yield return new WaitWhile(() => musicSource.isPlaying);

            // Pindah ke track berikutnya
            currentTrackIndex = (currentTrackIndex + 1) % currentPlaylist.Count;
        }
    }

    public void SetVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    IEnumerator FadeInNewMusic(AudioClip clip)
    {
        Debug.Log("Fading to new music: " + clip.name);

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
    }
}

