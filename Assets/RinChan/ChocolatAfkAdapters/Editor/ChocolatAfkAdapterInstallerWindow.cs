#if UNITY_EDITOR
using RinChan.AfkMotionPatcher;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace RinChan.ChocolatAfkAdapters.Editor
{
    public sealed class ChocolatAfkAdapterInstallerWindow : EditorWindow
    {
        private const string ChocolatIntroPath = "Assets/Amatousagi/Chocolat/Animation/AFK/Chocolat_AFK_Intro_VRSuya.anim";
        private const string ChocolatLoopPath = "Assets/Amatousagi/Chocolat/Animation/AFK/Chocolat_AFK_Loop_VRSuya.anim";
        private const string ChocolatOutroPath = "Assets/Amatousagi/Chocolat/Animation/AFK/Chocolat_AFK_Outro_VRSuya.anim";

        private const string PlumIntroPath = "Assets/Amatousagi/Plum/Animation/AFK/Plum_AFK_Intro_VRSuya.anim";
        private const string PlumLoopPath = "Assets/Amatousagi/Plum/Animation/AFK/Plum_AFK_Loop_VRSuya.anim";
        private const string PlumOutroPath = "Assets/Amatousagi/Plum/Animation/AFK/Plum_AFK_Outro_VRSuya.anim";

        private GameObject avatar;
        private TargetPreset targetPreset = TargetPreset.Plum;

        private enum TargetPreset
        {
            Plum = 0,
        }

        [MenuItem("GameObject/RinChan/Add Chocolat AFK Adapter...", false, 30)]
        public static void OpenFromHierarchy(MenuCommand command)
        {
            var window = GetWindow<ChocolatAfkAdapterInstallerWindow>(true, "Chocolat AFK Adapter");
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<VRCAvatarDescriptor>() != null)
            {
                window.avatar = Selection.activeGameObject.GetComponentInParent<VRCAvatarDescriptor>().gameObject;
            }
            window.minSize = new Vector2(440, 170);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Chocolat AFK Adapter", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Adds a configured AfkMotionPatch component. The core AFK Motion Patcher package must be installed separately.", MessageType.Info);

            avatar = (GameObject)EditorGUILayout.ObjectField("Avatar", avatar, typeof(GameObject), true);
            targetPreset = (TargetPreset)EditorGUILayout.EnumPopup("Target", targetPreset);

            using (new EditorGUI.DisabledScope(!CanInstall()))
            {
                if (GUILayout.Button("Add Chocolat AFK Adapter"))
                {
                    Install();
                }
            }
        }

        private bool CanInstall()
        {
            return avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() != null;
        }

        private void Install()
        {
            if (!LoadStandardChocolatClips(out var chocolatIntro, out var chocolatLoop, out var chocolatOutro)) return;
            if (!LoadTargetClips(targetPreset, out var targetIntro, out var targetLoop, out var targetOutro)) return;

            Undo.RegisterFullObjectHierarchyUndo(avatar, "Add Chocolat AFK Adapter");

            var holder = new GameObject("ChocolatAFKAdapter_" + targetPreset);
            holder.transform.SetParent(avatar.transform, false);
            var patch = holder.AddComponent<AfkMotionPatch>();

            patch.targetIntroMotion = targetIntro;
            patch.targetLoopMotion = targetLoop;
            patch.targetOutroMotion = targetOutro;
            patch.replacementIntroSource = chocolatIntro;
            patch.replacementLoopSource = chocolatLoop;
            patch.replacementOutroSource = chocolatOutro;
            patch.dropMissingBlendShapes = true;
            patch.patchActionLayer = true;
            patch.failOnMissingSource = true;

            ApplyTargetRemaps(targetPreset, patch);

            Selection.activeGameObject = holder;
            EditorGUIUtility.PingObject(holder);
            Close();
        }

        private static bool LoadStandardChocolatClips(out AnimationClip intro, out AnimationClip loop, out AnimationClip outro)
        {
            intro = AssetDatabase.LoadAssetAtPath<AnimationClip>(ChocolatIntroPath);
            loop = AssetDatabase.LoadAssetAtPath<AnimationClip>(ChocolatLoopPath);
            outro = AssetDatabase.LoadAssetAtPath<AnimationClip>(ChocolatOutroPath);
            if (intro != null && loop != null && outro != null) return true;

            EditorUtility.DisplayDialog(
                "Chocolat AFK Adapter",
                "Standard Chocolat VRSuya AFK clips were not found. Import your Chocolat avatar package first.",
                "OK");
            return false;
        }

        private static bool LoadTargetClips(TargetPreset target, out AnimationClip intro, out AnimationClip loop, out AnimationClip outro)
        {
            switch (target)
            {
                case TargetPreset.Plum:
                    intro = AssetDatabase.LoadAssetAtPath<AnimationClip>(PlumIntroPath);
                    loop = AssetDatabase.LoadAssetAtPath<AnimationClip>(PlumLoopPath);
                    outro = AssetDatabase.LoadAssetAtPath<AnimationClip>(PlumOutroPath);
                    break;
                default:
                    intro = null;
                    loop = null;
                    outro = null;
                    break;
            }

            if (intro != null && loop != null && outro != null) return true;

            EditorUtility.DisplayDialog(
                "Chocolat AFK Adapter",
                $"Target AFK clips for {target} were not found. Import the target avatar package first.",
                "OK");
            return false;
        }

        private static void ApplyTargetRemaps(TargetPreset target, AfkMotionPatch patch)
        {
            switch (target)
            {
                case TargetPreset.Plum:
                    patch.rendererPathRemaps.Add(new AfkMotionPatch.RendererPathRemap { fromPath = "Body", toPath = "Body" });
                    patch.blendShapeNameRemaps.Add(new AfkMotionPatch.BlendShapeNameRemap { fromName = "eye_nagomi", toName = "eye_nagomi_1" });
                    patch.blendShapeNameRemaps.Add(new AfkMotionPatch.BlendShapeNameRemap { fromName = "eye_nagomi_R", toName = "eye_nagomi_1_R" });
                    patch.blendShapeNameRemaps.Add(new AfkMotionPatch.BlendShapeNameRemap { fromName = "jitome_1", toName = "eye_nagomi_2" });
                    break;
            }
        }
    }
}
#endif
