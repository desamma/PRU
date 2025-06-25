using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Đã chọn Play Game.");
        SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        Debug.Log("Đã chọn Continue Game.");
        if (SaveSystem.SaveExists())
        {
            Debug.Log("Tìm thấy file save. Đang load dữ liệu...");
            PlayerData data = SaveSystem.LoadPlayer();

            SceneManager.LoadSceneAsync(1).completed += (op) =>
            {
                Debug.Log("Scene đã load xong.");
                // Gọi coroutine để đợi StatManager rồi mới gán
                GameObject mainMenuObj = new GameObject("TempLoader");
                mainMenuObj.AddComponent<TempLoader>().StartCoroutine(
                    TempLoader.WaitForStatManagerAndApplyData(data)
                );
            };
        }
        else
        {
            Debug.LogWarning("Không tìm thấy file save.");
        }
    }
}
