using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;     // Shows which board it's on
        this.position = position;       // Shows the position it's currently at
        this.data = data;       // Shows what piece it's shaped like
        this.rotationIndex = 0; // Resets the rotation to 0 on initialization

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

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate(-1);  // Rotate counterclockwise
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Rotate(1);   // Rotate clockwise
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            this.Rotate(1);   // Rotate clockwise
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);  // Calls Move and passes in where I want to move as a parameter ((-1, 0) in this case)
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        this.board.Set(this);   // Sets pieces in new position
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
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

    private void Rotate(int direction)
    {
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4); // Wraps the rotation index between 0 and 3

        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
