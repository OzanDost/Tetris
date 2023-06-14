using Game.Managers;
using ThirdParty;

/// <summary>
/// First param is old state, second is new state
/// </summary>
public class GameStateChanged : ASignal<GameState, GameState>{}
public class RequestGameStateChange : ASignal<GameState>{}


//UI Signals

public class FakeLoadingFinished : ASignal{}
public class PlayButtonClicked : ASignal{}
public class ContinueButtonClicked : ASignal{}
public class SoloPlayButtonClicked : ASignal{}
public class MultiplayerPlayButtonClicked : ASignal{}
public class SoloModeButtonClicked : ASignal{}
public class MultiplayerModeButtonClicked : ASignal{}
