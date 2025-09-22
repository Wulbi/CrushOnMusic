using UnityEngine;

[CreateAssetMenu(fileName = "ThemePalette", menuName = "UI/Theme Palette")]
public class ThemePalette : ScriptableObject
{
    [Header("Base")]
    public Color32 bg = FromHex("#0F172A");          // 네이비 계열 배경
    public Color32 text = FromHex("#F9FAFB");        // 기본 글자
    public Color32 textSub = FromHex("#9CA3AF");     // 서브 글자

    [Header("Primary Accents")]
    public Color32 primary = FromHex("#3B82F6");     // 전기 블루
    public Color32 success = FromHex("#22C55E");     // 라임 그린 (구매 가능)
    public Color32 warning = FromHex("#FACC15");     // 등급업 준비(경고/강조)
    public Color32 danger = FromHex("#EF4444");      // 음소거 등 위험/부정
    public Color32 disabled = FromHex("#6B7280");    // 비활성

    [Header("Assist Categories (악기별 포인트)")]
    public Color32 drum = FromHex("#F97316");        // 오렌지
    public Color32 guitar = FromHex("#2DD4BF");      // 민트
    public Color32 bass = FromHex("#8B5CF6");        // 퍼플

    public static Color32 FromHex(string hex) {
        Color c; ColorUtility.TryParseHtmlString(hex, out c);
        return (Color32)c;
    }
}