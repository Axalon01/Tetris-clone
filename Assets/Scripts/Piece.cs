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

    private void Update()
    {

        if (this.cells == null || this.cells.Length == 0) return;  // Don't update if not initialized

        this.board.Clear(this); // Clears old positions on board when pieces move

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);  // Calls Move and passes in where I want to move as a parameter ((-1, 0) in this case)
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }

        this.board.Set(this);   // Sets pieces in new position
    }

    private bool Move(Vector2Int translation)
    {
        // Calculate where the piece would move to
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x; // Add horizontal movement (-1 for left, +1 for right)
        newPosition.y += translation.y; // Add vertical movement (for falling)

        bool valid = this.board.isValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
        }
        return valid;
    }
}
