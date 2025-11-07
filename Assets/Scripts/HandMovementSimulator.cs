using UnityEngine;

public class HandMovementSimulator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Vitesse de déplacement de la main")]
    public float moveSpeed = 5f;
    
    [Tooltip("Vitesse du coup de poing automatique")]
    public float punchSpeed = 10f;
    
    [Header("Contrôles")]
    [Tooltip("Touche pour coup de poing avant rapide")]
    public KeyCode punchKey = KeyCode.Space;
    
    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool isPunching = false;
    
    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }
    
    void Update()
    {
        // Mouvement manuel avec les flèches
        float horizontal = Input.GetAxis("Horizontal"); // Flèches gauche/droite
        float vertical = Input.GetAxis("Vertical");     // Flèches haut/bas
        
        // Déplacement de la main
        Vector3 movement = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
        transform.localPosition += movement;
        
        // Simulation coup de poing avec Espace
        if (Input.GetKeyDown(punchKey) && !isPunching)
        {
            StartPunch();
        }
        
        // Animation du coup de poing
        if (isPunching)
        {
            AnimatePunch();
        }
    }
    
    void StartPunch()
    {
        isPunching = true;
        targetPosition = originalPosition + new Vector3(0, 0, 1.5f); // Coup vers l'avant
        Debug.Log("🥊 Simulation de coup de poing démarrée!");
    }
    
    void AnimatePunch()
    {
        // Déplacer rapidement vers l'avant
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition, 
            targetPosition, 
            punchSpeed * Time.deltaTime
        );
        
        // Quand on arrive, retourner à la position de départ
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.1f)
        {
            targetPosition = originalPosition;
            
            // Fin du coup
            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.1f)
            {
                isPunching = false;
            }
        }
    }
}