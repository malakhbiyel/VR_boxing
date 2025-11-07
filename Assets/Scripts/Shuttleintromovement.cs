using UnityEngine;
using System.Collections;

public class ShuttleIntroMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float startZ = -100f;
    public float endZ = -45f;
    
    [Header("Rotation Settings")]
    public float rotationAngle = 90f;
    public float rotationDuration = 1.5f;
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent onRotationComplete;

    void Start()
    {
        // Positionne le shuttle au départ SANS démarrer l'animation
        Vector3 startPos = transform.position;
        startPos.z = startZ;
        transform.position = startPos;
        
        Debug.Log("🚀 Shuttle ready at Z=" + startZ + " - Waiting for START button");
    }

    // Appelée par IntroCameraManager quand START est pressé
    public void StartIntroSequence()
    {
        Debug.Log("▶️ Starting shuttle intro sequence!");
        StartCoroutine(ShuttleSequence());
    }

    IEnumerator ShuttleSequence()
    {
        // Phase 1: Avance
        yield return StartCoroutine(MoveAlongZ());
        
        yield return new WaitForSeconds(0.3f);
        
        // Phase 2: Tourne
        yield return StartCoroutine(RotateOnY());
        
        // Déclenche l'événement pour le switch de caméra
        onRotationComplete?.Invoke();
        
        Debug.Log("✅ Shuttle sequence complete!");
    }

    IEnumerator MoveAlongZ()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position;
        targetPosition.z = endZ;
        
        float distance = Mathf.Abs(endZ - startZ);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        Debug.Log($"🚀 Moving shuttle from Z={startZ} to Z={endZ}");
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        Debug.Log("🎯 Shuttle reached platform at Z=" + endZ);
    }

    IEnumerator RotateOnY()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, rotationAngle, 0f);
        
        float elapsed = 0f;
        
        Debug.Log($"🔄 Rotating shuttle {rotationAngle}° on Y axis");
        
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotationDuration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        transform.rotation = targetRotation;
        Debug.Log("✅ Rotation complete!");
    }
}
