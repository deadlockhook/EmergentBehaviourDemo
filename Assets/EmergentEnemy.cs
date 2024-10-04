using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EmergentEnemy : MonoBehaviour
{
    public Transform target;

    private Vector3 randomDestination;
    private float directionChangeCooldown = 0f;

    private static float gridSize = 5f;
    private static Dictionary<Vector2, List<EmergentEnemy>> grid = new Dictionary<Vector2, List<EmergentEnemy>>();

    private Vector2 currentGridCell;

    private void Start() 
    {
        UpdateGridCell();
    }
    void Update()
    {
        Vector2 newGridCell = GetGridPosition(transform.position);
        if (newGridCell != currentGridCell)
        {
            UpdateGridCell();
            currentGridCell = newGridCell;
        }

        if (Vector3.Distance(transform.position, target.position) <= 10f)
        {
            Vector3 moveDirection = (target.position - transform.position).normalized;
           
            if (Vector3.Distance(transform.position, target.position) < 1.5f)
                moveDirection += (transform.position - target.position).normalized;

            transform.position += moveDirection.normalized * 5f * Time.deltaTime;
        }
        else
        {
            directionChangeCooldown += Time.deltaTime;
            if (directionChangeCooldown >= 2f)
            {
                randomDestination = Random.insideUnitSphere * 5f;
                randomDestination += transform.position;
                randomDestination.y = transform.position.y;

                directionChangeCooldown = 0f;
            }

            Vector3 moveDirection = (randomDestination - transform.position).normalized;
            transform.position += moveDirection * 2f * Time.deltaTime;
        }

        List<EmergentEnemy> nearbyEntities = GetNearbyEntities();

        foreach (EmergentEnemy otherEntity in nearbyEntities)
        {
            if (otherEntity == this) continue;

            if (Vector3.Distance(transform.position, otherEntity.transform.position) < 2f)
            {
                Vector3 separation = (transform.position - otherEntity.transform.position).normalized;
                transform.position += separation * 2f * Time.deltaTime;
            }
        }
    }

    Vector2 GetGridPosition(Vector3 position)
    {
        return new Vector2(Mathf.Floor(position.x / gridSize), Mathf.Floor(position.z / gridSize));
    }
    void UpdateGridCell()
    {
        if (grid.ContainsKey(currentGridCell))
        {
            grid[currentGridCell].Remove(this);
        }

        currentGridCell = GetGridPosition(transform.position);
        if (!grid.ContainsKey(currentGridCell))
        {
            grid[currentGridCell] = new List<EmergentEnemy>();
        }

        grid[currentGridCell].Add(this);
    }

    List<EmergentEnemy> GetNearbyEntities()
    {
        List<EmergentEnemy> nearbyEntities = new List<EmergentEnemy>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2 neighborCell = currentGridCell + new Vector2(x, z);
                if (grid.ContainsKey(neighborCell))
                {
                    nearbyEntities.AddRange(grid[neighborCell]);
                }
            }
        }

        return nearbyEntities;
    }

    /*
    public Transform chaseTarget;

    private Vector3 roamDestination;
    private float changeDirectionTimer = 0f;
    void Update()
    {
        if (Vector3.Distance(transform.position, chaseTarget.position) <= 10f)
        {
            Vector3 moveDirection = (chaseTarget.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, chaseTarget.position);

            if (distanceToTarget < 1.5f)
            {
                Vector3 repulsion = (transform.position - chaseTarget.position).normalized;
                moveDirection += repulsion;
            }

            transform.position += moveDirection.normalized * 5f * Time.deltaTime;
        }
        else
        {
            changeDirectionTimer += Time.deltaTime;
            if (changeDirectionTimer >= 2f)
            {
                roamDestination = Random.insideUnitSphere * 5f;
                roamDestination += transform.position;
                roamDestination.y = transform.position.y;

                changeDirectionTimer = 0f;
            }

            Vector3 moveDirection = (roamDestination - transform.position).normalized;
            transform.position += moveDirection * 2f * Time.deltaTime;
        }

        EmergentEnemy[] followers = FindObjectsOfType<EmergentEnemy>();

        foreach (EmergentEnemy otherFollower in followers)
        {
            if (otherFollower == this) continue;

            float distance = Vector3.Distance(transform.position, otherFollower.transform.position);

            if (distance < 2f)
            {
                Vector3 repulsion = (transform.position - otherFollower.transform.position).normalized;
                transform.position += repulsion * 2f * Time.deltaTime;
            }
        }
    }*/
}