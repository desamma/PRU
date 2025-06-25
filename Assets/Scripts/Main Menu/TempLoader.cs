using UnityEngine;
using System.Collections;

public class TempLoader : MonoBehaviour
{
    public static IEnumerator WaitForStatManagerAndApplyData(PlayerData data)
    {
        Debug.Log("⏳ Đang chờ StatManager được khởi tạo...");
        while (StatManager.instance == null)
            yield return null;

        Debug.Log("✅ StatManager đã sẵn sàng. Đang gán dữ liệu...");

        // Health
        StatManager.instance.maxHealth = data.maxHealth;
        StatManager.instance.currentHealth = data.currentHealth;

        // Combat
        StatManager.instance.damage = data.damage;
        StatManager.instance.weaponRange = data.weaponRange;
        StatManager.instance.knockbackForce = data.knockbackForce;
        StatManager.instance.knockbackTime = data.knockbackTime;
        StatManager.instance.stunTime = data.stunTime;
        StatManager.instance.cooldown = data.cooldown;

        // Movement
        StatManager.instance.maxStamina = data.maxStamina;
        StatManager.instance.currentStamina = data.currentStamina;
        StatManager.instance.staminaRegenRate = data.staminaRegenRate;
        StatManager.instance.staminaCost = data.staminaCost;
        StatManager.instance.dashDelay = data.dashDelay;
        StatManager.instance.dashDuration = data.dashDuration;
        StatManager.instance.moveSpeed = data.moveSpeed;

        // Exp
        StatManager.instance.currentExp = data.currentExp;
        StatManager.instance.expToNextLevel = data.expToNextLevel;
        StatManager.instance.level = data.level;
        StatManager.instance.upgradePoint = data.upgradePoint;

        // Vị trí
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector2(data.checkpointX, data.checkpointY);
            Debug.Log($"✅ Đã đặt vị trí player tại checkpoint: ({data.checkpointX}, {data.checkpointY})");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy Player sau khi load scene.");
        }

        Destroy(GameObject.Find("TempLoader"));
    }
}
