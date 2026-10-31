using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 720f;

    private Vector3 currentVelocity;
    private Rigidbody rb;
    private float moveX = 0f;
    private float moveZ = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Блокируем падения на спину
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // 1. Считываем ввод в обычном кадре
        GatherInput();

        // 2. Вращение через transform работает ВСЕГДА и не зависит от физических багов
        HandleRotation();
    }

    void FixedUpdate()
    {
        // 3. Двигаем персонажа
        HandleMovement();
    }

    void GatherInput()
    {
        moveX = 0f;
        moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
        }
    }

    void HandleMovement()
    {
        Vector3 targetDirection = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        // Плавный разгон скорости
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // ВАЖНО: Двигаем через linearVelocity! 
        // Это заставит Capsule Collider персонажа честно ударяться об Box Collider стен и не пускать его внутрь.
        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
    }

    void HandleRotation()
    {
        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 lookPoint = ray.GetPoint(rayDistance);
            Vector3 lookDirection = (lookPoint - transform.position);
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                // Прямой плавный поворот трансформа
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
