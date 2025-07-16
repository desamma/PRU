using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy_Dragon_Health : MonoBehaviour
{
    public int expReward = 3;
    public float deathAnimationDuration = 2f; // Thời gian chờ trước khi hiện ảnh win
    public Image youWinImage; // Gán trong Inspector
    public int sceneToLoad = 0; // Scene chiến thắng (main menu, victory...)

    public delegate void MonsterDefeted(int exp);
    public static event MonsterDefeted OnMonsterDefeated;
    public event Action OnEnemyDestroyed;

    public int currentHealth;
    public int maxHealth;

    private Enemy_Dragon_Movement dragonMovement;
    private bool isDying = false;

    private void Start()
    {
        currentHealth = maxHealth;
        dragonMovement = GetComponent<Enemy_Dragon_Movement>();

        if (youWinImage != null)
        {
            youWinImage.gameObject.SetActive(false);
            var tempColor = youWinImage.color;
            tempColor.a = 0;
            youWinImage.color = tempColor;
        }
    }

    public void ChangeHealth(int amount)
    {
        if (isDying) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(ShowVictoryImage());
            StartDeathSequence(); 
        }
    }

    private void StartDeathSequence()
    {

        isDying = true;

        if (dragonMovement != null)
        {
            dragonMovement.ChangeState(EnemyDragonState.Die);
        }
        
        DisableAllColliders();

        OnMonsterDefeated?.Invoke(expReward);
        OnEnemyDestroyed?.Invoke();
        

    }

    private void DisableAllColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in childColliders)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator ShowVictoryImage()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        if (youWinImage != null)
        {
            youWinImage.gameObject.SetActive(true);

            float duration = 1f;
            float t = 0f;
            Color c = youWinImage.color;

            while (t < duration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / duration);
                youWinImage.color = c;
                yield return null;
            }

            //hien you win!
            yield return new WaitForSeconds(10f);
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
