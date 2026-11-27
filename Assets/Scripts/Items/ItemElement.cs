using UnityEngine;
using UnityEngine.UI;

public class ItemElement : MonoBehaviour
{
    public Vector3 PreviousPosition;
    public Vector3 PreviousRotation;

    public ItemState state;

    public Item Parent;

    public SpriteRenderer sr;
    public Image img;

    public void SavePrevious()
    {
        PreviousPosition = transform.position; // assume position is correct
        PreviousRotation = state.GetRotation();
    }

    public void LoadState(ItemState s)
    {
        state = s;
        if (sr != null) sr.sprite = s.ID.sprite;
        if (img != null) img.Set(s.ID.sprite);
    }

    public Vector3 GetIdealLocalPosition()
    {
        return Parent.GetLocalPosition(this);
    }
}
