using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap Tilemap { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    private void Awake()
    {
        this.Tilemap = GetComponentInChildren<Tilemap>();
    }

    private void LateUpdate()
    {
        // Null check for tracking piece (in case it hasn't been initialized yet)
        if (this.trackingPiece.Cells == null || this.trackingPiece.Cells.Length == 0) return;

        // Initialize cells if needed
        if (this.Cells == null || this.Cells.Length == 0)
        {
            this.Cells = new Vector3Int[this.trackingPiece.Cells.Length];
        }

        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3Int tilePosition = this.Cells[i] + this.Position;
            this.Tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            this.Cells[i] = trackingPiece.Cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.trackingPiece.Position;

        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece); // Clears the tracking piece from the board so the ghost can move down without colliding with it

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (this.board.IsValidPosition(this.trackingPiece, position))
            {
                this.Position = position;
            }
            else
            {
                break;
            }
        }

        this.board.Set(this.trackingPiece); // Sets the tracking piece back on the board after the ghost has found its position
    }

    private void Set()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3Int tilePosition = this.Cells[i] + this.Position;
            this.Tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
