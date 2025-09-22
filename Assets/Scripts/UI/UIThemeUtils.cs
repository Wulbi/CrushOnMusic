using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIThemeUtil
{
    public static void SetLabel(TMP_Text label, bool isSub = false)
    {
        if (!label) return;
        var c = isSub ? UITheme.I.Colors.textSub : UITheme.I.Colors.text;
        label.color = c;
    }

    public static void SetUpgradeButtonVisual(Image btnImage, bool affordable, bool canGradeUp)
    {
        if (!btnImage) return;
        var colors = UITheme.I.Colors;

        if (!affordable) { btnImage.color = colors.disabled; return; }
        btnImage.color = canGradeUp ? colors.warning : colors.success;
    }

    public static void SetMuteVisual(Image iconImage, bool muted)
    {
        if (!iconImage) return;
        iconImage.color = muted ? UITheme.I.Colors.danger : UITheme.I.Colors.text;
    }
}