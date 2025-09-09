using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISaveClient
{
    Task Save(string key, object value);

    Task Save(params (string key, object value)[] values);

    Task<string> GetLoadData(string key);
    
    Task<T> Load<T>(string key);

    Task<IEnumerable<T>> Load<T>(params string[] keys);

    Task Delete(string key);

    Task DeleteAll();
}