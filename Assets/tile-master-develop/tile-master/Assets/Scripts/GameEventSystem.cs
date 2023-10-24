using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public static GameEventSystem current;

    private void Awake()
    {
        current = this;
    }

    // -
    public Func<GameObject, int, int> onSelectedTileMove;
    public int SelectedTileMove(GameObject id, int pivot)
    {
        if (onSelectedTileMove != null)
        {
            return onSelectedTileMove(id, pivot);
        }
        return 0;
    }
    // -

    // --
    public Func<GameObject, int> onMatch_Destroy;
    public int Match_Destroy(GameObject identity)
    {
        if (onMatch_Destroy != null)
        {
            return onMatch_Destroy(identity);
        }
        return 0;
    }
    // --

    // ---
    public Func<GameObject, int, int> onDestroy_RearrangeBar;
    public int RearrangeBar(GameObject identity, int pos)
    {
        if (onDestroy_RearrangeBar != null)
            return onDestroy_RearrangeBar(identity, pos);
        return 0;
    }
    // ---

    // ----
    public Func<Record, int> onUndo;
    public int Undo(Record r)
    {
        if (onUndo != null)
        {
            return onUndo(r);
        }
        return 0;
    }
    // ----

    // -----
    public Func<int, int, int> onRefresh;
    public int Refresh(int layer, int direction)
    {
        if (onRefresh != null)
        {
            return onRefresh(layer, direction);
        }
        return 0;
    }
    // -----

    // ------
    public Func<int, int, int> onBackFromRefresh;
    public int BackFromRefresh(int layer, int direction)
    {
        if (onBackFromRefresh != null)
        {
            return onBackFromRefresh(layer, direction);
        }
        return 0;
    }
    // ------

    // -------
    public Func<GameObject, int> onHint;
    public int Hint(GameObject obj)
    {
        if (onHint != null)
        {
            return onHint(obj);
        }
        return 0;
    }
    // -------
}
