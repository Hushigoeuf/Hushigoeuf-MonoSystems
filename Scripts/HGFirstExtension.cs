using System;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.Utilities.Editor;
#endif

namespace Hushigoeuf
{
    /// <summary>
    /// Этот класс определяет компонент как первичное расширение.
    /// Первичное расширение может содержать дочерние и является началом "древа" расширений.
    ///
    /// К примеру, компонент Player может быть определен как первичный.
    /// </summary>
    public abstract class HGFirstExtension<TSecondExtension> : HGBaseExtension where TSecondExtension : HGBaseExtension
    {
        protected const int EDITOR_MIN_ORDER = HGBaseExtension.EDITOR_MIN_ORDER + 1;

        /// Сюда будут вкладываться все дочерние расширения
        [NonSerialized] public readonly List<TSecondExtension> Extensions = new List<TSecondExtension>();

        protected List<TSecondExtension> _result = new List<TSecondExtension>();

        public override void Initialization()
        {
            var initialized = _initialized;

            base.Initialization();

            if (!initialized)
                for (var i = 0; i < Extensions.Count; i++)
                    Extensions[i].Initialization();
        }

        /// <summary>
        /// Регистрирует новое дочерние расширение.
        /// </summary>
        public override void RegisterSecondExtension(HGBaseExtension target)
        {
            base.RegisterSecondExtension(target);

            if (target is TSecondExtension t)
                if (t.CompareFirstExtension(this))
                    if (!Extensions.Contains(t))
                        Extensions.Add(t);
        }

        #region GetExtensions

        /// <summary>
        /// Возвращает список дочерних расширений.
        /// </summary>
        public TSecondExtension[] GetExtensions(string eID = null)
        {
            _result.Clear();
            for (var i = 0; i < Extensions.Count; i++)
                if (eID == null || Extensions[i].ExtensionID == eID)
                    _result.Add(Extensions[i]);
            return _result.ToArray();
        }

        public T[] GetExtensions<T>(string eID = null) where T : TSecondExtension
        {
            var result = new List<T>();
            foreach (var t in GetExtensions(eID))
                if (t is T tt)
                    result.Add(tt);
            return result.ToArray();
        }

        #endregion

        #region GetExtension

        /// <summary>
        /// Возвращает дочернее расширение по индексу.
        /// </summary>
        public TSecondExtension GetExtension(int index)
        {
            if (index > 0 && index < Extensions.Count)
                return Extensions[index];
            return null;
        }

        public T GetExtension<T>(int index) where T : TSecondExtension
        {
            var result = GetExtension(index);
            if (result != null)
                return result as T;
            return null;
        }

        /// <summary>
        /// Возвращает дочернее расширение по ID.
        /// </summary>
        public TSecondExtension GetExtension(string eID = null)
        {
            if (eID != null)
                for (var i = 0; i < Extensions.Count; i++)
                    if (Extensions[i].ExtensionID == eID)
                        return Extensions[i];
                    else if (Extensions.Count != 0)
                        return Extensions[0];
            return null;
        }

        public T GetExtension<T>(string eID = null) where T : TSecondExtension
        {
            for (var i = 0; i < Extensions.Count; i++)
            {
                if (eID != null)
                    if (Extensions[i].ExtensionID != eID)
                        continue;
                if (Extensions[i] is T result) return result;
            }

            return null;
        }

        #endregion

        #region IsExtension

        /// <summary>
        /// Проверяет существование дочернего расширение по индексу.
        /// </summary>
        public bool IsExtension(int index, out TSecondExtension result)
        {
            result = GetExtension(index);
            return result != null;
        }

        public bool IsExtension(int index) => IsExtension(index, out _);

        public bool IsExtension<T>(int index, out T result) where T : TSecondExtension
        {
            result = GetExtension<T>(index);
            return result != null;
        }

        public bool IsExtension<T>(int index) where T : TSecondExtension => IsExtension<T>(index, out _);

        /// <summary>
        /// Проверяет существование дочернего расширения по ID.
        /// </summary>
        public bool IsExtension(string eID, out TSecondExtension result)
        {
            result = GetExtension(eID);
            return result != null;
        }

        public bool IsExtension(string eID) => IsExtension(eID, out _);

        public bool IsExtension<T>(string eID, out T result) where T : TSecondExtension
        {
            result = GetExtension<T>(eID);
            return result != null;
        }

        public bool IsExtension<T>(string eID) where T : TSecondExtension => IsExtension<T>(eID, out _);

        /// <summary>
        /// Проверяет существование дочернего расширения на основе заданного типа.
        /// </summary>
        public bool IsExtension<T>(out T result) where T : TSecondExtension
        {
            result = GetExtension<T>();
            return result != null;
        }

        public bool IsExtension<T>() where T : TSecondExtension => IsExtension<T>(out _);

        #endregion

#if UNITY_EDITOR && ODIN_INSPECTOR
        protected List<string> _editorMessageOnPlaying;

        /// <summary>
        /// Отображает в инспекторе сообщение с информацией.
        /// </summary>
        [PropertyOrder(EDITOR_MIN_ORDER)]
        [OnInspectorGUI]
        protected virtual void EditorOnInspectorMessage()
        {
            if (!Application.isPlaying) return;

            if (_editorMessageOnPlaying == null)
                _editorMessageOnPlaying = new List<string>();
            else _editorMessageOnPlaying.Clear();

            EditorOnChangeMessageOnPlaying();

            if (_editorMessageOnPlaying.Count != 0)
                SirenixEditorGUI.MessageBox(string.Join(HGEditor.SPACE, _editorMessageOnPlaying));
        }

        /// <summary>
        /// Здесь происходит изменение информации, которая отображается в инспекторе во время игры.
        /// </summary>
        protected virtual void EditorOnChangeMessageOnPlaying()
        {
            if (Extensions.Count == 0)
            {
                _editorMessageOnPlaying.Add("Extensions (0)");
            }
            else
            {
                _editorMessageOnPlaying.Add("Extensions (" + Extensions.Count + "):");
                for (var i = 0; i < Extensions.Count; i++)
                    _editorMessageOnPlaying.Add(HGEditor.TAB + "[" + i + "]: " + Extensions[i].GetType());
            }
        }
#endif
    }

    /// <summary>
    /// Можно переопределить этот класс, чтобы не задавать тип дочернего расширения.
    /// </summary>
    public abstract class HGFirstExtension : HGFirstExtension<HGBaseExtension>
    {
    }
}