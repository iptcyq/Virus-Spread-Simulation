using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    // movement
    private Rigidbody2D rb;
    public float speed = 3;

    public Vector2 minPos;
    public Vector2 maxPos;

    private Vector3 nextPos;

    // all possible states units can be in
    public enum state { uninfected, infected, removed, vaccinated };
    public state currentState;

    [Space]
    //infections
    public bool vaccinated = false;
    public float infectionRadius = 0.6f;
    public float removedTime = 30f;

    public bool recovering = false;

    [Space]
    //depends on mask rate
    public float probabilityOfInfection = 0.5f; //where 1 is 100%

    //display
    private SpriteRenderer sr;
    public Color infectedColor;
    public Color removedColor; // recovered, died, etc is considered as "removed"
    
    [HideInInspector]
    public LineRenderer line; // circle for infection proximity

    // Start is called before the first frame update
    void Start()
    {
        // initialisation
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        nextPos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0);

        // draw infection proximity radius
        line = GetComponent<LineRenderer>();
        ChangeProximity();
    }

    // Update is called once per frame
    void Update()
    {
        // random movement
        if (transform.position == nextPos) // reached new position
        {
            // choose new random position
            nextPos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0);
        }
        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime); // move there

        // if removedTime set to zero by UnitController, no recovery
        if (removedTime <= 0.01f) 
        { 
            recovering = false; 
        } 
        else 
        { 
            recovering = true;
        }
        // start time before removal from simulation
        if (currentState == state.infected && recovering) 
        { 
            StartCoroutine("RemovedTime"); 
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(Physics2D.OverlapCircle(transform.position, infectionRadius, LayerMask.GetMask("Infected")));
        if (currentState == state.uninfected && Physics2D.OverlapCircle(transform.position, infectionRadius, LayerMask.GetMask("Infected"))) // if in range of infected
        {
            //Debug.Log((Random.Range(0f, 1f)).ToString() + probabilityOfInfection.ToString());
            if (Random.Range(0f, 1f) < probabilityOfInfection) // randomly see if infected every once in a while
            {
                currentState = state.infected;
                gameObject.layer = LayerMask.NameToLayer("Infected");
                sr.color = infectedColor;
            }
        }
    }

    public void ChangeProximity()
    {
        int segments = 20; // segments, number of sides a circle has hehe:)
        line.SetWidth(0.05f, 0.05f); // width of circle
        line.SetVertexCount(segments + 1);
        CreatePoints(segments);
    }
    private void CreatePoints(int segments) // used to calculate circle gizmo points
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * infectionRadius * 3; // change x coordinated by a sine curve
            y = Mathf.Cos (Mathf.Deg2Rad * angle) * infectionRadius * 3; // change y coordinated by a cosine curve

            line.SetPosition(i, new Vector3(x, y, 0)); // set new x and y to plot against

            angle += (360f / segments); // plot a new point for each segment
        }
    }

    private IEnumerator RemovedTime()
    {
        recovering = false; // flag for removedTime
        yield return new WaitForSeconds(removedTime);

        currentState = state.removed; // removed
        gameObject.layer = LayerMask.NameToLayer("Unit"); // remove from infection layer
        sr.color = removedColor; 
    }

    //debugging in game engine
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, infectionRadius);
    }
}
