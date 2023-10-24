using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackInBoard
{
    LinkedList<GameObject> chosenTile = new LinkedList<GameObject>();
    
    public int StackUp(GameObject Tile, float x, float y)                                 
    {
        //insert new tile to the list, calculate its position in the list
        int position = CalculatePosititonOfNewTile(Tile, x, y);

        //adjust the bar so the new chosen tile can fit in
        int iterator = 0;                                                                
        foreach (GameObject i in chosenTile)
        {
            GameEventSystem.current.SelectedTileMove(i, iterator);
            iterator++;
        }

        return position;                                                                  //return position of the new tile so move it
    }

    private int CalculatePosititonOfNewTile(GameObject Tile, float x, float y)            //insert new tile to the list, calculate its position in the list
    {
        if (chosenTile.Count == 0)
        //IF: list is empty (no tile chosen), so add first
        {
            BoardManager.instance.AddToRecord(Tile, x, y); BoardManager.instance.RecordClearMatch(Tile);
            chosenTile.AddFirst(Tile);
            return 0;
        }
        else
        //ELSE: list consists of tile(s)
        {
            int iterator = 0;
            int match = 0;
            Sprite a = Tile.GetComponent<SpriteRenderer>().sprite;
            //iterate the whole list from start to end, find the position of the last tile with the same type as the new tile
            foreach (GameObject i in chosenTile)                                          
            {                                                                            
                Sprite b = i.GetComponent<SpriteRenderer>().sprite;                      
                if (a == b)
                    match = iterator;

                iterator++;
            }

            // Check if the result from the loop above is the first one in the list
            bool match_Position_Zero = false;                                             
            if (a == chosenTile.First.Value.GetComponent<SpriteRenderer>().sprite)
                match_Position_Zero = true;


            if (match == 0 && !match_Position_Zero)
            //IF: tile type doesnt exist in the list, add to last 
            {
                int position = chosenTile.Count;
                BoardManager.instance.AddToRecord(Tile, x, y); BoardManager.instance.RecordClearMatch(Tile);
                chosenTile.AddLast(Tile);
                return position; // Last Position
            }
            else
            //ELSE: tile type existed and match is the last position of the matching tile
            {
                int iterate = 0;
                GameObject j = null;
                foreach (GameObject i in chosenTile)
                {
                    if (iterate == match)
                    {
                        j = i;
                        break;
                    }
                    iterate++;
                }
                BoardManager.instance.AddToRecord(Tile, x, y);
                BoardManager.instance.RecordClearMatch(Tile);
                chosenTile.AddAfter(chosenTile.Find(j), Tile);

                return match + 1;
            }
        }
    }

    // Check if there is A match in the Board
    public bool CheckForMatch()                                                         
    {
        if (chosenTile.Count < 3)
            return false;
        
        bool result = false;
        int MATCH = 0;
        GameObject before = chosenTile.First.Value;
        int iterator = 0;

        foreach (GameObject i in chosenTile)
        // This loop is used to check if there is a match in the list
        {
            if (before.GetComponent<SpriteRenderer>().sprite == i.GetComponent<SpriteRenderer>().sprite)
                MATCH++;
            else
            {
                before = i;
                MATCH = 1;
            }

            if (MATCH == 3)
            {
                result = true;
                break;
            }

            iterator++;
        }

        //IF: if there is no match, return
        if (result == false)
            return false;

        int iterator2 = 0;
        // A list to hold tiles that are in a match 
        List<GameObject> NeedRemoving = new List<GameObject>();                           
        foreach (GameObject i in chosenTile)
        // Find those tiles in a match and put it in the list
        {
            if (iterator2 == iterator)
            {
                NeedRemoving.Add(i);
                break;
            }
            if (iterator2 == iterator - 1 || iterator2 == iterator - 2)
            {
                NeedRemoving.Add(i);
            }
            iterator2++;
        }

        // Remove those tiles from the list(they still exist)
        for (int i = 0; i < 3; i++)                                                       
            chosenTile.Remove(NeedRemoving[i]);

        //Destroy those tiles then rearrange the bar
        BoardManager.instance.Delete(chosenTile, NeedRemoving);                           

        return result;
    }

    public bool IsFull()
    {
        return chosenTile.Count >= 7;
    }

    public void UponUndo(GameObject Tile)
    {
        chosenTile.Remove(Tile);
        int iterator3 = 0;
        foreach (GameObject i in chosenTile)
        {
            GameEventSystem.current.RearrangeBar(i, iterator3);
            iterator3++;
        }
    }

    public int Available()
    {
        return 7 - chosenTile.Count;
    }



    //public void Darken(float a, float b, float c)
    //{
    //    foreach (GameObject i in chosenTile)
    //    {
    //        i.GetComponent<SpriteRenderer>().material.color = new Color(a, b, c, 1f);
    //    }
    //}

    //public void Lighten()
    //{
    //    foreach (GameObject i in chosenTile)
    //    {
    //        i.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
    //    }
    //}


}
