using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonBehaviour<EventManager>
{
    // 이벤트 딕셔너리 (enum 기반, 델리게이트 타입은 object -> 제너릭 처리할 수 있게)
    private Dictionary<GameProgressEventType, Delegate> eventTable = new Dictionary<GameProgressEventType, Delegate>();
    
    // 이벤트 등록
    public void AddListener<T>(GameProgressEventType eventType, Action<T> listener)
    {
        if (eventTable.TryGetValue(eventType, out var del))
        {
            eventTable[eventType] = Delegate.Combine(del, listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }
    // 이벤트 제거
    public void RemoveListener<T>(GameProgressEventType eventType, Action<T> listener)
    {
        if (eventTable.TryGetValue(eventType, out var del))
        {
            del = Delegate.Remove(del, listener);
            if (del == null)
                eventTable.Remove(eventType);
            else
                eventTable[eventType] = del;
        }
    }
    // 이벤트 발생
    public void TriggerEvent<T>(GameProgressEventType eventType, T param)
    {
        if (eventTable.TryGetValue(eventType, out var del))
        {
            if (del is Action<T> callback)
            {
                callback.Fire(param);
            }
            else
            {
                Debug.LogWarning($"이벤트 {eventType} 타입이 일치하지 않음. 현재 등록된 타입: {del.GetType()}");
            }
        }
    }
    // 파라미터 없는 이벤트도 지원
    public void AddListener(GameProgressEventType eventType, Action listener)
    {
        AddListener<object>(eventType, _ => listener());
    }
    public void RemoveListener(GameProgressEventType eventType, Action listener)
    {
        RemoveListener<object>(eventType, _ => listener());
    }
    public void TriggerEvent(GameProgressEventType eventType)
    {
        TriggerEvent<object>(eventType, null);
    }
}