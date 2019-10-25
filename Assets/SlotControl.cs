using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
    }

    public void OnRelease()
    {
        DeckBuilder.Instance.otherSlot = transform.parent.gameObject;
        DeckBuilder.Instance.MoveSlot();
    }
}
