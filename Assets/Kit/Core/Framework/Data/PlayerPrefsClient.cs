using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 로컬 저장용
/// </summary>
public class PlayerPrefClient : ISaveClient
{
    public Task Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
        return Task.CompletedTask;
    }

    public Task DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        return Task.CompletedTask;
    }
    
    public Task<string> GetLoadData(string key) => Task.FromResult<string>(PlayerPrefs.GetString(key));
    
    public Task<T> Load<T>(string key)
    {
        var data = PlayerPrefs.GetString(key);
        if (!string.IsNullOrEmpty(data)) 
            return Task.FromResult(JsonUtility.FromJson<T>(data));
        
        return Task.FromResult<T>(default);
    }

    public async Task<IEnumerable<T>> Load<T>(params string[] keys)
    {
        return await Task.WhenAll(keys.Select(Load<T>));
    }

    public Task Save(string key, object value)
    {
        if (value is string s)
        {
            PlayerPrefs.SetString(key, s);
        }
        else
        {
            var data = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, data);
        }
        //PlayerPrefs.Save();
        return Task.CompletedTask;
    }

    public async Task Save(params (string key, object value)[] values)
    {
        foreach (var (key, value) in values)
        {
            await Save(key, value);
        }
    }
}