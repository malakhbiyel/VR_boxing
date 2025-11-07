using UnityEngine;
using UnityEngine.XR;

public class HandCollisionDetector : MonoBehaviour
{
    [Header("Hand Settings")]
    [Tooltip("Est-ce que c'est la main gauche?")]
    public bool isLeftHand = false;
    
    [Header("Haptic Feedback")]
    [Tooltip("Activer les vibrations")]
    public bool enableHaptics = true;
    
    [Tooltip("Intensité de la vibration pour les cibles (0-1)")]
    [Range(0f, 1f)]
    public float targetHapticIntensity = 0.5f;
    
    [Tooltip("Durée de la vibration pour les cibles (secondes)")]
    public float targetHapticDuration = 0.1f;
    
    [Tooltip("Intensité de la vibration pour les obstacles (0-1)")]
    [Range(0f, 1f)]
    public float obstacleHapticIntensity = 0.8f;
    
    [Tooltip("Durée de la vibration pour les obstacles (secondes)")]
    public float obstacleHapticDuration = 0.2f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private PunchDetector punchDetector;
    private GameManager gameManager;
    private InputDevice targetDevice;
    
    void Start()
    {
        // Récupérer les composants nécessaires
        punchDetector = GetComponent<PunchDetector>();
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Initialiser le device XR pour les vibrations
        InitializeXRDevice();
        
        if (punchDetector == null)
        {
            Debug.LogWarning($"⚠️ PunchDetector manquant sur {gameObject.name}");
        }
        
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager introuvable dans la scène!");
        }
        
        // Vérifier que le tag est bien "Hand"
        if (!gameObject.CompareTag("Hand"))
        {
            Debug.LogWarning($"⚠️ {gameObject.name} n'a pas le tag 'Hand'!");
        }
        
