using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public enum State
    {
        Shot,
        Moving,
        Stationary,
        Par
    }

    [SerializeField] private LayerMask groundLayer;
    [Header("Physics Materials")]
    [SerializeField] private PhysicMaterial fairway;
    [SerializeField] private PhysicMaterial rough;
    [SerializeField] private PhysicMaterial hazard;
    [SerializeField] private PhysicMaterial green;
    
    public event Action<State, string> OnStateChanged;

    Rigidbody rigidBody;
    State state;
    Vector3 wind;

    public PhysicMaterial GetHazardMaterial() => hazard;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
    }

    private void Start()
    {
        state = State.Stationary; 
    }

    private void FixedUpdate()
    {
        if (state == State.Shot)
        {
            if (rigidBody.velocity.magnitude > 0f)
                SetStateWithoutNotifying(State.Moving);
        }

        if (state != State.Moving)
            return;

        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 3f);
        if (hit.collider != null)
        {
            SetPhysicsMaterial(hit.collider);
        }

        /*Collider[] colliders = Physics.OverlapSphere(transform.position, 0.7f, groundLayer);
        if (colliders.Length > 0)
        {
            colliders = colliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
            SetPhysicsMaterial(colliders[0]);
        }*/
        else
        {
            wind = WindManager.Instance.Wind;
            rigidBody.AddForce(wind, ForceMode.Acceleration); //for wind
        }

        if (rigidBody.velocity.magnitude <= 0.3f)
        {
            rigidBody.isKinematic = true;
            rigidBody.drag = 0f;
            UpdateState(hit.collider.tag);
            //UpdateState(colliders[0].tag);
        }
    }

    public void Shoot(Vector3 force)
    {
        rigidBody.isKinematic = false;
        rigidBody.AddForce(force, ForceMode.Impulse);
        SetStateWithoutNotifying(State.Shot);
    }

    private void SetPhysicsMaterial(Collider collider)
    {
        if (collider.tag == CourseArea.data.First(t => t.Value == "Green").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = green;
        }
        else if (collider.tag == CourseArea.data.First(t => t.Value == "Fairway").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = fairway;
        }
        else if (collider.tag == CourseArea.data.First(t => t.Value == "Rough").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = rough;
        }
        else if (collider.tag == CourseArea.data.First(t => t.Value == "Bunkers").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = hazard;
        }
        else if (collider.tag == CourseArea.data.First(t => t.Value == "Water").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = hazard;
        }
        else if (collider.tag == CourseArea.data.First(t => t.Value == "Default").Key)
        {
            transform.GetComponent<SphereCollider>().sharedMaterial = green;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidBody.drag += 0.06f;
        if (collision.collider.tag == CourseArea.data.First(t => t.Value == "Water").Key)
        {
            UpdateState(collision.gameObject.tag);
            rigidBody.velocity = Vector3.zero;
            rigidBody.isKinematic = true;
            rigidBody.drag = 0f;
            gameObject.SetActive(false);
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        float minDistance = 0;
        Transform waterPoints = CourseManager.Instance.ActiveCourse.waterPoints;
        Vector3 newPos = transform.position;
        for (int i=0; i< waterPoints.childCount; i++)
        {
            if (minDistance == 0 || (waterPoints.GetChild(i).position - transform.position).magnitude < minDistance)
            { 
                minDistance = (waterPoints.GetChild(i).position - transform.position).magnitude;
                newPos = waterPoints.GetChild(i).position;
            }
        }
        transform.GetComponent<SphereCollider>().sharedMaterial = rough;
        transform.position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tag 0")
        {
            UpdateState(other.gameObject.tag);
            rigidBody.velocity = Vector3.zero;
            gameObject.SetActive(false);
            //transform.GetComponent<SphereCollider>().enabled = false;
            WaitTillDestroyed(2000);
        }
    }
    private async void WaitTillDestroyed(int time)
    {
        await UniTask.Delay(time);
        gameObject.SetActive(false);
    }
    public float GetMass()
    {
        return rigidBody.mass;
    }

    private void UpdateState(string tag)
    {
        if (tag == "Tag 0")
            SetState(State.Par, "Pin");
        else if(tag == "Tag 8")
            SetState(State.Stationary, "Rough");
        else
            SetState(State.Stationary, CourseArea.data[tag]);

        /*if (tag == "Tag 1")
        {
            SetState(State.Stationary, CourseArea.Tee);
        }
        else if (tag == "Tag 2")
        {
            SetState(State.Stationary, CourseArea.Fairway);
        }
        else if (tag == "Tag 3")
        {
            SetState(State.Stationary, CourseArea.Rough);
        }
        else if (tag == "Tag 6")
        {
            SetState(State.Stationary, CourseArea.Bunkers);
        }
        else if (tag == "Tag 7")
        {
            SetState(State.Stationary, CourseArea.Water);
        }
        else if (tag == "Tag 5")
        {
            SetState(State.Stationary, CourseArea.Green);
        }
        else if (tag == "Tag 0")
        {
            SetState(State.Par, CourseArea.Pin);
        }
        else if (tag == "Tag 8")
        {
            SetState(State.Stationary, CourseArea.Rough);
        }*/
    }

    private void SetState(State state, string area)
    {
        SetStateWithoutNotifying(state);
        OnStateChanged?.Invoke(state, area);
    }

    private void SetStateWithoutNotifying(State state)
    {
        this.state = state;
    }

    public State GetState() => state;
}


