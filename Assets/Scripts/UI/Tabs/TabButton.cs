using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private TabGroup tabGroup;
    private Image background;
    public GameObject tabContainer;
    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup?.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup?.OnTabHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup?.OnTabExit(this);
    }

    private void Awake()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        background = GetComponent<Image>();

        if(tabContainer == null){
            tabContainer = this.transform.GetChild(0).gameObject;
        }

        background.sprite = tabGroup?.tabIdle;
    }

    public void SetBackground(Sprite _sprite){
        background.sprite = _sprite;
    }

    public void Select()
    {
        OnTabSelected?.Invoke();

        background.sprite = tabGroup?.tabActive;

        tabContainer?.SetActive(true);
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke();

        background.sprite = tabGroup?.tabIdle;

        tabContainer?.SetActive(false);
    }
}
