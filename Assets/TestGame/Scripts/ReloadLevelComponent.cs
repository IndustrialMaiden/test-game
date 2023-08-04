using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestGame.Scripts
{
    public class ReloadLevelComponent : MonoBehaviour
    {
        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}