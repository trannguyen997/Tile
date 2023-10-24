using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TextCore.Text;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [SerializeField] GameObject tile;
    [SerializeField] GameObject Bar;
    [SerializeField] ResultGame resultGame;
    [SerializeField] int gameLevel = 1;

    private StackInBoard stackInBoard;
    private GameObject[] gameTiles;
    private List<Sprite> characters = new List<Sprite>();
    private List<Sprite> all_characters = new List<Sprite>();

    private List<Record> _tileInGame;
    private int _count = 0;
    private int _totalSprites = 8;
    private int _totalLevel = 3;
    private float _xBar, _yBar;
    private float _speed = 0.375f;

    private GameLevelObject _gameLevelObject;
    private int countUndo = 2;
    private int countSuggest = 2;
    private int countReload = 2;

    private void Awake()
    {
        for (int i = 1; i <= _totalSprites; i++)
            all_characters.Add(Resources.Load<Sprite>("Sprites/" + i));
        
        instance = GetComponent<BoardManager>();
        _xBar = -(Bar.GetComponent<SpriteRenderer>().bounds.size.x / 2) + (tile.GetComponent<SpriteRenderer>().bounds.size.x / 55f);
        _yBar = Bar.transform.position.y + (Bar.GetComponent<SpriteRenderer>().bounds.size.y / 55f);

        Application.targetFrameRate = 60;

        StartNewGame(GameLevel);
    }

    public void StartNewGame(int Level = 1)
    {
        if (Level <= 0 || Level >= _totalLevel)
        {
            Debug.Log("Max Level");
            return;
        }


        Debug.Log("?");
        _gameLevelObject = Utility.ReadGameLevelFromAsset(Level);

        characters.Clear();
        characters = all_characters.GetRange(0, SpriteInUse(Level));

        _tileInGame = new List<Record>();
        stackInBoard = new StackInBoard();

        CreateBoard();
    }
    private void CreateBoard()
    {
        int total = _gameLevelObject.tiles.Count;
        gameTiles = new GameObject[total];
        _count = total; // Total of Tile in Board

        int rationTimes = (total / 3);
        int[] TileRation = new int[characters.Count];
        for (int i = 0; i < TileRation.Length; i++)
            TileRation[i] = 0;

        for (int i = 0; i < rationTimes; i++)
        {
            int rand = Random.Range(0, characters.Count);
            TileRation[rand] += 3;
        }

        for (int i = 0; i < total; i++)
        {
            GameObject newTile = Instantiate(tile, new Vector3(_gameLevelObject.tiles[i].x, _gameLevelObject.tiles[i].y, 0), tile.transform.rotation);

            newTile.transform.parent = transform;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = (int)_gameLevelObject.tiles[i].z;
            newTile.name = "Tile_" + i;

            int rand = Random.Range(0, characters.Count);
            while (true)
            {
                if (TileRation[rand] == 0)
                {
                    rand = Random.Range(0, characters.Count); continue;
                }
                else
                    break;
            }
            TileRation[rand] -= 1;
            
            Sprite newSprite = characters[rand];
            newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
            gameTiles[i] = newTile;
        }
    }

    private int SpriteInUse(int lv)
    {
        int result = (lv / 50) + 8;
        
        if (result <= _totalSprites)
            return result;
        else
            return _totalSprites;
    }

    public void AddToRecord(GameObject Tile, float x, float y)
    {
        _tileInGame.Add(new Record(x, y, Tile));
    }

    public void RecordClearMatch(GameObject Tile)
    {
        int count = 0;
        Sprite temp = Tile.GetComponent<SpriteRenderer>().sprite;
        List<Record> needRemoving = new List<Record>();
        foreach (Record i in _tileInGame)
        {
            if (i.tile.GetComponent<SpriteRenderer>().sprite == temp)
            {
                needRemoving.Add(i);
                count++;
            }
        }

        if (count != 3)
            return;
        else
        {
            foreach (Record i in needRemoving)
            {
                _tileInGame.Remove(i);
            }
        }
    }

    public void Delete(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        StartCoroutine(DeleteCallback(chosenTile, NeedRemoving));
    }

    IEnumerator DeleteCallback(LinkedList<GameObject> chosenTile, List<GameObject> NeedRemoving)
    {
        for (int i = 0; i < 3; i++)
            GameEventSystem.current.Match_Destroy(NeedRemoving[i]);

        yield return new WaitForSeconds(0.375f);
        int iterator3 = 0;
        foreach (GameObject i in chosenTile)                                                                      
        // Rearrange the bar
        {
            GameEventSystem.current.RearrangeBar(i, iterator3);
            iterator3++;
        }

        CheckIfWon();
    }

    public bool CheckIfFull()
    {
        return stackInBoard.IsFull();
    }

    public void OnTileDestroy()
    {
        _count = _count - 1;
    }

    public bool UndoRecord()
    {
        if (_tileInGame.Count == 0)
            return false;

        Record r = _tileInGame[_tileInGame.Count - 1];
        _tileInGame.RemoveAt(_tileInGame.Count - 1);

        GameObject tile = r.tile;
        stackInBoard.UponUndo(tile);

        GameEventSystem.current.Undo(r);
        return true;
    }

    public void Hint()
    {
        int[] TileTaken = new int[characters.Count];
        for (int i = 0; i < TileTaken.Length; i++)
            TileTaken[i] = 0;

        List<GameObject> tempo = new List<GameObject>();

        for (int i = 0; i < gameTiles.Length; i++)
        {
            if (!gameTiles[i])                                                                                                   //tile destroyed
            {
                continue;
            }
            else if (gameTiles[i].transform.position.y == YBar || gameTiles[i].tag == "Moving")                                  //tile selected or is moving
            {
                Sprite temp = gameTiles[i].GetComponent<SpriteRenderer>().sprite;
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j] == temp)
                    {
                        TileTaken[j]++;
                        break;
                    }
                }
            }
            else
            {
                tempo.Add(gameTiles[i]);
            }
        }

        int available = stackInBoard.Available();

        for (int i = 0; i < TileTaken.Length; i++)
        {
            int need = 3 - TileTaken[i];
            if (need > available)
                continue;
            List<GameObject> tip = new List<GameObject>();

            foreach (GameObject j in tempo)
            {
                if (j.GetComponent<SpriteRenderer>().sprite == characters[i])
                {
                    tip.Add(j);
                }
            }

            if (tip.Count == 0)
                continue;
            if (tip.Count < need) //dig deeper cause there is not enough tile to form a match
            {

            }
            if (tip.Count >= need)  //this sprite has enough tile available to form a match
            {
                foreach (GameObject j in tip)
                {
                    if (need == 0)
                        break;
                    GameEventSystem.current.Hint(j);
                    need = need - 1;
                }
                break;
            }
        }
    }

    public void Refresh()
    {
        int[] TileRemain = new int[characters.Count];
        for (int i = 0; i < TileRemain.Length; i++)
            TileRemain[i] = 0;

        for (int i = 0; i < gameTiles.Length; i++)
        {
            //tile destroyed
            if (!gameTiles[i])                                                                                                   
                continue;
            //tile selected or is moving
            else if (gameTiles[i].transform.position.y == YBar || gameTiles[i].tag == "Moving")                                 
                continue;
            else
            {
                Sprite temp = gameTiles[i].GetComponent<SpriteRenderer>().sprite;
                for (int j = 0; j < characters.Count; j++)
                {
                    if (characters[j] == temp)
                    {
                        TileRemain[j]++;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < gameTiles.Length; i++)
        {
            //destroyed
            if (!gameTiles[i])                                                                                                   
                continue;
            //selected or moving
            else if (gameTiles[i].transform.position.y == YBar || gameTiles[i].tag == "Moving")                                  
                continue;
            else
            {
                int rand = Random.Range(0, characters.Count);

                while (true)
                {
                    if (TileRemain[rand] == 0)
                    {
                        rand = Random.Range(0, characters.Count); continue;
                    }
                    else
                        break;
                }
                TileRemain[rand] -= 1;

                Sprite newSprite = characters[rand];
                gameTiles[i].GetComponent<SpriteRenderer>().sprite = newSprite;
            }
        }
    }

    public bool CheckIfWon()
    {
        if (_count == 0)
        {
            resultGame.WinGame();
        }
        return _count == 0;
    }

    public bool CheckIfLose()
    {
        if (stackInBoard.IsFull())
            resultGame.LoseGame();
        
        return stackInBoard.IsFull();
    }

    // GETER - SETER
    public float XBar { get => _xBar; private set => _xBar = value; }
    public float YBar { get => _yBar; private set => _yBar = value; }
    public float Speed { get => _speed; private set => _speed = value; }
    public StackInBoard StackInBoard { get => stackInBoard; private set => stackInBoard = value; }
    public int CountUndo { get => countUndo; set => countUndo = value; }
    public int CountSuggest { get => countSuggest; set => countSuggest = value; }
    public int CountReload { get => countReload; set => countReload = value; }
    public int GameLevel { get => gameLevel; set => gameLevel = value; }
}
