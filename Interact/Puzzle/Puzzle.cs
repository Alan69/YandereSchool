using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour {
    [Header("Puzzle Settings")]
    [Tooltip("Empty block gameobject")]
    public PuzzleBlock emptyPuzzle;
    [Tooltip("All puzzle blocks")]
    public PuzzleBlock[] blocks;
    [Tooltip("Event that will occur after solving the puzzle")]
    public UnityEvent SolvedEvent;
    [Tooltip("Sets all puzzles to a random position at the start of the game.")]
    public bool randomize;
    [HideInInspector]
    public bool activated;

    public void Start()
    {
        if (randomize && !CheckBlocks())
        {
            for (int i = 0; i < 500; i++)
            {
                int r = Random.Range(0, blocks.Length);

                if ((blocks[r].transform.localPosition - emptyPuzzle.transform.localPosition).sqrMagnitude < 0.1)
                {
                    Vector3 bp = blocks[r].transform.position;
                    int id = blocks[r].ID;
                    blocks[r].ID = emptyPuzzle.ID;
                    emptyPuzzle.ID = id;
                    blocks[r].transform.position = emptyPuzzle.transform.position;
                    emptyPuzzle.transform.position = bp;
                }
            }

            if (CheckBlocks())
            {
                SolvedEvent.Invoke();
                activated = true;
            }
        }
        else
        {
            if(CheckBlocks())
            {
                SolvedEvent.Invoke();
                activated = true;
            }
        }
    }

    public void MovePuzzle(PuzzleBlock block)
    {
        
        if ((block.transform.localPosition-emptyPuzzle.transform.localPosition).sqrMagnitude < 0.1 && !activated)
        {
            Vector3 bp = block.transform.position;
            int id = block.ID;
            block.ID = emptyPuzzle.ID;
            emptyPuzzle.ID = id;
            block.transform.position = emptyPuzzle.transform.position;
            emptyPuzzle.transform.position = bp;
        }

        if(CheckBlocks() && !activated)
        {
            SolvedEvent.Invoke();
            activated = true;
        }

    }

    private bool CheckBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].ID != i)
            {
                return false;
            }
        }

        return true;
    }

    public void LoadState()
    {
        if(activated)
        {
            SolvedEvent.Invoke();
        }
    }
}
