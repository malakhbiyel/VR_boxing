using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float lifeTime = 5f;
    
    [Header("Type")]
    public bool isObstacle = false;
    
    [Header("Scoring")]
    [Tooltip("Points bonus pour avoir évité un obstacle")]
    public int dodgeBonus = 5;
    
    private bool hasBeenHit = false;
    
    void Start()
    {
        // Auto-destruction après X secondes
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        // Avance vers le joueur (direction -Z)
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }
    
    void OnDestroy()
    {
        // Si c'est un obstacle qui n'a pas été touché, le joueur l'a évité!
        if (isObstacle && !hasBeenHit)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.AddScore(dodgeBonus);
                Debug.Log($"✨ OBSTACLE ÉVITÉ! +{dodgeBonus} points (bon réflexe!)");
            }
        }
        // Si c'est une cible qui n'a pas été touchée, elle est ratée
        else if (!isObstacle && !hasBeenHit)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.TargetMissed();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Marquer comme touché pour éviter le double comptage
        if (hasBeenHit) return;
        
        // Vérifier si c'est une main
        if (other.CompareTag("Hand"))
        {
            hasBeenHit = true;
            
            if (isObstacle)
            {
                // Obstacle touché avec la main = pénalité légère
                Debug.Log("⚠️ OBSTACLE TOUCHÉ avec la main! -15 points");
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null) gm.AddScore(-15);
            }
            else
            {
                // Cible touchée = vérifier la vitesse du coup
                PunchDetector punch = other.GetComponent<PunchDetector>();
                if (punch != null && punch.speed >= punch.punchThreshold)
                {
                    // Le HandCollisionDetector gérera le scoring
                    Debug.Log($"🎯 Cible touchée (vitesse: {punch.speed:F2} m/s)");
                }
                else
                {
                    Debug.Log($"⚡ Coup trop faible (vitesse: {punch?.speed:F2} m/s)");
                }
            }
            
            // Détruit l'objet
            Destroy(gameObject);
        }
        // Vérifier si c'est le corps/tête du joueur
        else if (other.CompareTag("Player") || other.CompareTag("Body"))
        {
            hasBeenHit = true;
            
            if (isObstacle)
            {
                // Obstacle a touché le corps = grosse pénalité
                Debug.Log("💥 OBSTACLE A TOUCHÉ TON CORPS! -20 points (baisse-toi!)");
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null) 
                {
                    gm.AddScore(-20);
                    gm.obstaclesHit++;
                }
            }
            
            // Détruit l'objet
            Destroy(gameObject);
        }
    }
}