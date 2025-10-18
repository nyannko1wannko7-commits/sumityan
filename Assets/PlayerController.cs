using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpPower = 3f;
    [SerializeField] private Animator animator;
    [SerializeField] private float radioTaisoDuration = 3f;

    private CharacterController _characterController;
    private Transform _transform;
    private Vector3 _moveVelocity;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _radioTaiso;
    private InputAction _sprint;

    private bool isRadioTaisoPlaying = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
        _characterController = GetComponent<CharacterController>();
        _transform = transform;

        var input = GetComponent<PlayerInput>();
        _move = input.actions.FindAction("Move");
        _jump = input.actions.FindAction("Jump");
        _radioTaiso = input.actions.FindAction("RadioTaiso");
        _sprint = input.actions.FindAction("Sprint"); // Shiftキー用

        if (_move == null) Debug.LogError("❌ 'Move' アクションが見つかりません。");
        if (_jump == null) Debug.LogError("❌ 'Jump' アクションが見つかりません。");
        if (_radioTaiso == null) Debug.LogError("❌ 'RadioTaiso' アクションが見つかりません。");
        if (_sprint == null) Debug.LogError("❌ 'Sprint' アクションが見つかりません。");

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("❌ Animator が見つかりません。");
        }
    }

    private void Update()
    {
        if (_move == null || _jump == null || _radioTaiso == null || _sprint == null ||
            _characterController == null || _transform == null || animator == null)
            return;

        // ラジオ体操中は移動・ジャンプを無効化
        if (isRadioTaisoPlaying)
        {
            _moveVelocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0f);
            _characterController.Move(Vector3.zero);
            return;
        }

        var moveValue = _move.ReadValue<Vector2>();
        float currentSpeed = _sprint.IsPressed() ? sprintSpeed : walkSpeed;

        _moveVelocity.x = moveValue.x * currentSpeed;
        _moveVelocity.z = moveValue.y * currentSpeed;

        animator.SetFloat("MoveSpeed", new Vector3(_moveVelocity.x, 0, _moveVelocity.z).magnitude);

        if (moveValue != Vector2.zero)
        {
            var lookDirection = new Vector3(_moveVelocity.x, 0, _moveVelocity.z);
            _transform.LookAt(_transform.position + lookDirection);
        }

        // Enterキーでラジオ体操開始
        if (_radioTaiso.WasPressedThisFrame())
        {
            Debug.Log("ラジオ体操開始！（Enterキー）");
            animator.SetBool("RadioTaiso", true);
            isRadioTaisoPlaying = true;
            Invoke(nameof(EndRadioTaiso), radioTaisoDuration);
        }

        if (_characterController.isGrounded)
        {
            animator.SetBool("JumpTrigger", false);

            if (_jump.WasPressedThisFrame())
            {
                Debug.Log("ジャンプ！");
                _moveVelocity.y = jumpPower;
                animator.SetBool("JumpTrigger", true);
            }
        }
        else
        {
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        _characterController.Move(_moveVelocity * Time.deltaTime);
    }

    private void EndRadioTaiso()
    {
        animator.SetBool("RadioTaiso", false);
        isRadioTaisoPlaying = false;
        Debug.Log("ラジオ体操終了");
    }
}