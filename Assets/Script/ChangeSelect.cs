using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeSelect : MonoBehaviour, ISelectHandler
{
    public void SelectSelf()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void NonSelectSelf()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    
    public void OnSelect(BaseEventData eventData)
    {
        SoundManagerV2.Instance.PlaySE(22);
    }
}