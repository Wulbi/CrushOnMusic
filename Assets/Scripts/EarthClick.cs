using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class EarthClick : MonoBehaviour
{
    Camera cam;
    private Vector3 originalScale;

    void Awake() => cam = Camera.main;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 위 클릭이면 무시 (선택)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            Vector2 p = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(p, Vector2.zero);
            transform.DOKill();
            transform.localScale = originalScale;
            if (hit.collider != null && hit.transform == transform)
            {
                transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
}