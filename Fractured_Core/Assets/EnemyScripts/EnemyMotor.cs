using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private SpriteRenderer sprite;

    private bool isMoving;
    private Vector3 destination;

    //called by enemy brain
    public void MoveTo(Vector3 worldPos)
    {
        destination = worldPos;
        isMoving = true;
    }

    //stop movement
    public void Stop()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }

        Vector3 currentPos = transform.position;

        //keep z constant
        destination.z = currentPos.z;

        //direction toward target
        Vector3 direction = destination - currentPos;
        
        if(sprite != null && direction.x != 0)
        {
            //if moving left flip sprite
            //if moving right normal sprite

            sprite.flipX= direction.x < 0;
        }

        //stop if very close
        if(direction.sqrMagnitude < 0.01f)
        {
            isMoving = false;
            return;
        }

        //move forward toward player
        Vector3 step = direction.normalized * moveSpeed * Time.deltaTime;
        transform.position += step;

        transform.position += step;

        

    }
}
