using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //private Animator animator;
    private GameObject weaponAxis;
    private Animator weaponAnimator;
    public Sprite weaponSprite;
    public Sprite weaponBasicAtkRange;
    public Sprite weaponSkillRange;

    public BoxCollider2D basicAtkHitBox;
    public BoxCollider2D skillHitBox;

    public float attackDmg;
    public float skillDmg;
    //public float attackCoolTime;
    public float skillCoolTime;

    //Animation Parameters
    public bool isInAction;
    public bool isAttacking;    //true가 되면 공격 애니메이션을 실행한다
    public bool isUsingSkill;       //true가 되면 스킬 애니메이션을 실행한다

    //protected virtual void ShowAttackAnim(Vector2 dir) { }
    //protected virtual void ShowSkillAnim(Vector2 dir) { }

    private void Awake()
    {
        weaponAnimator = gameObject.GetComponent<Animator>();
        //weaponBasicAtkRange = Resources.Load<Sprite>("Sprites/AttackRange/Sword/BasicAtkRange");
        //weaponSkillRange = Resources.Load<Sprite>("Sprites/AttackRange/Sword/SkillRange");
    }

    private void Start()
    {
        weaponAxis = gameObject.transform.parent.gameObject;
        isInAction = false;
        isAttacking = false;
        isUsingSkill = false;   
    }

    private void Update()
    {
        weaponAnimator.SetBool("isAttacking", isAttacking);
        weaponAnimator.SetBool("isUsingSkill", isUsingSkill);
   
    }

    private void OnTriggerBlock(GameObject _block)
    {
        _block.GetComponent<DestroyBlock>().destroyBlock();
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("blockIndex", _block.GetComponent<BlockData>().blockID);
        GameObject.Find("GameManager").GetComponent<GameManager>().sendBlockDestroy(jsonData);
    }

    private void OnTriggerPlayer(GameObject _player, float _angleZ)
    {
        Vector3 force;
        GameObject myPlayer = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        if (myPlayer.transform.localScale.x < 0)
        {
            Debug.Log(Mathf.Cos(_angleZ));
            force = new Vector3(Mathf.Cos(_angleZ) * -1f, Mathf.Sin(_angleZ)).normalized;
        }
        else
        {
            force = new Vector3(Mathf.Cos(_angleZ), Mathf.Sin(_angleZ)).normalized;
        }
        Vector2 hitDirection = new Vector2(force.x, force.y);
        Vector2 contactPoint = new Vector2(0, 0);
        
        int targetUserIndex = _player.gameObject.GetComponent<PlayerDetail>().Controller.Num;
        myPlayer.GetComponent<Player>().sendUserHit(targetUserIndex, hitDirection, Player.ACTION_TYPE.TYPE_ATTACK);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BasePlayer>() != null)
        {
            OnTriggerPlayer(collision.gameObject, weaponAxis.transform.localRotation.z);
        }

        if (collision.gameObject.GetComponent<DestroyBlock>() != null)
        {
            OnTriggerBlock(collision.gameObject);
        }
    }
}
