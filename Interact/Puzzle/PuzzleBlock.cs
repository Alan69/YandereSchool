using UnityEngine;

public class PuzzleBlock : MonoBehaviour {

    [Tooltip("Link to gameobject with puzzle script")]
    public Puzzle puzzle;
    [Tooltip("Puzzle ID")]
    public int ID;

	public void PuzzleMoveBlock()
    {
        puzzle.MovePuzzle(this);
    }
}
