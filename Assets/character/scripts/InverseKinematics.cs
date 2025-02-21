using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class InverseKinematics : MonoBehaviour
{
    public int chain_length = 2;
    public Transform target;
    public Transform pole;

    [Header("Solver Parameters")]
    public int Iterations = 10;
    public float Delta = 0.001f;
    public float snap_back_strength = 1f;

    protected float[] bones_length;
    protected float complete_length;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Transform Root;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void Init()
    {
        //initial array
        bones = new Transform[chain_length + 1];
        positions = new Vector3[chain_length + 1];
        bones_length = new float[chain_length];
        StartDirectionSucc = new Vector3[chain_length + 1];
        StartRotationBone = new Quaternion[chain_length + 1];

        //find root
        Root = transform;
        for (var i = 0; i <= chain_length; i++)
        {
            if (Root == null)
                throw new UnityException("The chain value is longer than the ancestor chain!");
            Root = Root.parent;
        }

        //init target
        if (target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(target, GetPositionRootSpace(transform));
        }
        StartRotationTarget = GetRotationRootSpace(target);

        //init data
        var current = transform;
        complete_length = 0;
        for (var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            StartRotationBone[i] = GetRotationRootSpace(current);

            if (i == bones.Length - 1)
            {
                //leaf
                StartDirectionSucc[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
            }
            else
            {
                //mid bone
                StartDirectionSucc[i] = GetPositionRootSpace(bones[i + 1]) - GetPositionRootSpace(current);
                bones_length[i] = StartDirectionSucc[i].magnitude;
                complete_length += bones_length[i];
            }

            current = current.parent;
        }
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (bones_length.Length != chain_length)
            Init();

        //get position
        for (int i = 0; i < bones.Length; i++)
            positions[i] = GetPositionRootSpace(bones[i]);

        var targetPosition = GetPositionRootSpace(target);
        var targetRotation = GetRotationRootSpace(target);

        //1st is possible to reach?
        if ((targetPosition - GetPositionRootSpace(bones[0])).sqrMagnitude >= complete_length * complete_length)
        {
            //just stretch it
            var direction = (targetPosition - positions[0]).normalized;
            //set everything after root
            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + direction * bones_length[i - 1];
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + StartDirectionSucc[i], snap_back_strength);

            for (int iteration = 0; iteration < Iterations; iteration++)
            {
                //https://www.youtube.com/watch?v=UNoX65PRehA
                //back
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                        positions[i] = targetPosition; //set it to target
                    else
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bones_length[i]; //set in line on distance
                }

                //forward
                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bones_length[i - 1];

                //close enough?
                if ((positions[positions.Length - 1] - targetPosition).sqrMagnitude < Delta * Delta)
                    break;
            }
        }

        //move towards pole
        if (pole != null)
        {
            var polePosition = GetPositionRootSpace(pole);
            for (int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                SetRotationRootSpace(bones[i], Quaternion.Inverse(targetRotation) * StartRotationTarget * Quaternion.Inverse(StartRotationBone[i]));
            else
                SetRotationRootSpace(bones[i], Quaternion.FromToRotation(StartDirectionSucc[i], positions[i + 1] - positions[i]) * Quaternion.Inverse(StartRotationBone[i]));
            SetPositionRootSpace(bones[i], positions[i]);
        }
        //directly set rotation of end of chain bone maybe...

    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (Root == null)
        {
            Debug.Log("root is null");
            return current.position;
        }
        else
            return Quaternion.Inverse(Root.rotation) * (current.position - Root.position);
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        //inverse(after) * before => rot: before -> after
        if (Root == null)
            return current.rotation;
        else
            return Quaternion.Inverse(current.rotation) * Root.rotation;
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (Root == null)
            current.position = position;
        else
            current.position = Root.rotation * position + Root.position;
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (Root == null)
            current.rotation = rotation;
        else
            current.rotation = Root.rotation * rotation;
    }

    /*
    private void OnDrawGizmos()
    {

        var current = this.transform;
        for(int i = 0; i < chain_length && current != null  && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;

            Handles.matrix = Matrix4x4.TRS(
                current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), 
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));

            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
    */
}
