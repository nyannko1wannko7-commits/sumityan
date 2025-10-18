using UnityEngine;
 
public class CameraControll : MonoBehaviour
{
    [SerializeField] GameObject player;
 
    void Update()
    {
        // マウスの移動量を取得
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
 
        // X方向に一定量移動していれば横回転
        if (Mathf.Abs(mx) > 0.001f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(player.transform.position, Vector3.up, mx);
        }
 
        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(my) > 0.001f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(player.transform.position, transform.right, my);
        }
    }
}