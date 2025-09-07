using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SceneDisplayName : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        tmp.text = sceneName;
    }
}
