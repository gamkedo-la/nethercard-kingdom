using UnityEngine;

public class AddManaCheat : MonoBehaviour
{
    [SerializeField] private int cheatAmount = 5;

    public void OnClick()
    {
        SummoningManager.Instance.AddMana(cheatAmount);
    }
}
