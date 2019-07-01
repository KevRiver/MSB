using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : MonoBehaviour
{
    //무기의 각 스킬 애니메이션
    // Start is called before the first frame update

    public Animator animator;   //인스펙터에서 대입 했음
    public GameObject weaponAxis;

    void Start()
    {
        weaponAxis = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBasicAtkStart()
    {
        Debug.Log("Sword Collider Enabled");
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void OnBasicAtkExit()
    {
        Debug.Log("Sword Collider Disabled");
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        weaponAxis.transform.localScale = new Vector3(1, 1, 1);
        weaponAxis.transform.localRotation = Quaternion.identity;
    }

    public void PlayBasicAtkAnim()
    {
		Debug.Log("Attack -> PlayBasicAtkAnim");
		animator.SetTrigger("AttackTrigger");
    }

    public void PlayActiveSkillAnim()
    {

    }

    public void PlayPassiveSkillAnim()
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
        weaponAxis.transform.localScale = new Vector3(1, 1, 1);
        weaponAxis.transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<PlayerState>().isAttackable = true;
    }

}
