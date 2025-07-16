using UnityEngine;
using UnityEngine.SceneManagement;

public class BossWatcher : MonoBehaviour
{
    public string bossTag = "Boss1"; // Gán tag "StoneBoss" trong Inspector

    private void Update()
    {
        GameObject boss = GameObject.FindGameObjectWithTag(bossTag);

        if (boss == null)
        {
            Debug.Log("Boss đã chết, chuyển cảnh...");
            SceneManager.LoadScene(2); // Load scene index 2
        }
    }
}
