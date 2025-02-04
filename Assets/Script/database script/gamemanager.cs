using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Dictionary<string, bool> repairedParts = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RepairPart(string partName)
    {
        if (!repairedParts.ContainsKey(partName))
        {
            repairedParts.Add(partName, true);
        }
    }

    public bool IsPartRepaired(string partName)
    {
        return repairedParts.ContainsKey(partName) && repairedParts[partName];
    }

    public void HandleObjectState(GameObject obj, string partName)
    {
        if (IsPartRepaired(partName))
        {
            obj.SetActive(false); // Despawn if repaired
        }
        else
        {
            obj.SetActive(true); // Spawn if not repaired
        }
    }
}
