using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverView : MonoBehaviour
{

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void CreateGameOverView()
    {
        GameOverView view = Resources.Load<GameOverView>("GameOverView");
  
        if (view != null)
        {
            // Instantiate the prefab in the scene
            GameOverView instance = Object.Instantiate(view);
            instance.transform.SetParent(GameObject.Find("Canvas").transform, false);


        }
        else
        {
            Debug.LogError("GameOverView prefab not found in Resources folder.");
        }
    }

    public void DeActivateGameOverView()
    {
        Destroy(gameObject);
        RestartGame();
    }
}
