using UnityEngine;
using UnityEngine.UI;

// 情報表示用の UI を制御するコンポーネント
public class Hub : MonoBehaviour
{
    public Image m_hpGauge; // HP ゲージ
    public Image m_expGauge; // 経験値ゲージ
    public Text m_levelText;// レベルのテキスト
    public GameObject m_gameOverText; // ゲームオーバーのテキスト

    // 毎フレーム呼び出される関数
    private void Update()
    {
        // プレイヤーを取得する
        var player = Player.m_instance;

        // HP のゲージの表示を更新する
        var hp = player.m_hp;
        var hpMax = player.m_hpMax;
        var now_hp = (float) hp / hpMax;
        m_hpGauge.fillAmount = now_hp;

        // 経験値のゲージの表示を更新する
        var exp = player.m_exp;
        var prevNeedExp = player.m_prevNeedExp;
        var needExp = player.m_needExp;
        var now_exp = (float)(exp - prevNeedExp) / (needExp - prevNeedExp);
        m_expGauge.fillAmount = now_exp;


        // レベルのテキストの表示を更新する
        m_levelText.text = player.m_level.ToString();

        // プレイヤーが非表示ならゲームオーバーと表示する
        m_gameOverText.SetActive(!player.gameObject.activeSelf);
    }
}