        string handSide = isLeftHand ? "GAUCHE" : "DROITE";
        Debug.Log($"✅ Main {handSide} configurée - Ready to detect collisions!");
    }
    
    void InitializeXRDevice()
    {
        // Récupérer le bon contrôleur (gauche ou droit)
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;
        
        if (isLeftHand)
        {
            characteristics |= InputDeviceCharacteristics.Left;
        }
        else
        {
            characteristics |= InputDeviceCharacteristics.Right;
        }
        
        var devices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
        
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            string handSide = isLeftHand ? "GAUCHE" : "DROITE";
            Debug.Log($"🎮 Contrôleur {handSide} trouvé: {targetDevice.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Contrôleur {(isLeftHand ? "gauche" : "droit")} non trouvé");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est une cible
        if (other.CompareTag("Target"))
        {
            HandleTargetHit(other);
        }
        // Vérifier si c'est un obstacle
        else if (other.CompareTag("Obstacle"))
        {
            HandleObstacleHit(other);
        }
    }
    
    void HandleTargetHit(Collider targetCollider)
    {
        // Récupérer le script Projectile du parent
        Projectile projectile = targetCollider.GetComponentInParent<Projectile>();
        
        if (projectile == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ Target sans script Projectile!");
            return;
        }
        
        // Vérifier la vitesse du coup
        float punchSpeed = punchDetector != null ? punchDetector.speed : 0f;
        float minSpeed = punchDetector != null ? punchDetector.punchThreshold : 1.5f;
        
        string handSide = isLeftHand ? "GAUCHE" : "DROITE";
        
        // ===== LOG DÉTAILLÉ =====
        Debug.Log("╔════════════════════════════════════╗");
        Debug.Log($"║ CIBLE TOUCHÉE - Main {handSide}");
        Debug.Log($"║ Vitesse du coup: {punchSpeed:F2} m/s");
        Debug.Log($"║ Vitesse minimum: {minSpeed:F2} m/s");
        Debug.Log($"║ Valide? {(punchSpeed >= minSpeed ? "✅ OUI" : "❌ NON")}");
        Debug.Log("╚════════════════════════════════════╝");
        
        if (punchSpeed >= minSpeed)
        {
            // Coup valide!
            int points = CalculatePoints(punchSpeed);
            
            if (gameManager != null)
            {
                gameManager.AddScore(points);
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"🎯 TARGET HIT! Main {handSide} | Vitesse: {punchSpeed:F2} m/s | Points: +{points}");
            }
            
            // ===== TEST VIBRATION =====
            Debug.Log("████████████████████████████████████");
            Debug.Log($"📳📳📳 APPEL DE VIBRATION! Main {handSide}");
            Debug.Log("████████████████████████████████████");
            
            // Effet haptique (vibration)
            TriggerHapticFeedback(targetHapticIntensity, targetHapticDuration);
            
            Debug.Log("████████████████████████████████████");
            Debug.Log("📳 VIBRATION ENVOYÉE (normalement)");
            Debug.Log("████████████████████████████████████");
            
            // Effet visuel/sonore ici si nécessaire
            // TODO: Ajouter particules, son
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"⚡ Coup trop faible | Main {handSide} | Vitesse: {punchSpeed:F2} m/s (min: {minSpeed:F2})");
            }
        }
        
        // Détruire la cible
        Destroy(projectile.gameObject);
    }
    
    void HandleObstacleHit(Collider obstacleCollider)
    {
        // Récupérer le script Projectile du parent
        Projectile projectile = obstacleCollider.GetComponentInParent<Projectile>();
        
        if (projectile == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ Obstacle sans script Projectile!");
            return;
        }
        
        string handSide = isLeftHand ? "GAUCHE" : "DROITE";
        
        // Pénalité pour avoir touché un obstacle
        // Le joueur aurait dû se baisser pour l'éviter!
        int penalty = -15;
        
        if (gameManager != null)
        {
            gameManager.AddScore(penalty);
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"❌ OBSTACLE TOUCHÉ! Main {handSide} | Tu aurais dû te baisser! | Pénalité: {penalty} points");
        }
        
        // Effet haptique plus fort pour les obstacles
        TriggerHapticFeedback(obstacleHapticIntensity, obstacleHapticDuration);
        
        // Effet visuel/sonore négatif ici
        // TODO: Ajouter effet rouge, son d'erreur
        
        // Détruire l'obstacle
        Destroy(projectile.gameObject);
    }
    
    int CalculatePoints(float speed)
    {
        // Système de points basé sur la vitesse
        // Plus le coup est rapide, plus on gagne de points
        
        if (speed >= 5f)
            return 20;  // Coup très puissant
        else if (speed >= 3.5f)
            return 15;  // Coup puissant
        else if (speed >= 2.5f)
            return 10;  // Coup normal
        else
            return 5;   // Coup faible mais valide
    }
    
    /// <summary>
    /// Déclenche une vibration haptique sur le contrôleur
    /// </summary>
    void TriggerHapticFeedback(float intensity, float duration)
    {
        if (!enableHaptics) return;
        
        string handSide = isLeftHand ? "GAUCHE" : "DROITE";
        bool vibrationSent = false;
        
        // Méthode 1: InputDevice (OpenXR standard)
        if (targetDevice.isValid)
        {
            bool success = targetDevice.SendHapticImpulse(0, intensity, duration);
            if (success)
            {
                vibrationSent = true;
                Debug.Log($"📳 [OpenXR] Vibration {handSide}: {intensity:F2} / {duration:F2}s");
            }
            else
            {
                Debug.LogWarning($"⚠️ Échec SendHapticImpulse pour {handSide}");
            }
        }
        
        // Méthode 2: Oculus/Meta native API (fallback)
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!vibrationSent)
        {
            try
            {
                OculusHapticFeedback oculusHaptic = GetComponent<OculusHapticFeedback>();
                if (oculusHaptic != null)
                {
                    oculusHaptic.TriggerHaptic(intensity, duration);
                    vibrationSent = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Oculus haptic fallback failed: {e.Message}");
            }
        }
        #endif
        
        // Méthode 3: XR Interaction Toolkit (autre fallback)
        if (!vibrationSent)
        {
            var xriController = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();
            if (xriController != null)
            {
                xriController.SendHapticImpulse(intensity, duration);
                vibrationSent = true;
                Debug.Log($"📳 [XRI] Vibration {handSide}: {intensity:F2} / {duration:F2}s");
            }
        }
        
        if (!vibrationSent)
        {
            Debug.LogError($"❌ Aucune méthode de vibration n'a fonctionné pour {handSide}!");
            // Réessayer de trouver le device
            InitializeXRDevice();
        }
    }
    
    // Gizmo pour visualiser le collider en mode Scene
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = isLeftHand ? Color.cyan : Color.magenta;
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
        }
    }
}