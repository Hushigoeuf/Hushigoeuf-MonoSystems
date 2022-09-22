using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Hushigoeuf.MonoSystems
{
    [CustomEditor(typeof(HGSystemContainer), true)]
    public class HGSystemContainerEditor : OdinEditor
    {
        private HGSystemContainer Target => (HGSystemContainer) target;

        private Color MainGUIColor => Color.cyan;
        private Color EnabledGUIColor => Color.green;
        private Color DisabledGUIColor => Color.red;

        public override void OnInspectorGUI()
        {
            DrawSystemList();

            base.OnInspectorGUI();
        }

        private void DrawSystemList()
        {
            if (!Application.isPlaying) return;

            var defaultGUIColor = GUI.backgroundColor;

            GUI.backgroundColor = MainGUIColor;
            SirenixEditorGUI.BeginBox();
            {
                DrawSystemListTitle(Target.SystemList.ActivatedSystems.Count);
                foreach (var system in Target.SystemList.ActivatedSystems)
                    if (!system.IsDependentSystem)
                        DrawSystemListItem(system);
            }
            SirenixEditorGUI.EndBox();
            GUI.backgroundColor = defaultGUIColor;
        }

        private void DrawSystemListTitle(int count)
        {
            string title = "Systems (" + count + ")" + (count != 0 ? ":" : "");

            SirenixEditorGUI.Title(title, "", TextAlignment.Left, true, true);
        }

        private void DrawSystemListItem(HGMonoSystem item)
        {
            DrawSystemListToggle(item);
            foreach (var system in Target.SystemList.ActivatedSystems)
                if (system.DependentType == item.SystemType)
                    DrawSystemListItem(system);
        }

        private void DrawSystemListToggle(HGMonoSystem system)
        {
            bool enabled = system.SystemEnabled;

            GUI.backgroundColor = enabled ? EnabledGUIColor : DisabledGUIColor;
            SirenixEditorGUI.BeginBox();
            {
                var text = "";
                if (system.IsDependentSystem) text += system.DependentType + " => ";
                text += system.SystemType;
                if (!system.Initialized) text += " (not initialized)";

                GUILayout.Toggle(enabled, text);
            }
            SirenixEditorGUI.EndBox();
        }
    }
}