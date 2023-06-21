using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Managers
{
    public class TestGameManager : MonoBehaviour
    {
        private bool _isGameStarted;
        private Piece _currentPiece;
        private Vector2 _inputVector;

        [Button]
        public void Initialize()
        {
            _isGameStarted = true;
            _inputVector.y = -0.1f;

            ConfigHelper.Initialize();
            GetRandomPiece();
        }

        private void GetRandomPiece()
        {
            _currentPiece =
                PoolManager.Instance.GetPiece(ConfigHelper.GetRandomPieceType());
            _currentPiece.transform.position = Vector3.up * 30f;
            _currentPiece.gameObject.SetActive(true);

            _currentPiece.PieceStateChanged += PieceStateChanged;
        }

        private void PieceStateChanged(PieceState oldState, PieceState newState)
        {
            _currentPiece.PieceStateChanged -= PieceStateChanged;
            if (newState == PieceState.Placed)
                GetRandomPiece();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _currentPiece.Move(Vector2.left);
                _inputVector.x = -1f;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _inputVector.x = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _currentPiece.Rotate();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _inputVector.y = -1f;
            }
        }

        private void FixedUpdate()
        {
            if (_currentPiece != null)
            {
                _currentPiece.Move(_inputVector);
                if (_inputVector.x != 0) _inputVector.x = 0f;
                if (_inputVector.y != -0.1f) _inputVector.y = -0.1f;
            }
        }
    }
}