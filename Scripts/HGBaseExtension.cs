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
    /// Этот класс определяет компонент как "контейнер" расширений.
    /// Такой "контейнер" может содержать расширения, которые дополняют функциональность.
    /// 
    /// К примеру, компонент Player может быть определен как контейнер и
    /// дополняться функционалом, например, движение персонажа можно внести в отдельное расширение.
    /// </summary>
    public abstract class HGBaseExtension : HGMonoBehaviour
    {
        protected const int EDITOR_MIN_ORDER = HGMonoBehaviour.EDITOR_MIN_ORDER + 1;

        /// Сюда будут вкладываться все зарегистрированные расширения
        protected Dictionary<Type, HGExtension> AuthorizedExtensions = new Dictionary<Type, HGExtension>();

        /// Сюда будут вкладываться все расширения, которые имеют связь с другими
        protected Dictionary<Type, List<HGExtension>> LinkedExtensions = new Dictionary<Type, List<HGExtension>>();

        protected bool _initialized;
        protected Transform _transform;

        public bool Initialized => _initialized;

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void Update()
        {
            var dt = Time.deltaTime;
            OnUpdate(dt);
            foreach (var e in AuthorizedExtensions.Values)
                if (e.ExtensionEnabled)
                    e.OnUpdate(dt);
        }

        protected virtual void FixedUpdate()
        {
            var fdt = Time.fixedDeltaTime;
            OnFixedUpdate(fdt);
            foreach (var e in AuthorizedExtensions.Values)
                if (e.ExtensionEnabled)
                    e.OnFixedUpdate(fdt);
        }

        protected virtual void LateUpdate()
        {
            var dt = Time.deltaTime;
            OnLateUpdate(dt);
            foreach (var e in AuthorizedExtensions.Values)
                if (e.ExtensionEnabled)
                    e.OnLateUpdate(dt);
        }

        /// <summary>
        /// Инициализирует расширение.
        /// Инициализация происходит по порядку, от родителя вниз по лестнице.
        /// </summary>
        protected virtual void Initialization()
        {
            if (_initialized) return;
            _initialized = true;
            OnInitialization();
            foreach (var t in AuthorizedExtensions.Keys)
                Initialization(t);
        }

        public virtual void Initialization(Type authorizedType)
        {
            if (!_initialized) return;
            if (!AuthorizedExtensions.ContainsKey(authorizedType)) return;
            AuthorizedExtensions[authorizedType].Initialization();
            if (LinkedExtensions.ContainsKey(authorizedType))
                foreach (var linkedExtension in LinkedExtensions[authorizedType])
                    linkedExtension.Initialization();
        }

        public virtual void Initialization(HGExtension authorizedExtension)
        {
            Initialization(authorizedExtension.GetType());
        }

        public virtual bool ContainsExtension(HGExtension authorizedExtension) =>
            AuthorizedExtensions.ContainsKey(authorizedExtension.GetType());

        public virtual void AddExtension(HGExtension authorizedExtension)
        {
            var t = authorizedExtension.GetType();
            if (!AuthorizedExtensions.ContainsKey(t))
            {
                AuthorizedExtensions.Add(t, authorizedExtension);
                authorizedExtension.ExtensionAuthorized = true;
            }

            if (LinkedExtensions.ContainsKey(t))
                foreach (var linkedTarget in LinkedExtensions[t])
                    AddExtension(linkedTarget);
        }

        public virtual void AddLinkedExtension(Type linkedType, HGExtension authorizedExtension)
        {
            if (!LinkedExtensions.ContainsKey(linkedType))
                LinkedExtensions.Add(linkedType, new List<HGExtension>());

            if (!LinkedExtensions[linkedType].Contains(authorizedExtension))
                LinkedExtensions[linkedType].Add(authorizedExtension);
            if (AuthorizedExtensions.ContainsKey(linkedType))
                AddExtension(authorizedExtension);
        }

        public virtual void RemoveExtension(HGExtension authorizedExtension)
        {
            var t = authorizedExtension.GetType();
            if (LinkedExtensions.ContainsKey(t))
                foreach (var linkedTarget in LinkedExtensions[t])
                    RemoveExtension(linkedTarget);

            if (AuthorizedExtensions.ContainsKey(t))
            {
                AuthorizedExtensions.Remove(t);
                authorizedExtension.ExtensionAuthorized = false;
            }
        }

        public virtual void RemoveLinkedExtension(Type linkedType, HGExtension authorizedExtension)
        {
            RemoveExtension(authorizedExtension);

            if (LinkedExtensions.ContainsKey(linkedType))
                if (LinkedExtensions[linkedType].Contains(authorizedExtension))
                    LinkedExtensions[linkedType].Remove(authorizedExtension);
        }

        /// <summary>
        /// Возвращает расширение из зарегистрированного списка.
        /// </summary>
        public HGExtension GetExtension(Type extensionType)
        {
            if (AuthorizedExtensions.ContainsKey(extensionType))
                return AuthorizedExtensions[extensionType];
            return null;
        }

        public T GetExtension<T>() where T : HGExtension => GetExtension(typeof(T)) as T;

        /// <summary>
        /// Проверяет существование расширения в зарегистрированном списке.
        /// </summary>
        public bool IsExtension(Type extensionType, out HGExtension _)
        {
            _ = GetExtension(extensionType);
            return _ != null;
        }

        public bool IsExtension<T>(out T _) where T : HGExtension
        {
            _ = GetExtension<T>();
            return _ != null;
        }

        protected virtual void OnInitialization()
        {
        }

        protected virtual void OnUpdate(float dt)
        {
        }

        protected virtual void OnFixedUpdate(float fdt)
        {
        }

        protected virtual void OnLateUpdate(float dt)
        {
        }

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
            if (AuthorizedExtensions.Count == 0)
            {
                _editorMessageOnPlaying.Add(" Extensions (0)");
            }
            else
            {
                _editorMessageOnPlaying.Add("Extensions (" + AuthorizedExtensions.Count + "):");
                var index = -1;
                foreach (var t in AuthorizedExtensions.Keys)
                {
                    index++;

                    var extensionMessage = HGEditor.TAB + "[" + index + "] ";

                    if (LinkedExtensions.Count != 0)
                        foreach (var lt in LinkedExtensions.Keys)
                            if (LinkedExtensions[lt].Contains(AuthorizedExtensions[t]))
                            {
                                extensionMessage += lt + " => ";
                                break;
                            }

                    extensionMessage += t;
                    if (!AuthorizedExtensions[t].ExtensionEnabled)
                        extensionMessage += " (Disabled)";
                    _editorMessageOnPlaying.Add(extensionMessage);
                }
            }
        }
#endif
    }

    public abstract class HGBaseExtension<TExtension> : HGBaseExtension where TExtension : class
    {
        /// Сюда будут вкладываться все расширения, которые могут быть преобразованы в TExtension
        protected List<TExtension> Extensions = new List<TExtension>();

        public override void AddExtension(HGExtension authorizedExtension)
        {
            base.AddExtension(authorizedExtension);

            var t = authorizedExtension.GetType();
            if (AuthorizedExtensions.ContainsKey(t))
                if (authorizedExtension is TExtension extension)
                    if (!Extensions.Contains(extension))
                        Extensions.Add(extension);
        }

        public override void RemoveExtension(HGExtension authorizedExtension)
        {
            base.RemoveExtension(authorizedExtension);

            var t = authorizedExtension.GetType();
            if (authorizedExtension is TExtension extension)
                if (Extensions.Contains(extension))
                    Extensions.Remove(extension);
        }
    }
}