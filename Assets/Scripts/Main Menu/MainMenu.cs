using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Chọn Play Game.");
        SceneManager.LoadScene(1); // hoặc SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        Debug.Log("Chọn Continue Game.");
        if (SaveSystem.SaveExists())
        {
            PlayerData data = SaveSystem.LoadPlayer();
            SceneManager.LoadSceneAsync(1).completed += (op) =>
            {
                TempLoader.CreateLoader(data);
            };
        }
        else
        {
            Debug.LogWarning("Không có file save.");
        }
    }
}
