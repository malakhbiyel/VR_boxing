using UnityEngine;

public class HandCollisionDetector : MonoBehaviour
{
    [Header("Hand Settings")]
    [Tooltip("Est-ce que c'est la main gauche?")]
    public bool isLeftHand = false;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private PunchDetector punchDetector;
    private GameManager gameManager;
    
    void Start()
    {
        // Récupérer les composants nécessaires
        punchDetector = GetComponent<PunchDetector>();
        gameManager = FindFirstObjectByType<GameManager>();
        
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
            
            // Effet visuel/sonore ici si nécessaire
            // TODO: Ajouter particules, son, vibration
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
        
        // Effet visuel/sonore négatif ici
        // TODO: Ajouter effet rouge, son d'erreur, vibration forte
        
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