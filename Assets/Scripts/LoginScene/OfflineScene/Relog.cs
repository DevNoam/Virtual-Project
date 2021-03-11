using UnityEngine;
using UnityEngine.SceneManagement;

public class Relog : MonoBehaviour
{

    public GameObject disconnectedPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        DestroyAllGameObjects();
    }
    public void DestroyAllGameObjects()
    {
        GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

        for (int i = 0; i < GameObjects.Length; i++)
        {
            if (GameObjects[i].name == "OfflineManager") { 
            
            }
            else
            {
                Destroy(GameObjects[i]);
            }
        }
        Resources.UnloadUnusedAssets();
        DontDestroyOnLoad(Instantiate(disconnectedPrefab));
        SceneManager.LoadScene(1);
    }
}
