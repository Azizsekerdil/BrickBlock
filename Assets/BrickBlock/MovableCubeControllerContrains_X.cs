using UnityEngine;
using DG.Tweening;

public class MovableCubeControllerContrains_X : MonoBehaviour
{
    private Renderer rend;
    private Rigidbody rb;
    private Vector3 offset;
    private bool isSelected = false;
    public float moveSpeed = 5f;
    private bool canMove = true;

    public GameObject shardPrefab;
    public int shardCount = 20;
    public float shardForce = 3f;

    public float moveDuration = 1.5f; // Hedefe hareket süresi

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        isSelected = true;
                        canMove = true;
                        rb.constraints = RigidbodyConstraints.None;
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                        rb.constraints = RigidbodyConstraints.FreezePositionZ;
                        rb.constraints = RigidbodyConstraints.FreezePositionY;

                        offset = transform.position - GetWorldPositionFromTouch(touch.position);
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved && isSelected && canMove)
            {
                Vector3 targetPosition = GetWorldPositionFromTouch(touch.position) + offset;
                rb.linearVelocity = (targetPosition - transform.position) * moveSpeed;

                rb.constraints = RigidbodyConstraints.FreezePositionZ;
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                isSelected = false;
                rb.linearVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private Vector3 GetWorldPositionFromTouch(Vector2 touchPosition)
    {
        Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(
            touchPosition.x, touchPosition.y, Camera.main.transform.position.y
        ));
        touchWorldPos.y = transform.position.y;
        return touchWorldPos;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("FixedCube"))
        {
            Renderer otherRenderer = other.gameObject.GetComponent<Renderer>();

            if (GetComponent<Renderer>().material.color == otherRenderer.material.color)
            {
                // 🚀 Rengine uygun objeyi bul ve hareket et
                GameObject targetCube = FindMatchingCube();
                if (targetCube != null)
                {
                    MoveToTarget(targetCube.transform);
                }
                else
                {
                    Debug.Log("Eşleşen renkli obje bulunamadı.");
                }
            }
            else
            {

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                canMove = false;
            }
        }
    }

    GameObject FindMatchingCube()
    {
        GameObject[] fixedCubes = GameObject.FindGameObjectsWithTag("FixedCube");

        GameObject nearestCube = null;
        float shortestDistance = Mathf.Infinity; // Başlangıçta sonsuz mesafe belirledik
        Vector3 currentPosition = transform.position; // Bulunduğu konum

        foreach (var cube in fixedCubes)
        {
            if (cube.GetComponent<Renderer>().material.color == rend.material.color)
            {
                float distance = Vector3.Distance(currentPosition, cube.transform.position);

                if (distance < shortestDistance) // Daha yakınını bulduysa
                {
                    shortestDistance = distance;
                    nearestCube = cube;
                }
            }
        }

        return nearestCube;
    }


    void MoveToTarget(Transform target)
    {
        transform.DOMove(target.position, moveDuration * 0.5f)
            .SetEase(Ease.InExpo) // Daha hızlı hareket için
            .OnComplete(() =>
            {
                CreateDominoEffect();
                Destroy(gameObject);
            });
    }


    void CreateDominoEffect()
    {
        Vector3 direction = Vector3.forward;

        for (int i = 0; i < shardCount; i++)
        {
            GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
            Renderer shardRenderer = shard.GetComponent<Renderer>();
            shardRenderer.material.color = GetComponent<Renderer>().material.color;

            Rigidbody shardRb = shard.GetComponent<Rigidbody>();
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            shardRb.AddForce(randomDirection * shardForce, ForceMode.Impulse);

            Destroy(shard, 2f); // Parçaların 2 saniye sonra kaybolmasını sağla
        }
    }

}
