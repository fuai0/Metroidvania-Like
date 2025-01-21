using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    [SerializeField] private Checkpoint[] checkpoints;

    [Header("lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    private Transform player;
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

        player = PlayerManager.instance.player.transform;
    }

    public void RestartScence()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
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

        foreach (var checkpoint in checkpoints)
        {
            if (_data.closestCheckpointId == checkpoint.id)
                player.position = checkpoint.transform.position;
        }

        StartCoroutine(LoadWithDelay(_data));
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;
        lostCurrencyAmount = _data.lostCurrencyAmount;

        if (lostCurrencyAmount > lostCurrencyX)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);
        LoadLostCurrency(_data);
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if (FindClosestCheckpoint() != null)
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
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activated)
            {
                closestCheckPoint = checkpoint;
                closestDistance = distanceToCheckpoint;
            }
        }

        return closestCheckPoint;
    }
}
