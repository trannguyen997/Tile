using UnityEditor;
using UnityEngine;

public class MyEditorWindowScript : EditorWindow
{
    private Sprite tileSprite;

    [MenuItem("Window/My Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<MyEditorWindowScript>("My Editor Window");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Configuration", EditorStyles.boldLabel);

        // Hiển thị trường để chọn hình ảnh cho tile
        tileSprite = (Sprite)EditorGUILayout.ObjectField("Tile Sprite", tileSprite, typeof(Sprite), false);

        // Kiểm tra nút "Apply" và xử lý khi người dùng nhấn nút này
        if (GUILayout.Button("Apply"))
        {
            ApplyTileSprite();
        }
    }

    private void ApplyTileSprite()
    {
        // Xử lý việc áp dụng hình ảnh cho tile trong GameManager ở đây
        if (tileSprite != null)
        {
            TileScript[] tiles = FindObjectsOfType<TileScript>();
            foreach (TileScript tile in tiles)
            {
                tile.SetSprite(tileSprite, tile.tileID);
            }
        }
    }
}
