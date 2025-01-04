using Sirenix.OdinValidator.Editor;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PawPhase : BossPhase
{
    [SerializeField] Vector3 leftPawStartPoint;
    [SerializeField] Vector3 rightPawStartPoint;
    public float maxXPos;
    public float attackCooldown;
    public float pawSpeed;
    public float pawDamage;
    public float pawReturnSpeed;
    public float homingFactor = 0f;
    public float hitBoxSize = 3f;
    [SerializeField] PawAttack leftPaw;
    [SerializeField] PawAttack rightPaw;
    private float attackTimer = 0f;
    private BoxCollider2D leftCollider;
    private BoxCollider2D rightCollider;
    private bool vulnerable = false;


    private void Awake()
    {
        leftCollider = this.AddComponent<BoxCollider2D>();
        leftCollider.offset = leftPawStartPoint;
        leftCollider.isTrigger = true;
        leftCollider.size = new Vector2(hitBoxSize, hitBoxSize);
        rightCollider = this.AddComponent<BoxCollider2D>();
        rightCollider.offset = rightPawStartPoint;
        rightCollider.isTrigger = true;
        rightCollider.size = new Vector2(hitBoxSize, hitBoxSize);

    }
    public override void ResetPhase()
    {
        base.ResetPhase();
        leftPaw.SetOrignPoint(leftPawStartPoint);
        leftPaw.transform.position = leftPawStartPoint;
        rightPaw.SetOrignPoint(rightPawStartPoint);
        rightPaw.transform.position = rightPawStartPoint;
        leftPaw.returnSpeed = pawReturnSpeed;
        rightPaw.returnSpeed = pawReturnSpeed;
        leftPaw.speed = pawSpeed;
        rightPaw.speed = -pawSpeed;
        leftPaw.maxXPositon = maxXPos;
        rightPaw.maxXPositon = -maxXPos;
        leftPaw.homingFactor = homingFactor;
        rightPaw.homingFactor = homingFactor;
        leftPaw.gameObject.SetActive(false);
        rightPaw.gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        ResetPhase();
        leftPaw.gameObject.SetActive(true);
        rightPaw.gameObject.SetActive(true);
    }
    public void Loop(float delta) 
    {
        if (leftPaw.GetReachedposition() || rightPaw.GetReachedposition())
        {
            attackTimer += delta;
            vulnerable = true;
            if (attackTimer > attackCooldown)
            {
                vulnerable = false;
                leftPaw.SetReachedposition(false);
                rightPaw.SetReachedposition(false);
            }
        }
        else
        {
            leftPaw.UpdatePositon(delta);
            rightPaw.UpdatePositon(delta);
        }

    }

    public void Update()
    {
        Loop(Time.deltaTime);
    }

    public override void Hurt(float damage)
    {
        if (vulnerable)
        {
            base.Hurt(damage);

        }
    }

        public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(leftPawStartPoint, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rightPawStartPoint, Vector3.one);
        Gizmos.color = new Color(0.25f,1,0);
        Gizmos.DrawWireCube(leftPawStartPoint + new Vector3(maxXPos,0,0), Vector3.one);
        Gizmos.color = new Color(0.25f, 0, 1);
        Gizmos.DrawWireCube(rightPawStartPoint + new Vector3(-maxXPos, 0, 0), Vector3.one);

    }
}
