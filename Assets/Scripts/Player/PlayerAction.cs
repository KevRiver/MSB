using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    //조준, 공격, 스킬 사용이 해당된다
    PlayerState playerState;

    Transform tr;
    GameObject aimAxis;
    GameObject weaponAxis;
    GameObject weapon;

    float aimAngle;

    // Start is called before the first frame update
    void Start()
    {
        playerState = gameObject.GetComponent<PlayerState>();
        tr = gameObject.GetComponent<Transform>();
        aimAxis = gameObject.GetComponent<PlayerObjects>().aimAxis;
        weaponAxis = gameObject.GetComponent<PlayerObjects>().weaponAxis;
        weapon = weaponAxis.transform.GetChild(0).gameObject;   //weaponAxis 의 자식에 있는 무기 오브젝트를 캐싱한다
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        //죽는 이펙트 나오고 몇 초 있다가 리스폰
    }

    public float Vec32Angle(Vector3 vector)
    {
        float _aimAngle;
        if (vector == Vector3.zero)
            return 0;
        else
        {
            if (tr.localScale.x < 0)
                _aimAngle = Mathf.Rad2Deg * Mathf.Atan2(vector.x, vector.y) + 90;
            else
                _aimAngle = -Mathf.Rad2Deg * Mathf.Atan2(vector.x, vector.y) + 90;

            return _aimAngle;
        }
    }

    public void Aim(Vector3 vector, string curRange)
    {
        if (vector == Vector3.zero)
        {
            aimAxis.transform.Find("BasicAtkRange").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            aimAxis.transform.Find("SkillRange").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            aimAxis.transform.localRotation = Quaternion.identity;
        }
        else
        {
            aimAngle = Vec32Angle(vector);
            aimAxis.transform.Find(curRange).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            aimAxis.transform.localRotation = Quaternion.Euler(0, 0, aimAngle); //aimAxis를 회전 
        }

        aimAngle = Vec32Angle(vector);
        aimAxis.transform.Find(curRange).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        aimAxis.transform.localRotation = Quaternion.Euler(0, 0, aimAngle); //aimAxis를 회전 
    }

	public void Attack(Vector3 vector)
	{
		// 조준한 방향으로 공격이 나감
		// 이미 공격을 하고 있거나 스킬을 사용중에는 다시 Attack 이 호출되지 않는

		if (gameObject.GetComponent<PlayerState>().isAttackable)
		{
			gameObject.GetComponent<PlayerState>().isAttackable = false;
			aimAngle = Vec32Angle(vector);

			//sendUserAttack(aimAngle);
			weaponAxis.transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
			if (aimAngle >= -90 && aimAngle <= 90)
			{
				weaponAxis.transform.localScale = new Vector3(1, 1, 1);
				weaponAxis.transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
			}
			else
			{
				weaponAxis.transform.localScale = new Vector3(-1, 1, 1);
				if (aimAngle > 0)
					weaponAxis.transform.localRotation = Quaternion.Euler(0, 0, -(180 - aimAngle));
				else
					weaponAxis.transform.localRotation = Quaternion.Euler(0, 0, -(180 + aimAngle));
			}
			if (gameObject.tag == "LocalPlayer")
				gameObject.GetComponent<PlayerNetwork>().sendUserAttack(vector);

			weapon.GetComponent<WeaponAnim>().PlayBasicAtkAnim();

			StartCoroutine(WaitForIt());
			StartCoroutine(CoolTime());
			//weaponAxis.transform.localRotation = Quaternion.identity;
		}
	}

	public void UseActiveSkill()
    {

    }

    public void UsePassiveSkill()
    {

    }

    public IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
        //weaponAxis.transform.localRotation = Quaternion.identity;
    }

    public IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<PlayerState>().isAttackable = true;
        //weaponAxis.transform.localScale = new Vector3(1, 1, 1);
        //weaponAxis.transform.localRotation = Quaternion.identity;
        //gameObject.GetComponent<PlayerState>().isAttackable = true;
    }
}
