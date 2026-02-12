using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public float fadeSpeed = 1f;//how fast the ghost fades away
    
    private SpriteRenderer sr;//reference to this ghosts SpriteRenders
    private Color color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color= sr.color;

    }

    // Update is called once per frame
    void Update()
    {
        //gradually reduce the ghost's alpha
        color.a -= fadeSpeed * Time.deltaTime;

        //apply the updated color
        sr.color = color;

        //when fully transparent, remove the gameObject(ghost)
        if(color.a <= 0)
        {
            Destroy(gameObject);
        }

       
    }

    //this function allows the player scripts to set the ghosts sprite and flip direction
    public void Setup(Sprite sprite, bool flipX)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = sprite;
        sr.flipX = flipX;

        color = sr.color;
    }

}
