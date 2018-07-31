using System.IO;
using UnityEngine;

[System.Serializable]
public class ConfigurationData
{
    public int networkPort = 7989;
    public string defaultIP = "localhost";
    public bool startAsServer = true;



    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

public class Configuration : MonoBehaviour
{
    public static ConfigurationData Data { get { return instance.configData; } }
    private static Configuration instance;
#if UNITY_EDITOR
    [SerializeField]
#endif
    private ConfigurationData configData;

    [SerializeField] private string configFile = "configuration.json";

    private void Awake()
    {
        instance = this;
#if !UNITY_EDITOR
    configData = LoadConfig();
#endif
    }

    private void OnApplicationQuit()
    {
        SaveConfig(configData);
    }

    private ConfigurationData LoadConfig()
    {
        try
        {
            return JsonUtility.FromJson<ConfigurationData>(File.ReadAllText(configFile));
        }
        catch
        {
            ConsoleLog.Log.Write("Could not find configuration file. Using default configuration.", ConsoleLog.LogRecordType.Error);
            return new ConfigurationData();
        }
    }

    private void SaveConfig(ConfigurationData write)
    {
        File.WriteAllText(configFile, JsonUtility.ToJson(write, true));
    }
}
