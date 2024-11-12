using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HP�Q�[�W
/// </summary>
public class HPGauge : MonoBehaviour
{
    #region �萔

    private const float G_MAX = 62f;
    private const float G_HEIGHT = 2f;

    #endregion

    #region �����o�[

    public Transform gMain;
    public Transform gBlue;
    public Transform gRed;

    #endregion

    #region �ϐ�

    private DeltaFloat rate_main = new DeltaFloat();
    private DeltaFloat rate_red = new DeltaFloat();
    private IEnumerator animCoroutine = null;

    #endregion

    #region �ݒ�

    /// <summary>
    /// HP�����\��
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    public void SetHP(int current, int max)
    {
        rate_main.Set((float)current / max);
        rate_red.Set(0);
        SetGauge(gMain, rate_main.Get());
        SetGauge(gBlue, 0);
        SetGauge(gRed, 0);
    }

    /// <summary>
    /// HP�ω��A�j���[�V����
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    public void AnimHP(int current, int max)
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = AnimHPCoroutine(current, max);
        StartCoroutine(animCoroutine);
    }

    /// <summary>
    /// HP�\���R���[�`��
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    private IEnumerator AnimHPCoroutine(int current, int max)
    {
        var rate = (float)current / max;
        if (rate > rate_main.Get())
        {
            // �����Ă�̂ŉ񕜈���
            rate_red.Set(0);
            SetGauge(gRed, rate_red.Get());
            SetGauge(gBlue, rate);
            rate_main.Set(rate_main.Get());
            yield return new WaitForSeconds(0.2f);
            rate_main.MoveTo(rate, 0.3f, DeltaFloat.MoveType.LINE);
            while (rate_main.IsActive())
            {
                yield return null;
                SetGauge(gMain, rate_main.Get());
            }
        }
        else
        {
            // �����Ă�̂Ń_���[�W����
            SetGauge(gBlue, 0);
            rate_red.Set(rate_main.Get());
            SetGauge(gRed, rate_red.Get());
            rate_main.Set(rate);
            SetGauge(gMain, rate);
            yield return new WaitForSeconds(0.2f);
            rate_red.MoveTo(rate, 0.3f, DeltaFloat.MoveType.LINE);
            while (rate_red.IsActive())
            {
                yield return null;
                SetGauge(gRed, rate_red.Get());
            }
        }
    }

    #endregion

    #region �v���C�x�[�g

    /// <summary>
    /// �Q�[�W�����Z�b�g
    /// </summary>
    /// <param name="gg"></param>
    /// <param name="rate"></param>
    private void SetGauge(Transform gg, float rate)
    {
        var width = rate * G_MAX;
        var x = (width - G_MAX) / 2f;

        gg.localScale = new Vector3(width, G_HEIGHT, 1);
        gg.localPosition = new Vector3(x, 0);
    }

    #endregion
}
