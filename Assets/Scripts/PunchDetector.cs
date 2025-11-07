using UnityEngine;

public class PunchDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Vitesse minimum pour détecter un coup (en mètres/seconde)")]
    public float punchThreshold = 2.0f;
    
    [Header("Debug Info")]
    [Tooltip("Vitesse actuelle de la main")]
    public Vector3 velocity;
    
    [Tooltip("Vitesse en m/s")]
    public float speed;
    
    // Variables privées
    private Vector3 previousPosition;
    private bool isPunching = false;
    
    void Start()
    {
        // Au démarrage, on sauvegarde la position initiale
        previousPosition = transform.position;
    }
    
    void Update()
    {
        // Calculer la vélocité de la main
        // Vélocité = (Position actuelle - Position précédente) / Temps écoulé
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        
        // Calculer la vitesse (magnitude du vecteur vélocité)
        speed = velocity.magnitude;
        
        // Détecter un coup de poing
        if (speed > punchThreshold && !isPunching)
        {
            OnPunchDetected();
            isPunching = true;
        }
        // Réinitialiser quand la main ralentit
        else if (speed < punchThreshold * 0.5f)
        {
            isPunching = false;
        }
        
        // Sauvegarder la position pour le prochain frame
        previousPosition = transform.position;
    }
    
    void OnPunchDetected()
    {
        Debug.Log($" PUNCH! Vitesse: {speed:F2} m/s - Direction: {velocity.normalized}");
        
        // Déterminer le type de coup
        string punchType = GetPunchType(velocity.normalized);
        Debug.Log($" Type de coup: {punchType}");
    }
    
    string GetPunchType(Vector3 direction)
    {
        // Analyse de la direction du coup
        float forward = direction.z;      // Avant/Arrière
        float horizontal = Mathf.Abs(direction.x);  // Gauche/Droite
        float vertical = direction.y;     // Haut/Bas
        
        // Logique de détection du type de coup
        if (vertical > 0.5f)
            return "UPPERCUT ";
        else if (horizontal > 0.6f)
            return "HOOK ";
        else if (forward > 0.5f)
            return "JAB/CROSS ";
        else
            return "BODY SHOT ";
    }
}