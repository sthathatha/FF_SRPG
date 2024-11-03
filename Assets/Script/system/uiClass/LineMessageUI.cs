using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1�s���b�Z�[�WUI
/// </summary>
public class LineMessageUI : MonoBehaviour
{
    public RectTransform bg;
    public TMP_Text text;

    private bool isActive = false;

    /// <summary>
    /// �\��
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(string msg)
    {
        var size = bg.sizeDelta;
        size.y = 0;
        bg.sizeDelta = size;
        gameObject.SetActive(true);
        isActive = true;
        text.SetText(msg);
        StartCoroutine(openBG());
        yield return new WaitWhile(() => isActive);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// BG���L����A�j���[�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator openBG()
    {
        var size = bg.sizeDelta;
        var h = new DeltaFloat();
        h.Set(0f);
        h.MoveTo(80f, 0.2f, DeltaFloat.MoveType.DECEL);
        while (h.IsActive())
        {
            yield return null;
            h.Update(Time.deltaTime);
            size.y = h.Get();
            bg.sizeDelta = size;
        }
    }

    /// <summary>
    /// �L�����Z��
    /// </summary>
    public void ClickCancel()
    {
        isActive = false;
    }
}
