using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap Tilemap { get; private set; }
    public Piece ActivePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public TetrominoData NextPiece { get; private set; }
    private int[] bag;
    private int bagIndex;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public TetrominoData heldPiece;
    public bool hasHeldPiece = false;
    public bool CanHold { get; private set; } = true;

    public RectInt Bounds   // This is a property, so it gets capitalized. A property looks like a variable, but runs code when accessed.
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2); // Tetris origin is 0,0, so this is negative (tells it to go left) and divided by half so it starts at -5, -10.
            return new RectInt(position, this.boardSize); // This tells it "Start at -5, -10 and then make a size of this.boardSize (10, 20)
        }
    }

    private void Awake()
    {
        this.Tilemap = GetComponentInChildren<Tilemap>();
        this.ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            // Copy struct, initialize it, write it back (structs don't modify in-place)
            TetrominoData data = this.tetrominoes[i];
            data.Initialize();
            this.tetrominoes[i] = data;
        }
        InitBag();
        NextPiece = DrawFromBag(); // Initialize the next piece so it's ready to go when the first piece spawns
    }

    public void SpawnPiece()
    {
        TetrominoData data = NextPiece; // Use what was queued
        NextPiece = DrawFromBag(); // Queue the next piece

        this.ActivePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(this.ActivePiece, this.spawnPosition))
        {
            Set(this.ActivePiece);
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }

    private void InitBag()
    {
        bag = new int[] { 0, 1, 2, 3, 4, 5, 6 };
        // Fisher-Yates shuffle
        for (int i = bag.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = bag[i];
            bag[i] = bag[j];
            bag[j] = temp;
        }
        bagIndex = 0;
    }

    private TetrominoData DrawFromBag()
    {
        if (bagIndex >= bag.Length) InitBag();
        return tetrominoes[bag[bagIndex++]];
    }

    public void HoldPiece()
    {
        if (!CanHold) return;

        if (!hasHeldPiece)
        {
            // Nothing held yet: Save current piece and spawn a new random one
            heldPiece = this.ActivePiece.Data;
            hasHeldPiece = true;
            Clear(this.ActivePiece);
            SpawnPiece();
        }
        else
        {
            // Swap current piece with held data
            TetrominoData temp = this.ActivePiece.Data;
            Clear(this.ActivePiece);        // Remove the old piece from the board
            this.ActivePiece.Initialize(this, this.spawnPosition, heldPiece);
            heldPiece = temp;
            Set(this.ActivePiece);
        }

        CanHold = false;
    }

    public void ResetHold()
    {
        CanHold = true;
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            this.Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            this.Tilemap.SetTile(tilePosition, null);       // Sets the piece data as null so this can unset the position when it moves to a new one
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) // If the position is NOT within the bounds
            {
                return false;   // Movement isn't valid
            }

            if (this.Tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;   // Track how many lines are cleared

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++; // Increment the count of lines cleared
            }
            else
            {
                row++;
            }
        }

        // If any lines were cleared, tell GameManger
        if (linesCleared > 0)
        {
            GameManager.instance.sfxAudioSource.PlayOneShot(GameManager.instance.lineClearSound); // Play line clear sound when a line is cleared
            GameManager.instance.AddClearedLines(linesCleared);
            GameManager.instance.AddScore(linesCleared);
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.Tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.Tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.Tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
