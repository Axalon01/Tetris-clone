
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public int RotationIndex { get; private set; }

    public float stepDelay = 0.8f;        // How long between automatic piece drops
    public float lockDelay = 0.5f;      // How long a piece sits before locking

    private float stepTime;     // Time before the next drop should happen
    private float lockTime;     // Tracks how long the piece has been grounded
    private int moveCount = 0;
    private const int MaxMovesBeforeLockReset = 15;
    private float moveDelay = 0.1f;    // Minimum time between repeat moves
    private float lastMoveTime;    // Time when the last move was made

    private TetrisControls controls;

    private void Awake()
    {
        controls = new TetrisControls();
        controls.devices = null;
        controls.Gameplay.Pause.performed += ctx =>
        {
            if (GameManager.instance != null && GameManager.instance.gameStarted && !GameManager.instance.isGameOver)
                GameManager.instance.TogglePause();
        };
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.Board = board;     // Shows which board it's on
        this.Position = position;       // Shows the position it's currently at
        this.Data = data;       // Shows what piece it's shaped like
        this.RotationIndex = 0; // Resets the rotation to 0 on initialization
        this.stepTime = Time.time + this.stepDelay; // Sets the step time to the current time plus the step delay (so it will fall after the step delay)
        this.lockTime = 0f;      // Resets the lock time to 0 on initialization
        this.moveCount = 0;     // Resets the move count to 0 on initialization

        if (this.Cells == null)
        {
            this.Cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.Cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        if (GameManager.instance == null) return; // If GameManager instance doesn't exist yet, skip the rest of the update loop
        if (GameManager.instance.isGameOver) return; // If the game is over, skip the rest of the update loop
        if (GameManager.instance.isPaused) return; // If the game is paused, skip the rest of the update loop
        if (this.Cells == null || this.Cells.Length == 0) return;  // Don't update if not initialized

        this.Board.Clear(this); // Clears old positions on board when pieces move

        this.lockTime += Time.deltaTime; // Increments lock time by the time since the last frame


        if (controls.Gameplay.RotateLeft.WasPressedThisFrame())
            Rotate(-1);
        if (controls.Gameplay.RotateRight.WasPressedThisFrame())
            Rotate(1);
        if (controls.Gameplay.HardDrop.WasPressedThisFrame())
            HardDrop();
        if (controls.Gameplay.Hold.WasPressedThisFrame())
            Hold();

        if (controls.Gameplay.MoveLeft.WasPressedThisFrame())
        {
            Move(Vector2Int.left);
            lastMoveTime = Time.time + moveDelay;
        }
        else if (controls.Gameplay.MoveLeft.IsPressed() && Time.time >= lastMoveTime + moveDelay)
        {
            Move(Vector2Int.left);
            lastMoveTime = Time.time;
        }

        if (controls.Gameplay.MoveRight.WasPressedThisFrame())
        {
            Move(Vector2Int.right);
            lastMoveTime = Time.time + moveDelay;
        }
        else if (controls.Gameplay.MoveRight.IsPressed() && Time.time >= lastMoveTime + moveDelay)
        {
            Move(Vector2Int.right);
            lastMoveTime = Time.time;
        }

        if (controls.Gameplay.SoftDrop.WasPressedThisFrame())
        {
            Move(Vector2Int.down);
            lastMoveTime = Time.time + moveDelay;
        }
        else if (controls.Gameplay.SoftDrop.IsPressed() && Time.time >= lastMoveTime + moveDelay)
        {
            Move(Vector2Int.down);
            lastMoveTime = Time.time;
        }

        if (Time.time >= this.stepTime)
            Step();

        this.Board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay; // Resets the step time to the current time plus the step delay (so it will fall after the step delay)
        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        GameManager.instance.sfxAudioSource.PlayOneShot(GameManager.instance.lockSound); // Play lock sound when piece locks in place
        this.Board.Set(this);   // Sets the piece in its final position on the board
        this.Board.ClearLines(); // Clears any lines that are completed by this piece
        this.Board.ResetHold();

        if (!GameManager.instance.isGameOver) // Only spawn a new piece if the game isn't over
        {
            this.Board.SpawnPiece(); // Spawns a new piece after locking the current piece in place
        }

    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Hold()
    {
        if (!this.Board.CanHold) return;
        this.Board.HoldPiece();
    }

    private bool Move(Vector2Int translation)
    {
        // Calculate where the piece would move to
        Vector3Int newPosition = this.Position;
        newPosition.x += translation.x; // Add horizontal movement (-1 for left, +1 for right)
        newPosition.y += translation.y; // Add vertical movement (for falling)

        bool valid = this.Board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.Position = newPosition;

            // Check if piece is grounded (can't move down anymore)
            Vector3Int below = this.Position;
            below.y -= 1;
            bool isGrounded = !this.Board.IsValidPosition(this, below);

            if (isGrounded)
            {
                moveCount++;

                // Only reset lock timer if under move limit
                if (moveCount <= MaxMovesBeforeLockReset)
                {
                    this.lockTime = 0f; // Reset lock time when piece is grounded and has moved less than the max moves before lock reset
                }
            }
            else
            {
                moveCount = 0; // Reset move count when piece is not grounded
                this.lockTime = 0f; // Reset lock time
            }
        }
        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotationIndex = this.RotationIndex;
        this.RotationIndex = Wrap(this.RotationIndex + direction, 0, 4); // Wraps the rotation index between 0 and 3

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.RotationIndex, direction))
        {
            this.RotationIndex = originalRotationIndex; // If rotation isn't valid, reset the rotation index
            ApplyRotationMatrix(-direction); // Rotate back to original position
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3 cell = this.Cells[i];   // Get the current cell position (e.g., (1, 0, 0)). This creates a copy of that cell's position.

            int x, y;

            switch (this.Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));
                    break;
            }

            this.Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.Data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.Data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2; // Each rotation index has 2 wall kick tests (one for clockwise, one for counterclockwise)

        if (rotationDirection < 0) // If rotating counterclockwise, we need to test the previous rotation index's wall kicks
        {
            wallKickIndex -= 1;
        }

        return Wrap(wallKickIndex, 0, this.Data.wallKicks.GetLength(0)); // Wrap the wall kick index to stay within bounds of the wall kick array
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

    public void UpdateStepDelay(float newStepDelay)
    {
        this.stepDelay = newStepDelay;
    }

    public void DisableControls()
    {
        controls.Gameplay.Disable();
    }

    public void EnableControls()
    {
        controls.Gameplay.Enable();
    }
}
