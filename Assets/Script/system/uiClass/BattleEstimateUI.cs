using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// バトル結果予測UI
/// </summary>
public class BattleEstimateUI : MonoBehaviour
{
    #region メンバー

    public Transform windowBase;
    public TMP_Text p_hp;
    public TMP_Text e_hp;
    public TMP_Text p_atk;
    public TMP_Text e_atk;
    public TMP_Text p_hit;
    public TMP_Text e_hit;
    public TMP_Text p_crt;
    public TMP_Text e_crt;
    public TMP_Text p_count;
    public TMP_Text e_count;
    public GameObject p_countWnd;
    public GameObject e_countWnd;

    #endregion

    #region 表示結果

    /// <summary>
    /// 選択結果
    /// </summary>
    public enum BattleSelectResult
    {
        Display = 0,
        Cancel,
        Execute,
    }
    /// <summary>選択結果</summary>
    public BattleSelectResult Result { get; private set; }

    #endregion

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="param"></param>
    /// <param name="pc"></param>
    /// <param name="enm"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(GameParameter.BattleParameter param, PlayerCharacter pc, EnemyCharacter enm)
    {
        ShowParameter(param, pc, enm);
        gameObject.SetActive(true);
        Result = BattleSelectResult.Display;
        yield return new WaitWhile(() => Result == BattleSelectResult.Display);
        gameObject.SetActive(false);
    }

    public void ClickCancel()
    {
        Result = BattleSelectResult.Cancel;
    }

    public void ClickExec()
    {
        Result = BattleSelectResult.Execute;
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="param"></param>
    /// <param name="pc"></param>
    /// <param name="enm"></param>
    private void ShowParameter(GameParameter.BattleParameter param, PlayerCharacter pc, EnemyCharacter enm)
    {
        p_hp.SetText(pc.param.HP.ToString());
        e_hp.SetText(enm.param.HP.ToString());
        p_atk.SetText(param.a_dmg.ToString());
        p_hit.SetText(param.a_hit.ToString());
        p_crt.SetText(param.a_critical.ToString());
        if (param.d_atkCount == 0)
        {
            // 反撃不可
            e_atk.SetText("--");
            e_hit.SetText("--");
            e_crt.SetText("--");
        }
        else
        {
            e_atk.SetText(param.d_dmg.ToString());
            e_hit.SetText(param.d_hit.ToString());
            e_crt.SetText(param.d_critical.ToString());
        }

        // 攻撃回数
        if (param.a_atkCount > 1)
        {
            p_countWnd.SetActive(true);
            p_count.SetText($"x{param.a_atkCount}");
        }
        else
            p_countWnd.SetActive(false);
        if (param.d_atkCount > 1)
        {
            e_countWnd.SetActive(true);
            e_count.SetText($"x{param.d_atkCount}");
        }
        else
            e_countWnd.SetActive(false);

        // プレイヤーによって位置調整
        var pos = windowBase.localPosition;
        if (pc.GetLocation().x >= FieldSystem.COL_COUNT / 2)
            pos.x = -260f;
        else
            pos.x = 260f;
        windowBase.localPosition = pos;
    }
}
