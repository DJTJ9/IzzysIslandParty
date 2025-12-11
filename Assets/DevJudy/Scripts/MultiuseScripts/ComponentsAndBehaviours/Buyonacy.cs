using UnityEngine;
using Ditzelgames;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    //public properties
    [SerializeField] private float airDrag = 1;
    [SerializeField] private float waterDrag = 10;
    [SerializeField] private bool affectDirection = true;
    [SerializeField] private bool attachToSurface = false;
    [SerializeField] private Transform[] floatPoints;
    [SerializeField] private float gravityMultiplier = 3f;

    private bool pointUnderWater = false;

    //used components
    private Rigidbody rb;
    private WaveCreator waveCreator;

    //water line
    private float waterLine;
    private float newWaterLine = 0f;
    private Vector3[] waterLinePoints;

    //help Vectors
    private Vector3 smoothVectorRotation;
    private Vector3 targetUp;
    private Vector3 centerOffset;

    private Vector3 Center => transform.position + centerOffset;

    // Start is called before the first frame update
    void Awake()
    {
        //get components
        waveCreator = FindFirstObjectByType<WaveCreator>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        //compute center
        waterLinePoints = new Vector3[floatPoints.Length];

        for (int i = 0; i < floatPoints.Length; i++)
        {
            waterLinePoints[i] = floatPoints[i].position;
        }

        centerOffset = PhysicsHelper.GetCenter(waterLinePoints) - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pointUnderWater = false;

        CalculateWaterline();

        var waterLineDelta = newWaterLine - waterLine;
        waterLine = newWaterLine;

        //compute up vector
        targetUp = PhysicsHelper.GetNormal(waterLinePoints);

        //gravity
        var gravity = Physics.gravity;
        rb.linearDamping = airDrag;

        if (waterLine > Center.y)
        {
            rb.linearDamping = waterDrag;

            if (attachToSurface)
            {
                //attach to water surface
                rb.position = new Vector3(rb.position.x, waterLine - centerOffset.y, rb.position.z);
            }
            else
            {
                //go up
                gravity = affectDirection ? targetUp * (-Physics.gravity.y * gravityMultiplier) : -Physics.gravity * gravityMultiplier;
                transform.Translate(Vector3.up * (waterLineDelta * 0.9f));
            }
        }
        else
        {
            gravity = Physics.gravity * gravityMultiplier;
        }

        rb.AddForce(gravity * Mathf.Clamp(Mathf.Abs(waterLine - Center.y), 0, 1));

        //rotation
        if (pointUnderWater)
        {
            Debug.Log("under watter");
            //attach to water surface
            targetUp = Vector3.SmoothDamp(transform.up, targetUp, ref smoothVectorRotation, 0.2f);
            rb.rotation = Quaternion.FromToRotation(transform.up, targetUp) * rb.rotation;
        }
    }

    private void CalculateWaterline()
    {
        newWaterLine = 0f;
        //set WaterLinePoints and WaterLine
        for (int i = 0; i < floatPoints.Length; i++)
        {
            //height
            waterLinePoints[i] = floatPoints[i].position;
            waterLinePoints[i].y = waveCreator.GetHeight(floatPoints[i].position);

            newWaterLine += waterLinePoints[i].y / floatPoints.Length;

            if (waterLinePoints[i].y > floatPoints[i].position.y)
                pointUnderWater = true;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (floatPoints == null)
            return;

        for (int i = 0; i < floatPoints.Length; i++)
        {
            if (floatPoints[i] == null)
                continue;

            if (waveCreator != null)
            {
                //draw cube
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(waterLinePoints[i], Vector3.one * 0.3f);
            }

            //draw sphere
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(floatPoints[i].position, 0.1f);
        }

        //draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, waterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, waterLine, Center.z), targetUp * 1f);
        }
    }
}