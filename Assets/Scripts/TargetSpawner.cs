using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Cible pour coup de poing droit")]
    public GameObject targetRightPrefab;
    
    [Tooltip("Cible pour coup de poing gauche")]
    public GameObject targetLeftPrefab;
    
    public GameObject obstaclePrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Temps entre chaque spawn")]
    public float spawnInterval = 2f;
    
    [Tooltip("% chance d'obstacle (0-100)")]
    [Range(0, 100)]
    public float obstacleChance = 30f;
    
    [Header("Spawn Position")]
    [Tooltip("Décalage horizontal pour les cibles (positif = droite, négatif = gauche)")]
    public float horizontalOffset = 0.2f;
    
    [Header("Movement")]
    public float projectileSpeed = 3f;
    public float lifeTime = 5f;
    
    [Header("Rotation")]
    [Tooltip("Angle d'inclinaison fixe sur l'axe Z (sera +45 ou -45)")]
    public float tiltAngle = 45f;
    
    void Start()
    {
        // Vérification initiale des prefabs
        Debug.Log("=== VERIFICATION PREFABS ===");
        Debug.Log($"Target Right Prefab: {(targetRightPrefab != null ? targetRightPrefab.name : "NULL")}");
        Debug.Log($"Target Left Prefab: {(targetLeftPrefab != null ? targetLeftPrefab.name : "NULL")}");
        Debug.Log($"Obstacle Prefab: {(obstaclePrefab != null ? obstaclePrefab.name : "NULL")}");
        Debug.Log($"Spawner Position: {transform.position}");
        Debug.Log("============================");
        
        StartCoroutine(SpawnRoutine());
    }
    
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnProjectile();
        }
    }
    
    void SpawnProjectile()
    {
        // Décide obstacle ou cible
        float random = Random.Range(0f, 100f);
        bool spawnObstacle = (random < obstacleChance);
        
        GameObject prefab;
        string targetType = "";
        bool isRight = false;
        
        if (spawnObstacle)
        {
            prefab = obstaclePrefab;
            targetType = "OBSTACLE";
        }
        else
        {
            // Choisir aléatoirement entre target right et target left
            isRight = Random.value > 0.5f;
            prefab = isRight ? targetRightPrefab : targetLeftPrefab;
            targetType = isRight ? "TARGET RIGHT" : "TARGET LEFT";
        }
        
        Debug.Log($"\n[SPAWN] Random: {random:F1} | Seuil: {obstacleChance} | Type: {targetType}");
        
        if (prefab == null)
        {
            Debug.LogError($"[ERROR] Prefab {targetType} est NULL!");
            return;
        }
        
        Debug.Log($"[OK] Prefab found: {prefab.name}");
        
        // Calculer la position de spawn en utilisant l'axe X du monde
        Vector3 spawnPosition = transform.position;
        
        if (!spawnObstacle)
        {
            // Décaler la position pour les cibles sur l'axe X du monde
            if (isRight)
            {
                spawnPosition += Vector3.right * horizontalOffset; // Décalage à droite (axe X mondial)
                Debug.Log($"[POSITION] Target Right à X: {spawnPosition.x}");
            }
            else
            {
                spawnPosition += Vector3.left * horizontalOffset; // Décalage à gauche (axe X mondial)
                Debug.Log($"[POSITION] Target Left à X: {spawnPosition.x}");
            }
        }
        
        // Rotation fixe à +45 ou -45 sur axe Z pour obstacles
        Quaternion spawnRotation = Quaternion.identity;
        if (spawnObstacle)
        {
            // Choisir aléatoirement entre +45 et -45
            float finalAngle = Random.value > 0.5f ? tiltAngle : -tiltAngle;
            spawnRotation = Quaternion.Euler(0, 0, finalAngle);
            Debug.Log($"[ROTATION] Obstacle incliné à {finalAngle} degrés sur axe Z");
        }
        
        // Créer l'objet avec rotation et position
        GameObject obj = Instantiate(prefab, spawnPosition, spawnRotation);
        
        // === DEBUG DÉTAILLÉ ===
        Debug.Log($"[CREATED] Object: {obj.name}");
        Debug.Log($"Position: {obj.transform.position}");
        Debug.Log($"Rotation: {obj.transform.rotation.eulerAngles}");
        Debug.Log($"Active: {obj.activeSelf}");
        Debug.Log($"Children: {obj.transform.childCount}");
        
        // Vérifier les composants
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider col = obj.GetComponent<Collider>();
        Debug.Log($"Rigidbody: {(rb != null ? "YES" : "NO")}");
        Debug.Log($"Collider: {(col != null ? col.GetType().Name : "NO")}");
        
        // Vérifier le child (pour l'obstacle)
        if (obj.transform.childCount > 0)
        {
            Transform child = obj.transform.GetChild(0);
            Debug.Log($"Child name: {child.name}");
            Debug.Log($"Active: {child.gameObject.activeSelf}");
            Debug.Log($"Local Position: {child.localPosition}");
            Debug.Log($"Local Scale: {child.localScale}");
            
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Debug.Log($"MeshRenderer: YES (enabled: {mr.enabled})");
                if (mr.material != null)
                {
                    Debug.Log($"Material: {mr.material.name}");
                    Debug.Log($"Shader: {mr.material.shader.name}");
                    Color color = mr.material.HasProperty("_Color") ? mr.material.color : Color.white;
                    Debug.Log($"Color: {color}");
                }
                else
                {
                    Debug.LogError("[ERROR] Material is NULL!");
                }
            }
            else
            {
                Debug.LogError("[ERROR] No MeshRenderer on child!");
            }
            
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Debug.Log($"Mesh: {mf.sharedMesh.name} ({mf.sharedMesh.vertexCount} vertices)");
            }
            else
            {
                Debug.LogError("[ERROR] No Mesh or MeshFilter!");
            }
        }
        
        // Ajouter le script Projectile
        Projectile proj = obj.AddComponent<Projectile>();
        proj.speed = projectileSpeed;
        proj.lifeTime = lifeTime;
        proj.isObstacle = spawnObstacle;
        
        Debug.Log($"[CONFIGURED] Projectile: speed={projectileSpeed}, lifetime={lifeTime}, isObstacle={spawnObstacle}");
        Debug.Log("==================\n");
    }
    
    // Visual helper to see spawn position in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Visualiser les positions de spawn des cibles (axe X mondial)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * horizontalOffset, 0.3f); // Right
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * horizontalOffset, 0.3f); // Left
    }
}