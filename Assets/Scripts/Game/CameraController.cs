using Cinemachine;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private CinemachineConfiner2D _confiner;
        [SerializeField] private LayerMask _viewBoxLayerMask;

        private PolygonCollider2D _viewBoxCollider;
        private Bounds _bounds;

        private void Awake()
        {
            Signals.Get<BoardArranged>().AddListener(OnBoardArranged);
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState is GameState.Fail or GameState.Success)
            {
                _targetGroup.m_Targets = new CinemachineTargetGroup.Target[] { };
                _bounds = new Bounds();
            }
        }

        private void OnBoardArranged(Transform[] groundBounds, Transform pieceSpawner)
        {
            CreateViewBox(groundBounds, pieceSpawner);
            foreach (var groundBound in groundBounds)
            {
                _targetGroup.AddMember(groundBound, 1f, 1f);
            }

            _targetGroup.AddMember(pieceSpawner, 1f, 1f);

            _targetGroup.m_PositionMode = CinemachineTargetGroup.PositionMode.GroupAverage;
        }

        private void CreateViewBox(Transform[] groundBounds, Transform pieceSpawner)
        {
            _bounds.Encapsulate(pieceSpawner.position);

            foreach (Transform ground in groundBounds)
            {
                _bounds.Encapsulate(ground.position);
            }

            if (_viewBoxCollider == null)
            {
                GameObject viewBox = new GameObject("ViewBox");
                _viewBoxCollider = viewBox.AddComponent<PolygonCollider2D>();
            }

            // _viewBoxCollider.transform.SetParent(transform);
            _viewBoxCollider.isTrigger = true;
            _viewBoxCollider.gameObject.layer = Utils.LayerMaskToLayer(_viewBoxLayerMask);
            Vector2[] viewBoxPoints =
            {
                new Vector2(_bounds.min.x, _bounds.min.y),
                new Vector2(_bounds.min.x, _bounds.max.y),
                new Vector2(_bounds.max.x, _bounds.max.y),
                new Vector2(_bounds.max.x, _bounds.min.y)
            };

            _viewBoxCollider.points = viewBoxPoints;

            _confiner.m_BoundingShape2D = _viewBoxCollider;

            _virtualCamera.m_Lens.OrthographicSize = Mathf.Abs(_bounds.size.x);
        }
    }
}