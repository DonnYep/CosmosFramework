using UnityEngine;
using Cosmos;
[RequireComponent(typeof(EntityAnimator))]
public class EntityPlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4;
    [SerializeField] float rotSpeed = 8;
    EntityAnimator entityAnimator;
    EntityAblilty entityAblilty;
    bool onAttack;
    bool onAttackAnim;
    private void Start()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        entityAnimator.OnAttackOff += OnAttackOff;
        entityAnimator.OnAttackAnim += OnAttackAnim; ;
        entityAblilty = GetComponent<EntityAblilty>();
    }

    void Update()
    {
        var h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
        var v = CosmosEntry.InputManager.GetAxis(InputAxisType._Vertical);
        bool attackBtnDown = CosmosEntry.InputManager.GetButtonDown(InputButtonType._MouseLeft);
        if (attackBtnDown)
        {
            if (!onAttack)
            {
                entityAnimator.Attack();
                onAttack = true;
            }
        }
        if (onAttack)
            return;
        var inputDir = new Vector3(h, 0, v);
        if (inputDir != Vector3.zero)
        {
            var inputNormalized = inputDir.normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputNormalized), rotSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, transform.position + inputNormalized, Time.deltaTime * moveSpeed);
            entityAnimator.Move();
        }
        else
        {
            entityAnimator.Idle();
        }
    }
    private void OnAttackOff()
    {
        onAttack = false;
        onAttackAnim = false;
    }
    private void OnAttackAnim()
    {
        if (!onAttackAnim)
        {
            entityAblilty.Attack();
            onAttackAnim = true;
        }
    }
}
