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
}

public enum ENPCType
{
    ///<summary>일반손님</summary>
    Customer = 0,
}

public enum EPotionType
{
    ///<summary>활력</summary>
    Vitality = 0,
    ///<summary>생명</summary>
    Life = 1,
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
    ///<summary>가루</summary>
    MortarOutput = 3,
    ///<summary>분쇄물</summary>
    GrinderOutput = 4,
    ///<summary>추출물</summary>
    HeatingPotOutput = 5,
    ///<summary>증류원액</summary>
    DistillerOutput = 6,
    ///<summary>냉각물</summary>
    CoolerOutput = 7,
    ///<summary>포션</summary>
    Potion = 8,
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
}

public enum ECustomerStateType
{
    ///<summary>줄 (주문대기)</summary>
    AtLine = 0,
    ///<summary>홀 (포션대기)</summary>
    AtHall = 1,
    ///<summary>이동중</summary>
    Moving = 2,
    ///<summary>포션받기</summary>
    AtCounter = 3,
    ///<summary>퇴장</summary>
    Out = 4,
}

