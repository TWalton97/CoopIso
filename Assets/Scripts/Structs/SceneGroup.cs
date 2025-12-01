using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Scene Groups", menuName = "Scene Group")]
public class SceneGroup : ScriptableObject
{
    public string GroupName = "New Scene Group";
    public List<SceneData> Scenes;

    public string FindSceneNameByType(SceneType sceneType)
    {
        return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.SceneField.SceneName;
    }

}

[Serializable]
public class SceneData
{
    public SceneField SceneField;
    public string Name => SceneField.SceneName;
    public SceneType SceneType;
}

public enum SceneType
{
    ActiveScene,
    MainMenu,
    Player,
    UserInterface,
    Environment,
    Debug
}
