using UnityEngine;

public class ItemElement : MonoBehaviour
{
    public Vector3 PreviousPosition;
    public Vector3 PreviousRotation;

    public ItemState state;

    public Item Parent;

    public SpriteRenderer sr;

    public void SavePrevious()
    {
        PreviousPosition = transform.position; // assume position is correct
        PreviousRotation = state.GetRotation();
    }

    public void LoadState(ItemState s)
    {
        state = s;
        sr.sprite = s.ID.sprite;
    }

    public Vector3 GetIdealLocalPosition()
    {
        return Parent.GetLocalPosition(this);
    }
}
