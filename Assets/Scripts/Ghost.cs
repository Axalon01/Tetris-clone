using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
    }

    private void LateUpdate()
    {
        // Null check for tracking piece (in case it hasn't been initialized yet)
        if (this.trackingPiece.cells == null || this.trackingPiece.cells.Length == 0) return;

        // Initialize cells if needed
        if (this.cells == null || this.cells.Length == 0)
        {
            this.cells = new Vector3Int[this.trackingPiece.cells.Length];
        }

        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;

        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece); // Clears the tracking piece from the board so the ghost can move down without colliding with it

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (this.board.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
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
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
