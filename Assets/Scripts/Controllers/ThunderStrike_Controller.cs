using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage,CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    void Update()
    {
        if (triggered || !targetStats)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;
        if(Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .3f);
            anim.transform.localRotation = Quaternion.identity;

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            Invoke("DamageAndSelfDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
