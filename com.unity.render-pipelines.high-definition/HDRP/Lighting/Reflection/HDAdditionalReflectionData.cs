using UnityEngine.Serialization;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
    [RequireComponent(typeof(ReflectionProbe))]
    public class HDAdditionalReflectionData : HDProbe, ISerializationCallbackReceiver
    {
        const int currentVersion = 3;

        [SerializeField, FormerlySerializedAs("version")]
        int m_Version;

        ReflectionProbe m_LegacyProbe;
        ReflectionProbe legacyProbe { get { return m_LegacyProbe ?? (m_LegacyProbe = GetComponent<ReflectionProbe>()); } }

        //data only keeped for migration
        [SerializeField]
        Shape influenceShape;
        [SerializeField]
        float influenceSphereRadius = 3.0f;
        [SerializeField]
        float sphereReprojectionVolumeRadius = 1.0f;
        [SerializeField]
        bool useSeparateProjectionVolume = false;
        [SerializeField]
        Vector3 boxReprojectionVolumeSize = Vector3.one;
        [SerializeField]
        Vector3 boxReprojectionVolumeCenter = Vector3.zero;
        [SerializeField]
        float maxSearchDistance = 8.0f;
        [SerializeField]
        Texture previewCubemap;
        [SerializeField]
        Vector3 blendDistancePositive = Vector3.zero;
        [SerializeField]
        Vector3 blendDistanceNegative = Vector3.zero;
        [SerializeField]
        Vector3 blendNormalDistancePositive = Vector3.zero;
        [SerializeField]
        Vector3 blendNormalDistanceNegative = Vector3.zero;
        [SerializeField]
        Vector3 boxSideFadePositive = Vector3.one;
        [SerializeField]
        Vector3 boxSideFadeNegative = Vector3.one;

        //editor value that need to be saved for easy passing from simplified to advanced and vice et versa
        // /!\ must not be used outside editor code
        [SerializeField]
        Vector3 editorAdvancedModeBlendDistancePositive;
        [SerializeField]
        Vector3 editorAdvancedModeBlendDistanceNegative;
        [SerializeField]
        float editorSimplifiedModeBlendDistance;
        [SerializeField]
        Vector3 editorAdvancedModeBlendNormalDistancePositive;
        [SerializeField]
        Vector3 editorAdvancedModeBlendNormalDistanceNegative;
        [SerializeField]
        float editorSimplifiedModeBlendNormalDistance;
        [SerializeField]
        bool editorAdvancedModeEnabled;

        bool needMigrateToHDProbeChild = false;

        public Vector3 boxBlendCenterOffset { get { return (blendDistanceNegative - blendDistancePositive) * 0.5f; } }
        public Vector3 boxBlendSizeOffset { get { return -(blendDistancePositive + blendDistanceNegative); } }
        public Vector3 boxBlendNormalCenterOffset { get { return (blendNormalDistanceNegative - blendNormalDistancePositive) * 0.5f; } }
        public Vector3 boxBlendNormalSizeOffset { get { return -(blendNormalDistancePositive + blendNormalDistanceNegative); } }


        public float sphereBlendRadiusOffset { get { return -blendDistancePositive.x; } }
        public float sphereBlendNormalRadiusOffset { get { return -blendNormalDistancePositive.x; } }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (m_Version != currentVersion)
            {
                // Add here data migration code
                if (m_Version < 2)
                {
                    needMigrateToHDProbeChild = true;
                }
                else
                {
                    if (m_Version < 3)
                    {
                        MigrateToUseInfluanceVolume();
                    }
                    m_Version = currentVersion;
                }
            }
        }

        private void OnEnable()
        {
            if (needMigrateToHDProbeChild)
                MigrateToUseInfluanceVolume();
        }

        void MigrateToHDProbeChild()
        {
            mode = legacyProbe.mode;
            refreshMode = legacyProbe.refreshMode;
            m_Version = 2;
            OnAfterDeserialize();   //continue migrating if needed
        }

        void MigrateToUseInfluanceVolume()
        {
            influenceVolume.shape = influenceShape;
        }

        public override ReflectionProbeMode mode
        {
            set
            {
                base.mode = value;
                legacyProbe.mode = value; //ensure compatibility till we capture without the legacy component
            }
        }

        public override ReflectionProbeRefreshMode refreshMode
        {
            set
            {
                base.refreshMode = value;
                legacyProbe.refreshMode = value; //ensure compatibility till we capture without the legacy component
            }
        }
    }
}
