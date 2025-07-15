using System.Collections;
using UnityEngine;

public class TempLoader : MonoBehaviour
{
    private PlayerData data;

    public static void CreateLoader(PlayerData data)
    {
        GameObject loaderObj = new GameObject("TempLoader");
        var loader = loaderObj.AddComponent<TempLoader>();
        loader.data = data;
    }

    private void Start()
    {
        StartCoroutine(ApplyDataWhenReady());
    }

    private IEnumerator ApplyDataWhenReady()
    {
        yield return new WaitUntil(() => StatManager.instance != null);
        StatManager.instance.ApplyData(data);

        Vector2 checkpoint = data.GetCheckpoint();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = checkpoint;
            Debug.Log("Dịch chuyển người chơi đến checkpoint: " + checkpoint);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player trong scene.");
        }

        Destroy(gameObject);
    }
}
