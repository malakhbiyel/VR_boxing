using UnityEngine;

/// <summary>
/// Détecte quand un obstacle touche la tête/corps du joueur
/// Le joueur aurait dû se baisser!
/// </summary>
public class BodyCollisionDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Pénalité quand un obstacle touche le corps")]
    public int obstaclePenalty = -20;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager introuvable dans la scène!");
        }
        
        // Vérifier que le tag est bien "Player" ou "Body"
        if (!gameObject.CompareTag("Player") && !gameObject.CompareTag("Body"))
        {
            Debug.LogWarning($"⚠️ {gameObject.name} devrait avoir le tag 'Player' ou 'Body'!");
        }
        
        Debug.Log($"✅ Body Collision Detector activé sur {gameObject.name}");
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est un obstacle
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
    }
    
    void HandleObstacleCollision(Collider obstacleCollider)
    {
        // Récupérer le script Projectile
        Projectile projectile = obstacleCollider.GetComponentInParent<Projectile>();
        
        if (projectile == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ Obstacle sans script Projectile!");
            return;
        }
        
        // Le joueur n'a pas réussi à éviter l'obstacle!
        if (gameManager != null)
        {
            gameManager.AddScore(obstaclePenalty);
            gameManager.obstaclesHit++;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"💥 COLLISION OBSTACLE! Tu aurais dû te BAISSER! | Pénalité: {obstaclePenalty} points");
        }
        
        // Effet visuel/sonore fort
        // TODO: Ajouter:
        // - Flash rouge sur l'écran
        // - Son d'impact
        // - Vibration forte des contrôleurs
        // - Peut-être ralentir le temps brièvement
        
        // Détruire l'obstacle
        Destroy(projectile.gameObject);
    }
    
    // Visualiser le collider
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
            else if (col is CapsuleCollider capCol)
            {
                // Approximation pour le capsule
                Gizmos.DrawWireSphere(transform.position + capCol.center, capCol.radius);
            }
        }
    }
}