using Enums;
using Game;
using ThirdParty;
using UnityEngine;

/// <summary>
/// First param is old state, second is new state
/// </summary>
public class GameStateChanged : ASignal<GameState, GameState>{}
public class RequestGameStateChange : ASignal<GameState>{}
public class GameplayStarted : ASignal<GameMode>{}

public class BoardArranged : ASignal<Transform[], Transform[], Transform>{}
public class AIBoardArranged : ASignal<Transform[], Transform[], Transform>{}

public class LifeLost : ASignal{}
public class LivesFinished : ASignal{}

public class LevelFinished : ASignal<bool>{}
public class LevelQuit : ASignal{}
public class TogglePause: ASignal<bool>{}
public class GameplayRequested : ASignal<GameMode>{}


//UI Signals

public class FakeLoadingFinished : ASignal{}
public class RetryButtonClicked : ASignal{}


// Powerup Signals

public class OnLightningButtonClicked : ASignal{}
public class OnFreezeButtonClicked : ASignal{}

//Board Signals

public class CurrentPieceChanged : ASignal<Piece>{}
public class NextPieceChanged : ASignal<Piece>{}
public class PiecePlaced : ASignal<Piece>{}
public class AIPiecePlaced : ASignal<float>{}
public class AIBoardHeightChanged:ASignal<float>{}
public class BoardHeightCalculated : ASignal<float>{}


//Player Input Signals

public class VerticalSpeedToggled : ASignal<bool>{}
public class HorizontalInputGiven: ASignal<float>{}
public class RotateInputGiven : ASignal{}


// Versus Signals

public class AIMistakesFilled : ASignal{}
public class AiPieceChanged : ASignal<Piece>{}
