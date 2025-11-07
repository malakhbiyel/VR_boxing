using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int score = 0;
    public int targetsHit = 0;
    public int targetsMissed = 0;
    public int obstaclesHit = 0;
    
    [Header("UI References")]
    [Tooltip("Text component pour afficher le score")]
    public Text scoreText;
    
    [Header("Stats")]
    public float accuracy = 0f; // Pourcentage de précision
    
    void Start()
    {
        UpdateScoreUI();
    }
    
    public void AddScore(int points)
    {
        score += points;
        
        // Tracking des stats
        if (points > 0)
        {
            targetsHit++;
        }
        else if (points < 0)
        {
            obstaclesHit++;
        }
        
        // Calculer la précision
        int totalAttempts = targetsHit + obstaclesHit;
        if (totalAttempts > 0)
        {
            accuracy = (float)targetsHit / totalAttempts * 100f;
        }
        
        Debug.Log($"📊 Score: {score} | Cibles: {targetsHit} | Obstacles: {obstaclesHit} | Précision: {accuracy:F1}%");
        
        UpdateScoreUI();
    }
    
    public void TargetMissed()
    {
        targetsMissed++;
        Debug.Log($"😔 Cible ratée | Total manqué: {targetsMissed}");
    }
    
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public int GetScore()
    {
        return score;
    }
    
    public void ResetScore()
    {
        score = 0;
        targetsHit = 0;
        targetsMissed = 0;
        obstaclesHit = 0;
        accuracy = 0f;
        UpdateScoreUI();
        Debug.Log("🔄 Score reset!");
    }
    
    // Afficher les statistiques finales
    public void ShowFinalStats()
    {
        Debug.Log("=== STATISTIQUES FINALES ===");
        Debug.Log($"Score Final: {score}");
        Debug.Log($"Cibles Touchées: {targetsHit}");
        Debug.Log($"Cibles Ratées: {targetsMissed}");
        Debug.Log($"Obstacles Touchés: {obstaclesHit}");
        Debug.Log($"Précision: {accuracy:F1}%");
        Debug.Log("============================");
    }
}