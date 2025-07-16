using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator hpTextAnim;
    public Slider healthSlider;

    public Image youDiedImage; // Gán từ Inspector

    private bool isDead = false; // CHẶN GỌI NHIỀU LẦN

    private void Start()
    {
        StatManager.instance.currentHealth = StatManager.instance.maxHealth;
        healthText.text = "HP: " + StatManager.instance.currentHealth + "/" + StatManager.instance.maxHealth;
        healthSlider.maxValue = StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;

        if (youDiedImage != null)
        {
            youDiedImage.gameObject.SetActive(false); // Ẩn ban đầu
            var tempColor = youDiedImage.color;
            tempColor.a = 0;
            youDiedImage.color = tempColor;
        }
    }

    public void UpdateHealth()
    {
        Debug.Log("Updating health");
        healthSlider.maxValue = StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;
    }

    public void ChangeHealth(int amount)
    {
        Debug.Log("Changing health");
        StatManager.instance.currentHealth += amount;
        healthText.text = "HP: " + StatManager.instance.currentHealth + "/" + StatManager.instance.maxHealth;
        healthSlider.value = StatManager.instance.currentHealth;

        if (StatManager.instance.currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(HandleDeath()); // Gọi đúng 1 lần
        }
    }

    private IEnumerator HandleDeath()
    {
        if (youDiedImage != null)
        {
            youDiedImage.gameObject.SetActive(true);

            // Fade in
            float duration = 1f;
            float t = 0f;
            Color c = youDiedImage.color;

            while (t < duration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / duration);
                youDiedImage.color = c;
                yield return null;
            }

            // Giữ trong 3 giây
            yield return new WaitForSeconds(3f);
        }

        // Chuyển về scene Main Menu (Scene 0)
        SceneManager.LoadScene(0);
    }
}
