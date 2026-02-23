using UnityEngine;
using UnityEngine.Tilemaps;

public class HoldDisplay : MonoBehaviour
{
    public Tilemap tilemap;
    public Board board;

    private Vector3Int[] cells = new Vector3Int[4];
    public Vector3Int displayPosition = new Vector3Int(0, 0, 0);   // Adjust in inspector

    private void LateUpdate()
    {
        Clear();

        if (!board.hasHeldPiece) return;

        for (int i = 0; i < board.heldPiece.cells.Length; i++)
        {
            cells[i] = (Vector3Int)board.heldPiece.cells[i];
        }

        Draw();
    }

    private void Clear()
    {
        Vector3Int centeringOffset = GetCenteringOffset(board.heldPiece);
        for (int i = 0; i < cells.Length; i++)
        {
            tilemap.SetTile(cells[i] + displayPosition + displayPosition, null);
        }
    }
    
    private void Draw()
    {

        Vector3Int centeringOffset = GetCenteringOffset(board.heldPiece);

        for (int i = 0; i < cells.Length; i++)
        {
            tilemap.SetTile(cells[i] + displayPosition + centeringOffset, board.heldPiece.tile);
        }
    }

    private Vector3Int GetCenteringOffset(TetrominoData piece)
    {
        switch (piece.tetromino)
        {
            case Tetromino.I: return new Vector3Int(0, 0, 0);
            case Tetromino.O: return new Vector3Int(0, 1, 0);
            default:          return new Vector3Int(0, 0, 0);
        }
    }
}
