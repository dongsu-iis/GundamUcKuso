using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// 敵の出現位置の種類
public enum RESPAWN_TYPE
{
    UP, // 上
    RIGHT, // 右
    DOWN, // 下
    LEFT, // 左
    SIZEOF, // 敵の出現位置の数
}

public class Enemy : MonoBehaviour
{

    public Vector2 m_respawnPosInside; // 敵の出現位置（内側）
    public Vector2 m_respawnPosOutside; // 敵の出現位置（外側）
    public float m_speed; // 移動する速さ
    public int m_hpMax; // HP の最大値
    public int m_exp; // この敵を倒した時に獲得できる経験値
    public int m_damage; // この敵がプレイヤーに与えるダメージ

    public Item[] m_itemPrefabs; // アイテムのプレハブを管理する配列
    public float m_itemSpeedMin; // 生成するアイテムの移動の速さ（最小値）
    public float m_itemSpeedMax; // 生成するアイテムの移動の速さ（最大値）

    private int m_hp; // HP
    private Vector3 m_direction; // 進行方向

    public bool m_isFollow; // プレイヤーを追尾する場合 true

    public Explosion m_explosionPrefab; // 爆発エフェクトのプレハブ

    public AudioClip m_deathClip; // 敵を倒した時に再生する SE

    // 敵が生成された時に呼び出される関数
    private void Start()
    {
        // HP を初期化する
        m_hp = m_hpMax;
    }

    // 毎フレーム呼び出される関数
    private void Update()
    {
        // プレイヤーを追尾する場合
        if (m_isFollow)
        {
            // プレイヤーの現在位置へ向かうベクトルを作成する
            var angle = Utils.GetAngle(
                transform.localPosition,
                Player.m_instance.transform.localPosition);
            var direction = Utils.GetDirection(angle);

            // プレイヤーが存在する方向に移動する
            transform.localPosition += direction * m_speed;

            // プレイヤーが存在する方向を向く
            var angles = transform.localEulerAngles;
            angles.z = angle - 90;
            transform.localEulerAngles = angles;
            return;
        }

        // まっすぐ移動する
        transform.localPosition += m_direction * m_speed;
    }

    // 敵が出現する時に初期化する関数
    public void Init(RESPAWN_TYPE respawnType)
    {
        var pos = Vector3.zero;

        // 指定された出現位置の種類に応じて、
        // 出現位置と進行方向を決定する
        switch (respawnType)
        {
            // 出現位置が上の場合
            case RESPAWN_TYPE.UP:
                pos.x = Random.Range(
                    -m_respawnPosInside.x, m_respawnPosInside.x);
                pos.y = m_respawnPosOutside.y;
                m_direction = Vector2.down;
                break;

            // 出現位置が右の場合
            case RESPAWN_TYPE.RIGHT:
                pos.x = m_respawnPosOutside.x;
                pos.y = Random.Range(
                    -m_respawnPosInside.y, m_respawnPosInside.y);
                m_direction = Vector2.left;
                break;

            // 出現位置が下の場合
            case RESPAWN_TYPE.DOWN:
                pos.x = Random.Range(
                    -m_respawnPosInside.x, m_respawnPosInside.x);
                pos.y = -m_respawnPosOutside.y;
                m_direction = Vector2.up;
                break;

            // 出現位置が左の場合
            case RESPAWN_TYPE.LEFT:
                pos.x = -m_respawnPosOutside.x;
                pos.y = Random.Range(
                    -m_respawnPosInside.y, m_respawnPosInside.y);
                m_direction = Vector2.right;
                break;
        }

        // 位置を反映する
        transform.localPosition = pos;
    }

    // 他のオブジェクトと衝突した時に呼び出される関数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーと衝突した場合
        if (collision.name.Contains("Player"))
        {
            // プレイヤーにダメージを与える
            var player = collision.GetComponent<Player>();
            player.Damage(m_damage);
            return;
        }

        // 弾と衝突した場合
        if (collision.name.Contains("shot"))
        {
            // 弾が当たった場所に爆発エフェクトを生成する
            Instantiate(
                m_explosionPrefab,
                collision.transform.localPosition,
                Quaternion.identity);

            // 弾を削除する
            Destroy(collision.gameObject);

            // 敵の HP を減らす
            m_hp--;

            // 敵の HP がまだ残っている場合はここで処理を終える
            if (0 < m_hp) return;

            // 敵を倒した時の SE を再生する
            var audioSource = FindObjectOfType<AudioSource>();
            audioSource.PlayOneShot(m_deathClip);

            // 敵が死亡した場合はアイテムを散らばらせる
            var exp = m_exp;

            while (0 < exp)
            {
                // 生成可能なアイテムを配列で取得する
                var itemPrefabs = m_itemPrefabs.Where(c => c.m_exp <= exp).ToArray();

                // 生成可能なアイテムの配列から、生成するアイテムをランダムに決定する
                var itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

                // 敵の位置にアイテムを生成する
                var item = Instantiate(
                    itemPrefab, transform.localPosition, Quaternion.identity);

                // アイテムを初期化する
                item.Init(m_exp, m_itemSpeedMin, m_itemSpeedMax);

                // まだアイテムを生成できるかどうか計算する
                exp -= item.m_exp;
            }

            // 敵を削除する
            Destroy(gameObject);
        }
    }
}
