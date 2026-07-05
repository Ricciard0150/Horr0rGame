using UnityEngine;
using UnityEngine.SceneManagement;

public class BackMenu : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void SceneButton()
    {
        SceneManager.LoadScene(sceneName);
    }
}
