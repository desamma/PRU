using UnityEngine;

public class Bonfire : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào bonfire.");
            HealPlayer();
            SaveCheckpoint();
        }
    }

    private void HealPlayer()
    {
        var stats = StatManager.instance;
        stats.currentHealth = stats.maxHealth;
        stats.currentStamina = stats.maxStamina;
        Debug.Log("Đã hồi máu và stamina.");
    }

    private void SaveCheckpoint()
    {
        Vector2 checkpointPos = transform.position;
        SaveSystem.SavePlayer(StatManager.instance, checkpointPos);
        Debug.Log("Checkpoint đã được lưu tại: " + checkpointPos);
    }
}
