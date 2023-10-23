using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TileScript tilePrefab;
    public Transform tileParent;
    public Sprite[] tileSprites; // Mảng chứa tất cả các sprite cho tiles

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject tileMapPrefab;

    private GameObject playerInstance;
    private GameObject enemyInstance;
    private GameObject tileMapInstance;

    private TileScript firstSelectedTile;
    private TileScript secondSelectedTile;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerInstance = Instantiate(playerPrefab);
        enemyInstance = Instantiate(enemyPrefab);
        tileMapInstance = Instantiate(tileMapPrefab);

        GenerateTiles();
    }

    public void TileSelected(TileScript tile)
    {
        if (firstSelectedTile == null)
        {
            firstSelectedTile = tile;
        }
        else
        {
            secondSelectedTile = tile;
            CheckMatch();
        }
    }

    private void CheckMatch()
    {
        if (firstSelectedTile.tileID == secondSelectedTile.tileID)
        {
            // Matched
            Destroy(firstSelectedTile.gameObject);
            Destroy(secondSelectedTile.gameObject);
        }
        firstSelectedTile = null;
        secondSelectedTile = null;
    }

    private void GenerateTiles()
    {
        // Tạo ra các tile dựa trên tileSprites
        for (int i = 0; i < tileSprites.Length; i++)
        {
            TileScript newTile = Instantiate(tilePrefab, tileParent);
            newTile.SetSprite(tileSprites[i], i);
        }
    }
}
