using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("Not Enough Money");
            return false;
        }
        
        currency -= _price;
        return true;
    }

    public int GetCurrency() => currency;

    private IEnumerator LoadCurrencyDelay(GameData _data)
    {
        yield return new WaitForSeconds(.2f);

        this.currency = _data.currency;
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadCurrencyDelay(_data));
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
