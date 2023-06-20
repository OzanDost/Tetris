using Enums;
using ThirdParty;
using UnityEngine;

/// <summary>
/// First param is old state, second is new state
/// </summary>
public class GameStateChanged : ASignal<GameState, GameState>{}
public class RequestGameStateChange : ASignal<GameState>{}
public class GameplayStarted : ASignal<GameMode>{}

public class BoardArranged : ASignal<Transform, Transform>{}

public class LifeLost : ASignal{}

//UI Signals

public class FakeLoadingFinished : ASignal{}
public class PlayButtonClicked : ASignal{}
public class ContinueButtonClicked : ASignal{}
public class SoloPlayButtonClicked : ASignal{}
public class MultiplayerPlayButtonClicked : ASignal{}
public class SoloModeButtonClicked : ASignal{}
public class MultiplayerModeButtonClicked : ASignal{}

// Powerup Signals

public class OnLightningButtonClicked : ASignal{}
public class OnFreezeButtonClicked : ASignal{}
