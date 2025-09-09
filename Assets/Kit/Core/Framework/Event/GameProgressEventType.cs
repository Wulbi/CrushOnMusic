/// <summary>
/// 게임 이벤트 리스트
/// </summary>
public enum GameProgressEventType
{
    //터치 시작할 때
    TOUCH_BEGIN,
    //보조 뷰가 업그레이드할 때
    ASSIST_VIEW_UPGRADE,
    //피버 상태가 변경 시
    FEVER_UPDATED,
    //게임 시작
    GAME_STARTED,
    //음악 시작
    MUSIC_BEGIN,
    //음악 종료
    MUSIC_END,
    //업적
    ACHIEVEMENT_UPDATED
}
