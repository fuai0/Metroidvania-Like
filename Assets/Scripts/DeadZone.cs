using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<CharacterStats>() != null)
            collision.GetComponent<CharacterStats>().Kill();
        else
            Destroy(collision.gameObject);
    }
}
