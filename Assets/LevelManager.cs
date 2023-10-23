using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelManager : ScriptableObject
{
    public int numberOfTiles; // Số lượng tile trong mỗi level

    public void LoadLevel(int levelNumber)
    {
        // Logic để tải cấp độ dựa trên số cấp độ
    }

    public void NextLevel()
    {
        // Logic để chuyển sang cấp độ tiếp theo
    }
}
