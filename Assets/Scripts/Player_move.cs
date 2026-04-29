using UnityEngine;
using UnityEngine.InputSystem; // ← 이게 새 방식의 핵심!

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        // 옛날: Input.GetAxis("Horizontal")
        // 새방식: Keyboard.current 로 직접 키 확인!

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.dKey.isPressed) x = -1f;  // D키 → 오른쪽
        if (Keyboard.current.aKey.isPressed) x = 1f; // A키 → 왼쪽
        if (Keyboard.current.wKey.isPressed) z = -1f;  // W키 → 앞
        if (Keyboard.current.sKey.isPressed) z = 1f; // S키 → 뒤

        Vector3 movement = new Vector3(x, 0, z) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}