using System;
using UnityEngine.Serialization;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable]
    public class InfluenceVolume
    {
        [SerializeField, FormerlySerializedAs("m_ShapeType")]
        Shape m_Shape = Shape.Box;
        [SerializeField, FormerlySerializedAs("m_BoxBaseOffset")]
        Vector3 m_Offset;
        [SerializeField, Obsolete("Kept only for compatibility. Use m_Offset instead")]
        Vector3 m_SphereBaseOffset;

        // Box
        [SerializeField]
        Vector3 m_BoxBaseSize = Vector3.one;
        [SerializeField, FormerlySerializedAs("m_BoxInfluencePositiveFade")]
        Vector3 m_BoxPositiveFade;
        [SerializeField, FormerlySerializedAs("m_BoxInfluenceNegativeFade")]
        Vector3 m_BoxNegativeFade;
        [SerializeField, FormerlySerializedAs("m_BoxInfluenceNormalPositiveFade")]
        Vector3 m_BoxNormalPositiveFade;
        [SerializeField, FormerlySerializedAs("m_BoxInfluenceNormalNegativeFade")]
        Vector3 m_BoxNormalNegativeFade;
        [SerializeField, FormerlySerializedAs("m_BoxPositiveFaceFade")]
        Vector3 m_BoxFacePositiveFade = Vector3.one;
        [SerializeField, FormerlySerializedAs("m_BoxNegativeFaceFade")]
        Vector3 m_BoxFaceNegativeFade = Vector3.one;

        //editor value that need to be saved for easy passing from simplified to advanced and vice et versa
        // /!\ must not be used outside editor code
        [SerializeField] private Vector3 editorAdvancedModeBlendDistancePositive;
        [SerializeField] private Vector3 editorAdvancedModeBlendDistanceNegative;
        [SerializeField] private float editorSimplifiedModeBlendDistance;
        [SerializeField] private Vector3 editorAdvancedModeBlendNormalDistancePositive;
        [SerializeField] private Vector3 editorAdvancedModeBlendNormalDistanceNegative;
        [SerializeField] private float editorSimplifiedModeBlendNormalDistance;
        [SerializeField] private bool editorAdvancedModeEnabled;

        // Sphere
        [SerializeField]
        float m_SphereBaseRadius = 1;
        [SerializeField, FormerlySerializedAs("m_SphereInfluenceFade")]
        float m_SphereFade;
        [SerializeField, FormerlySerializedAs("m_SphereInfluenceNormalFade")]
        float m_SphereNormalFade;

        /// <summary>Shape of this InfluenceVolume.</summary>
        public Shape shape { get { return m_Shape; } set { m_Shape = value; } }

        /// <summary>Offset of this influence volume to the component handling him.</summary>
        public Vector3 offset { get { return m_Offset; } set { m_Offset = value; } }

        public Vector3 boxBaseSize { get { return m_BoxBaseSize; } set { m_BoxBaseSize = value; } }
        public Vector3 boxPositiveFade { get { return m_BoxPositiveFade; } set { m_BoxPositiveFade = value; } }
        public Vector3 boxNegativeFade { get { return m_BoxNegativeFade; } set { m_BoxNegativeFade = value; } }
        public Vector3 boxNormalPositiveFade { get { return m_BoxNormalPositiveFade; } set { m_BoxNormalPositiveFade = value; } }
        public Vector3 boxNormalNegativeFade { get { return m_BoxNormalNegativeFade; } set { m_BoxNormalNegativeFade = value; } }
        public Vector3 boxFacePositiveFade { get { return m_BoxFacePositiveFade; } set { m_BoxFacePositiveFade = value; } }
        public Vector3 boxFaceNegativeFade { get { return m_BoxFaceNegativeFade; } set { m_BoxFaceNegativeFade = value; } }

        public Vector3 boxOffset { get { return (boxNegativeFade - boxPositiveFade) * 0.5f; } }
        public Vector3 boxSize { get { return -(boxPositiveFade + boxNegativeFade); } }
        public Vector3 boxNormalOffset { get { return (boxNormalNegativeFade - boxNormalPositiveFade) * 0.5f; } }
        public Vector3 boxNormalSize { get { return -(boxNormalPositiveFade + boxNormalNegativeFade); } }

        public float sphereBaseRadius { get { return m_SphereBaseRadius; } set { m_SphereBaseRadius = value; } }
        public float sphereFade { get { return m_SphereFade; } set { m_SphereFade = value; } }
        public float sphereNormalFade { get { return m_SphereNormalFade; } set { m_SphereNormalFade = value; } }

        public BoundingSphere GetBoundingSphereAt(Transform transform)
        {
            switch (shape)
            {
                default:
                case Shape.Sphere:
                    return new BoundingSphere(transform.TransformPoint(offset), sphereBaseRadius);
                case Shape.Box:
                {
                    var position = transform.TransformPoint(offset);
                    var radius = Mathf.Max(boxBaseSize.x, Mathf.Max(boxBaseSize.y, boxBaseSize.z));
                    return new BoundingSphere(position, radius);
                }
            }
        }

        public Bounds GetBoundsAt(Transform transform)
        {
            switch (shape)
            {
                default:
                case Shape.Sphere:
                    return new Bounds(transform.position, Vector3.one * sphereBaseRadius);
                case Shape.Box:
                {
                    var position = transform.TransformPoint(offset);
                    // TODO: Return a proper AABB based on influence box volume
                    return new Bounds(position, boxBaseSize);
                }
            }
        }

        public Vector3 GetWorldPosition(Transform transform)
        {
            return transform.TransformPoint(offset);
        }

        internal void MigrateOffsetSphere()
        {
            if (shape == Shape.Sphere)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                m_Offset = m_SphereBaseOffset;
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}
