using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnScript : MonoBehaviour {

	public int id;
	public int team;
	public int matrix_x;
	public int matrix_y;
    public bool canCapture = false;
    private Vector3 target;
    private bool move = false;
    private bool disable = false;
    private float speed = 12;
    private float grow_speed = 0.01F;
    private float distance;
    private float alpha = 1f;
	// Use this for initialization
	public void Init ()
	{
		
	}

    public void Update()
    {
        if(move)
        {
            if (Vector3.Distance(target, transform.position) > distance / 2)
            {
                if (transform.localScale.x < 1.3f)
                    transform.localScale += new Vector3(grow_speed, grow_speed, 0);
            }
            else if (transform.localScale.x > 1.0f)
                transform.localScale += new Vector3(-grow_speed, -grow_speed, 0);

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position == target)
            {
                transform.localScale = Vector3.one;
                move = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            }
        }
        if(disable)
        {
            print(alpha);
            alpha -= 0.04f;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
            if(alpha<=0)
            {
                disable = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void MoveTo(float x, float y)
    {
        target = new Vector3(x, y, -1f);
        distance = Vector3.Distance(target, transform.position);
        move = true;
    }

    public void Disable()
    {
        disable = true;
    }

}
