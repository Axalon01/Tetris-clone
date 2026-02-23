using System;
using UnityEngine;
using UnityEngine.UI;

public class HoldDisplayUI : MonoBehaviour
{
    public Board board;
    public Image[] cells = new Image[4];

    public Sprite spriteI;
    public Sprite spriteO;
    public Sprite spriteT;
    public Sprite spriteJ;
    public Sprite spriteL;
    public Sprite spriteS;
    public Sprite spriteZ;

    private Tetromino lastHeldPiece;
    private bool lastHasHeldPiece = false;

    private void Update()
    {
        if (board.hasHeldPiece != lastHasHeldPiece || 
        (board.hasHeldPiece && board.heldPiece.tetromino != lastHeldPiece))
        {
            RefreshDisplay();
            lastHasHeldPiece = board.hasHeldPiece;
            if (board.hasHeldPiece)
                lastHeldPiece = board.heldPiece.tetromino;
        }
    }

    private void RefreshDisplay()
    {
        if (!board.hasHeldPiece)
        {
            foreach (Image cell in cells)
                cell.gameObject.SetActive(false);
            return;
        }

        Sprite sprite = GetSprite(board.heldPiece.tetromino);

        // Use only the first cell as a single image display
        cells[0].gameObject.SetActive(true);
        cells[0].sprite = sprite;
        cells[0].color = Color.white;
        cells[0].SetNativeSize();
    }

    private Sprite GetSprite(Tetromino tetromino)
    {
        switch (tetromino)
        {
            case Tetromino.I: return spriteI;
            case Tetromino.O: return spriteO;
            case Tetromino.T: return spriteT;
            case Tetromino.J: return spriteJ;
            case Tetromino.L: return spriteL;
            case Tetromino.S: return spriteS;
            case Tetromino.Z: return spriteZ;
            default:          return null;  
        }
    }
}
