using UnityEngine;

public class Bonfire : MonoBehaviour
{
    private InventoryManager inventory;
    [SerializeField] private Animator playerAnimator;
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
        stats.UpdateUI();
        Debug.Log("Đã hồi máu và stamina.");
    }

    private void SaveCheckpoint()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
        {
            inventory = inventoryCanvas.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                var slots = inventory.inventorySlots;
                Vector2 checkpointPos = transform.position;
                var isRed = playerAnimator.GetBool("isRed");
                SaveSystem.SavePlayer(StatManager.instance, checkpointPos, slots, inventory.gold, isRed);
                Debug.Log("Checkpoint đã được lưu tại: " + checkpointPos);
            }
            else
            {
                Debug.Log("Khong tim thay inventory.");
            }
        }
        else
        {
            Debug.Log("Khong tim thay inventory canvas.");
        }
    }
}
