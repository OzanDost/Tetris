using System;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Managers
{
    public class TestGameManager : MonoBehaviour
    {
        private bool _isGameStarted;
        private Piece _currentPiece;

        [Button]
        public void Initialize()
        {
            _isGameStarted = true;

            GetRandomPiece();
        }

        private void GetRandomPiece()
        {
            _currentPiece =
                PoolManager.Instance.GetPiece(
                    (PieceType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PieceType)).Length));
            _currentPiece.transform.position = Vector3.up * 30f;
            _currentPiece.gameObject.SetActive(true);

            _currentPiece.OnPieceStateChanged += OnPieceStateChanged;
        }

        private void OnPieceStateChanged(PieceState oldState, PieceState newState)
        {
            _currentPiece.OnPieceStateChanged -= OnPieceStateChanged;
            if (newState == PieceState.Placed)
                GetRandomPiece();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _currentPiece.Move(Vector2.left * 10);
                Debug.Log("Left");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _currentPiece.Move(Vector2.right * 10);
                Debug.Log("Right");
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _currentPiece.Rotate();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _currentPiece.Move(Vector2.down);
            }
        }

        private void FixedUpdate()
        {
            // if (_currentPiece != null)
                // _currentPiece.Move(Vector2.down * 0.1f);
        }
    }
}