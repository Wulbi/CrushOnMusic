using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class UIPanelInfo :ISerializationCallbackReceiver
{

    [System.NonSerialized]  
    public UIPanelType panelType;

    public string panelTypeString;

    public string path;
    
    public void OnBeforeSerialize()
    {
        
    }
    public void OnAfterDeserialize()
    {
        UIPanelType type = (UIPanelType)System.Enum.Parse(typeof(UIPanelType), panelTypeString);
        panelType = type;
    }
}
