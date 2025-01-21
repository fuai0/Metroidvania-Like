using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    [SerializeField] private Checkpoint[] checkpoints;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else
            instance = this;

    }

    private void Start()
    {
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.InstanceID);
    }

    public void RestartScence()
    {
        SaveManager.instance.SaveGame();

        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
        SaveManager.instance.LoadGame();
    }

    public void LoadData(GameData _data)
    {
        foreach (var pair in _data.checkpoints)
        {
            foreach (var checkpoint in checkpoints)
            {
                if (pair.Key == checkpoint.id && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }

        foreach(var checkpoint in checkpoints)
        {
            if(_data.closestCheckpointId == checkpoint.id)
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;

        _data.checkpoints.Clear();

        foreach (var checkpoint in checkpoints)
            _data.checkpoints.Add(checkpoint.id, checkpoint.activated);
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckPoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activated)
            {
                closestCheckPoint = checkpoint;
                closestDistance = distanceToCheckpoint;
            }
        }

        return closestCheckPoint;
    }
}
