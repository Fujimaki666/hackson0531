using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("Start");
    }
}