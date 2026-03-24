
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance ;
    public DataGame AllData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
