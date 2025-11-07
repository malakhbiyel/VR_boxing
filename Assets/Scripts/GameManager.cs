using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int score = 0;
    
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"📊 Score: {score}");
    }
    
    public int GetScore()
    {
        return score;
    }
}