public enum EBGMAudioType
{
    Lobby,
    IngamePreparingPhase,
    IngameServingPhase
}

public enum EEffectAudioType
{
    Ping,
    Deliver
}
public enum EPlayerAudioType
{
    Move,
    Hold,
    Drop,
}

public enum EMachineAudioType
{
    Activate,
    Deactivate,
    In,
    Out,
    Done,
    None,
    Mortar,
    Grinder,
    Mixer,
    HeatingPot,
}

public enum EStorageAudioType
{
    Buy
}

public enum ECustomerAudioType
{
    OrderReceived,
    PickupPotion,
    EnduranceZero
}

public enum EPhaseAudioType
{
    EnterPreparingPhase,
    EnterServingPhase,
    EnterEndingPhase,
    EndingPhaseSuccess,
    EndingPhaseFailure
}

public enum EUIAudioType
{
    ButtonClicked,
    ButtonSelected,
    ClientReady,
    HostReady,
    BuyProductSucess,
    BuyProductFailure,
    Error,
}

public enum EPopupAudioType
{
    NewsPaper,
    Guide,
    Market
}