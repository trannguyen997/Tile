using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileInGame : MonoBehaviour, IPointerDownHandler
{
    private int layer;

    private bool _selected = false;
    private bool _isMoving = false;

    private int collisionCount;
    private bool doneMoving = false;
    private bool undoMoving = false;
    private float originX, originY;

    private IEnumerator _mover;

    private void Start()
    {
        GameEventSystem.current.onRefresh += UponRefresh;
        GameEventSystem.current.onBackFromRefresh += UponBackFromRefresh;
        GameEventSystem.current.onHint += UponHint;

        originX = transform.position.x;
        originY = transform.position.y;

        layer = GetComponent<SpriteRenderer>().sortingOrder;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Tile clicked");

        PlayerClick();
    }

    private void PlayerClick()
    {
        if (_selected || _isMoving)
            return;

        if (BoardManager.instance.CheckIfFull())
            return;

        _selected = true;
        GameEvent();

        if (doneMoving == true)
            doneMoving = false;

        StartCoroutine(MoveToBar());
    }

    IEnumerator MoveToBar()
    {
        BoardManager.instance.StackInBoard.StackUp(gameObject, originX, originY);

        yield return new WaitUntil(() => doneMoving == true);
        doneMoving = false;
        BoardManager.instance.StackInBoard.CheckForMatch();
        BoardManager.instance.CheckIfLose();
    }

    private void GameEvent()
    {
        GameEventSystem.current.onSelectedTileMove += UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy += UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar += UponMatchRearrange;
        GameEventSystem.current.onUndo += UponUndoReset;

        GameEventSystem.current.onRefresh -= UponRefresh;
        GameEventSystem.current.onBackFromRefresh -= UponBackFromRefresh;
        GameEventSystem.current.onHint -= UponHint;
    }

    private void GameEventReset()
    {
        GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
        GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
        GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
        GameEventSystem.current.onUndo -= UponUndoReset;

        GameEventSystem.current.onRefresh += UponRefresh;
        GameEventSystem.current.onBackFromRefresh += UponBackFromRefresh;
        GameEventSystem.current.onHint += UponHint;
    }

    private int UponOtherTileSelected(GameObject id, int pivot)
    {
        if (id == gameObject && _selected)
        {
            if (_isMoving)
            {
                StopCoroutine(_mover);
                _isMoving = false;
            }
            _mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.XBar + pivot, BoardManager.instance.YBar, 0), dur: 1);
            StartCoroutine(_mover);
            GetComponent<SpriteRenderer>().sortingOrder = 1000 + pivot;
        }
        return 0;
    }

    private int UponMatchDestruction(GameObject identity)
    {
        if (identity == gameObject && _selected)
        {
            BoardManager.instance.OnTileDestroy();
            StartCoroutine(WitherAway(dur: 0.375f));
        }
        return 0;
    }

    private IEnumerator WitherAway(float dur)
    {
        yield return StartCoroutine(Wither(dur: 0.375f));
        Destroy(gameObject);
    }

    private IEnumerator Wither(float dur)
    {
        //AudioManager.instance.PlayDestroySound();
        float t = 0f;
        while (t < dur && transform.localScale.x >= 0)
        {
            t += Time.deltaTime;
            transform.localScale -= new Vector3(1f, 1f, 0) * (t * t / dur);
            yield return null;
        }
    }

    private int UponMatchRearrange(GameObject identity, int pos)
    {
        if (_selected && identity == gameObject)
        {
            if (_isMoving)
            {
                StopCoroutine(_mover);
                _isMoving = false;
            }

            _mover = MoveToCoroutine(transform, new Vector3(BoardManager.instance.XBar + pos, BoardManager.instance.YBar, 0), dur: 1);
            StartCoroutine(_mover);
            GetComponent<SpriteRenderer>().sortingOrder = 1000 + pos;
        }

        return 0;
    }

    private int UponUndoReset(Record r)
    {
        if (!_selected) return 0;
        if (gameObject != r.tile) return 0;

        if (_isMoving)
        {
            StopCoroutine(_mover);
            _isMoving = false;
        }

        _mover = MoveToCoroutine(transform, new Vector3(r.prevX, r.prevY, 0), dur: 1);
        StartCoroutine(_mover);

        _selected = false; 
        doneMoving = false;

        GameEventReset();
        undoMoving = true;
        return 0;
    }

    private Vector3 cache;
    private int UponRefresh(int layer, int direction)
    {
        if (_selected || _isMoving) return 0;
        int dif = GetComponent<SpriteRenderer>().sortingOrder - layer;
        if (!(dif >= 0 && dif < 100)) return 0;

        cache = transform.position;

        if (direction == 0)
            transform.position -= new Vector3(15, 0, 0); //to the left
        else if (direction == 1)
            transform.position += new Vector3(0, 15, 0); //go up
        else if (direction == 2)
            transform.position += new Vector3(15, 0, 0); //to the right

        return 0;
    }

    private int UponBackFromRefresh(int layer, int direction)
    {
        if (_selected || _isMoving) return 0;
        int dif = GetComponent<SpriteRenderer>().sortingOrder - layer;
        if (!(dif >= 0 && dif < 100)) return 0;

        if (_isMoving)
        {
            StopCoroutine(_mover);
            _isMoving = false;
        }
        _mover = MoveToCoroutine(transform, cache, 0.3f);
        StartCoroutine(_mover);
        return 0;
    }

    private int UponHint(GameObject obj)
    {
        if (obj == gameObject)
        {
            PlayerClick();
        }
        return 0;
    }

    private IEnumerator MoveToCoroutine(Transform targ, Vector3 pos, float dur)
    {
        _isMoving = true; 
        GetComponent<BoxCollider2D>().enabled = false; 
        gameObject.tag = "Moving";

        float t = 0f;
        Vector3 start = targ.position;
        Vector3 v = pos - start;
        while (t < dur)
        {
            t += Time.deltaTime;
            targ.position = start + v * t / dur;
            yield return null;
        }

        targ.position = pos;

        _isMoving = false; GetComponent<BoxCollider2D>().enabled = true; 
        gameObject.tag = "Untagged";
        
        doneMoving = true;
        if (undoMoving)
        {
            GetComponent<SpriteRenderer>().sortingOrder = layer;
            undoMoving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int _this = GetComponent<SpriteRenderer>().sortingOrder;
        int _other = collision.GetComponent<SpriteRenderer>().sortingOrder;

        if (_this < _other)
            collisionCount++;

        if (collisionCount > 0)
        {
            GetComponent<SpriteRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            gameObject.layer = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int _this = GetComponent<SpriteRenderer>().sortingOrder;
        int _other = collision.GetComponent<SpriteRenderer>().sortingOrder;
        if (_this < _other)
            collisionCount--;

        if (collisionCount == 0)
        {
            GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
            gameObject.layer = 0;
        }
    }

    private void OnDestroy()
    {
        if (_selected)
        {
            GameEventSystem.current.onSelectedTileMove -= UponOtherTileSelected;
            GameEventSystem.current.onMatch_Destroy -= UponMatchDestruction;
            GameEventSystem.current.onDestroy_RearrangeBar -= UponMatchRearrange;
            GameEventSystem.current.onUndo -= UponUndoReset;
        }
        else
        {
            GameEventSystem.current.onRefresh -= UponRefresh;
            GameEventSystem.current.onBackFromRefresh -= UponBackFromRefresh;
            GameEventSystem.current.onHint -= UponHint;
        }
    }
}
