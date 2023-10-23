using UnityEngine;

public class TileScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int tileID; // ID để xác định loại tile

    private void OnMouseDown()
    {
        GameManager.instance.TileSelected(this);
    }

    public void SetSprite(Sprite newSprite, int id)
    {
        spriteRenderer.sprite = newSprite;
        tileID = id;
    }
}
