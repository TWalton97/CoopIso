using UnityEngine.SceneManagement;

//Anything that we want to happen at application start can go here
public class Bootstrapper : Singleton<Bootstrapper>
{
    protected override void Awake()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}