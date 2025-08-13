// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
public enum ESaveType
{
    ///<summary>재화</summary>
    Currency = 0,
    ///<summary>해금 데이터</summary>
    Unlocked = 1,
    ///<summary>가구 배치 레이아웃</summary>
    Layout = 2,
    ///<summary>생존 날짜</summary>
    Day = 3,
}

public enum EProductType
{
    ///<summary>조리 기구</summary>
    Machine = 0,
    ///<summary>가구</summary>
    Furniture = 1,
    ///<summary>이사</summary>
    HouseMoving = 2,
}

public enum EPhaseType
{
    ///<summary>영업준비시간</summary>
    PreparingPhase = 0,
    ///<summary>영업시간</summary>
    ServingPhase = 1,
    ///<summary>영업종료</summary>
    EndingPhase = 2,
    ///<summary>연습모드</summary>
    PracticingPhase = 3,
}

public enum ENPCType
{
    ///<summary>일반손님</summary>
    Customer = 0,
}

public enum EAreaType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>홀</summary>
    Hall = 1,
    ///<summary>주방</summary>
    Kitchen = 2,
    ///<summary>창고</summary>
    Storage = 3,
    ///<summary>앞마당</summary>
    FrontYard = 4,
    ///<summary>줄서는 구역</summary>
    Line = 5,
}

public enum EIngredientType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>식물</summary>
    Plants = 1,
    ///<summary>동물</summary>
    Animals = 2,
    ///<summary>결정</summary>
    Crystals = 3,
}

public enum EInputType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>재료</summary>
    Ingredient = 1,
    ///<summary>결과물</summary>
    Output = 2,
    ///<summary>실패 결과물</summary>
    FailureOutput = 3,
    ///<summary>포션</summary>
    Potion = 4,
}

public enum EOutputType
{
    ///<summary>가루</summary>
    PowderOutput = 0,
    ///<summary>액체</summary>
    LiquidOutput = 1,
    ///<summary>실패 결과물</summary>
    FailureOutput = 2,
    ///<summary>포션</summary>
    Potion = 3,
}

public enum EMachineMeshType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>동작 전</summary>
    Ready = 1,
    ///<summary>동작 중</summary>
    Run = 2,
    ///<summary>끝</summary>
    Done = 3,
}

public enum EInteractType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>반복 클릭</summary>
    ClickRepeatly = 1,
    ///<summary>계속 누르기</summary>
    KeepPressing = 2,
    ///<summary>한번 누르기</summary>
    ClickOnce = 3,
    ///<summary>누르면 자동 진행</summary>
    AutoProgress = 4,
}

public enum EStructureType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>머신</summary>
    Machine = 1,
    ///<summary>가구</summary>
    Furniture = 2,
    ///<summary>재료 상자</summary>
    Storage = 3,
}

public enum ESpecialStructureType
{
    ///<summary>없음</summary>
    None = 0,
    ///<summary>픽업테이블</summary>
    PickUpTable = 1,
    ///<summary>계산기</summary>
    Casher = 2,
    ///<summary>쓰레기통</summary>
    TrashCan = 3,
    ///<summary>문</summary>
    Door = 4,
    ///<summary>허름한의자</summary>
    OldChair = 5,
    ///<summary>푹신한의자</summary>
    LuxuryChair = 6,
    ///<summary>연습모드</summary>
    Practice = 7,
}

public enum ECustomerStateType
{
    ///<summary>줄 서기</summary>
    Lining = 0,
    ///<summary>기다리기</summary>
    Sitting = 1,
    ///<summary>챙기기</summary>
    PickingUp = 2,
    ///<summary>나가기</summary>
    Leaving = 3,
}

public enum ETierType
{
    ///<summary>티어 1</summary>
    Tier1 = 0,
    ///<summary>티어 2</summary>
    Tier2 = 1,
    ///<summary>티어 3</summary>
    Tier3 = 2,
}

public enum EUnlockType
{
    ///<summary>포션</summary>
    Potion = 0,
    ///<summary>구조물</summary>
    Structure = 1,
    ///<summary>재료</summary>
    Ingredient = 2,
}

public enum EVFXType
{
    ///<summary>핑</summary>
    Ping = 0,
    ///<summary>플레이어 이동</summary>
    PlayerMovement = 1,
    ///<summary>절구</summary>
    MortarClick = 2,
    ///<summary>분쇄기</summary>
    GrinderClick = 3,
    ///<summary>혼합기</summary>
    MixerClick = 4,
}

