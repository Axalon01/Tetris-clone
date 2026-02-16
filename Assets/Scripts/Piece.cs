using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position {get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;     // Shows which board it's on
        this.position = position;       // Shows the position it's currently at
        this.data = data;       // Shows what piece it's shaped like

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
}
