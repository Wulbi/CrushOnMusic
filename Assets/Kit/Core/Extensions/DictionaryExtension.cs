using System.Collections.Generic;

/// <summary>
/// Dictionary Extension
/// </summary>
public static class DictionaryExtension {

    /// <summary>
    /// 키를 기반으로 값을 가져오고, 얻은 경우 값을 반환하고, 그렇지 않으면 null을 반환합니다.
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    /// <param name="dict">해당 값을 가져오기 위한 Dictionary</param>
    /// <param name="key">해당 Dictionary Key</param>
    /// <returns></returns>
    public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey,Tvalue> dict,Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }
}
