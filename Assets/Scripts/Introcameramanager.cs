using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroCameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera startMenuCamera;      // Caméra fixe pour le menu START
    public Camera overheadCamera;       // Caméra qui suit le shuttle
    public Camera mainCamera;           // Caméra VR gameplay
    public GameObject xrOrigin;
    
    [Header("UI")]
    public GameObject startButton;
    public KeyCode startKey = KeyCode.Space;
    
    [Header("Shuttle")]
    public ShuttleIntroMovement shuttleScript;
    
    [Header("Positions")]
    public Transform playerFinalPosition;  // Position finale sur la plateforme
    
    private CanvasGroup fadePanel;
    private bool hasStarted = false;

    void Start()
    {
        // Au début: StartMenuCamera active
        if (startMenuCamera != null) startMenuCamera.enabled = true;
        if (overheadCamera != null) overheadCamera.enabled = false;
        if (mainCamera != null) mainCamera.enabled = false;

        // XR Origin peut être désactivé pendant le menu
        if (xrOrigin != null) xrOrigin.SetActive(false);

        if (startButton != null) startButton.SetActive(true);

        // CRÉE le fade panel
        CreateFadePanel();

        // Subscribe au shuttle event
        if (shuttleScript != null)
        {
            shuttleScript.onRotationComplete.AddListener(OnShuttleComplete);
            Debug.Log("✅ Subscribed to shuttle complete event");
        }

        Debug.Log("🎮 START MENU - Press SPACE to begin!");
    }

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(startKey))
        {
            OnStartPressed();
        }
    }

    public void OnStartPressed()
    {
        if (hasStarted) return;
        hasStarted = true;
        
        Debug.Log("🚀 START pressed - Beginning transportation!");
        StartCoroutine(TransportationSequence());
    }

    IEnumerator TransportationSequence()
    {
        // Cache le bouton
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
        
        // Fade to black depuis le menu
        yield return StartCoroutine(FadeToBlack());
        
        // Switch vers overhead camera
        if (startMenuCamera != null) startMenuCamera.enabled = false;
        if (overheadCamera != null) overheadCamera.enabled = true;
        
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());
        
        // Démarre le shuttle (représente le "voyage")
        if (shuttleScript != null)
        {
            shuttleScript.StartIntroSequence();
        }
        
        Debug.Log("📹 Transportation in progress - Overhead view!");
    }

    void OnShuttleComplete()
    {
        Debug.Log("🎬 Transportation complete - Arriving at platform!");
        StartCoroutine(ArriveAtPlatform());
    }

    IEnumerator ArriveAtPlatform()
    {
        yield return new WaitForSeconds(1f);
        
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        
        // Active et positionne le XR Origin
        if (xrOrigin != null)
        {
            xrOrigin.SetActive(true);
            
            if (playerFinalPosition != null)
            {
                xrOrigin.transform.position = playerFinalPosition.position;
                xrOrigin.transform.rotation = playerFinalPosition.rotation;
            }
        }
        
        // Switch vers Main Camera (VR)
        if (overheadCamera != null) overheadCamera.enabled = false;
        if (mainCamera != null) mainCamera.enabled = true;
        
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());
        
        Debug.Log("✅ Arrived on platform - Gameplay starts!");
    }

    void CreateFadePanel()
    {
        // Crée un Canvas en Screen Space Overlay
        GameObject canvas = new GameObject("FadeCanvas");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 1000;

        // Ajoute le CanvasScaler pour que ça scale correctement
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Crée le panel noir
        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(canvas.transform, false); // FALSE est important!

        // Ajoute l'Image
        Image img = panel.AddComponent<Image>();
        img.color = Color.black;

        // Configure le RectTransform pour remplir TOUT l'écran
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);  // Coin bas-gauche
        rect.anchorMax = new Vector2(1, 1);  // Coin haut-droit
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;  // Important!

        // Ajoute le CanvasGroup
        fadePanel = panel.AddComponent<CanvasGroup>();
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        DontDestroyOnLoad(canvas);

        Debug.Log("✅ Fade panel created - covers full screen");
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        float duration = 1f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = t / duration;
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    IEnumerator FadeFromBlack()
    {
        float t = 0f;
        float duration = 1f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = 1f - (t / duration);
            yield return null;
        }
        fadePanel.alpha = 0f;
    }
}