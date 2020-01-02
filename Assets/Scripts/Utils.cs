using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 m_moveLimit = new Vector2(4.7f, 3.4f);

    /// <summary>
    /// 画面から出ないようにする処理
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3
        (
            Mathf.Clamp(position.x, -m_moveLimit.x, m_moveLimit.x),
            Mathf.Clamp(position.y, -m_moveLimit.y, m_moveLimit.y),
            0
        );
    }

    /// <summary>
    /// 指定した２つの位置から角度を計算して返す
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float GetAngle(Vector2 from, Vector2 to)
    {
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg;
    }

    // 指定された角度（ 0 ～ 360 ）をベクトルに変換して返す
    public static Vector3 GetDirection(float angle)
    {
        return new Vector3
        (
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );
    }
}
