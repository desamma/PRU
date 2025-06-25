using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.dat";

    public static void SavePlayer(StatManager stats, Vector2 checkpointPos)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(stats, checkpointPos);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found at " + path);
            return null;
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(path);
    }
}
