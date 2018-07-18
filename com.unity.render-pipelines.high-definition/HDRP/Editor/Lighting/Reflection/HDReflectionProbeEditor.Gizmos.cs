using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    partial class HDReflectionProbeEditor
    {
        static Mesh sphere;
        static Material material;

        [DrawGizmo(GizmoType.Active)]
        static void RenderGizmo(ReflectionProbe reflectionProbe, GizmoType gizmoType)
        {
            var e = GetEditorFor(reflectionProbe);
            if (e == null || !e.sceneViewEditing)
                return;

            var reflectionData = reflectionProbe.GetComponent<HDAdditionalReflectionData>();

            switch (EditMode.editMode)
            {
                // Influence editing
                case EditMode.SceneViewEditMode.ReflectionProbeBox:
                    Gizmos_Influence(reflectionProbe, reflectionData, e, true);
                    break;
                // Influence fade editing
                case EditMode.SceneViewEditMode.GridBox:
                    Gizmos_InfluenceFade(reflectionProbe, reflectionData, e, InfluenceType.Standard, true);
                    break;
                // Influence normal fade editing
                case EditMode.SceneViewEditMode.Collider:
                    Gizmos_InfluenceFade(reflectionProbe, reflectionData, e, InfluenceType.Normal, true);
                    break;
            }
        }

        [DrawGizmo(GizmoType.Selected)]
        static void DrawSelectedGizmo(ReflectionProbe reflectionProbe, GizmoType gizmoType)
        {
            var e = GetEditorFor(reflectionProbe);
            if (e == null)
                return;

            var reflectionData = reflectionProbe.GetComponent<HDAdditionalReflectionData>();
            Gizmos_CapturePoint(reflectionProbe, reflectionData, e);

            if (!e.sceneViewEditing)
                return;

            //Gizmos_Influence(reflectionProbe, reflectionData, e, false);
            //Gizmos_InfluenceFade(reflectionProbe, reflectionData, null, InfluenceType.Standard, false);
            //Gizmos_InfluenceFade(reflectionProbe, reflectionData, null, InfluenceType.Normal, false);

            //[TODO:] DrawInfluence here

            DrawVerticalRay(reflectionProbe.transform);
        }

        static void DrawVerticalRay(Transform transform)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Handles.color = Color.green;
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawLine(transform.position - Vector3.up * 0.5f, hit.point);
                Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);

                Handles.color = Color.red;
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                Handles.DrawLine(transform.position, hit.point);
                Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);
            }
        }

        static void Gizmos_CapturePoint(ReflectionProbe p, HDAdditionalReflectionData a, HDReflectionProbeEditor e)
        {
            if(sphere == null)
            {
                sphere = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
            }
            if(material == null)
            {
                material = new Material(Shader.Find("Debug/ReflectionProbePreview"));
            }
            material.SetTexture("_Cubemap", p.texture);
            material.SetPass(0);
            Graphics.DrawMeshNow(sphere, Matrix4x4.TRS(p.transform.position, Quaternion.identity, Vector3.one));
        }
    }
}
