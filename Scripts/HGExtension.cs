using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Hushigoeuf
{
    public enum HGExtensionPositions
    {
        Self,
        Parent,
        Hierarchy
    }

    /// <summary>
    /// Этот класс определяет компонент как расширение.
    /// К примеру, компонент Health может определять здоровье персонажа и быть частью Player.
    /// </summary>
    public abstract class HGExtension : HGMonoBehaviour
    {
        /// Зарегистрировано ли данное расширение в базовом классе
        [NonSerialized] public bool ExtensionAuthorized;

        /// Работает ли данное расширение
        public virtual bool ExtensionEnabled => isActiveAndEnabled && ExtensionAuthorized;

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

        /// <summary>
        /// Инициализирует расширение.
        /// Инициализация происходит по порядку, от родителя вниз по лестнице.
        /// </summary>
        public virtual void Initialization()
        {
            if (_initialized) return;
            _initialized = true;
            OnInitialization();
        }

        /// <summary>
        /// Кастомный сброс расширения.
        /// </summary>
        public virtual void ResetExtension(bool value)
        {
        }

        protected virtual void AddExtension()
        {
        }

        protected virtual void RemoveExtension()
        {
        }

        protected virtual void OnInitialization()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnFixedUpdate(float fdt)
        {
        }

        public virtual void OnLateUpdate(float dt)
        {
        }
    }

    public abstract class HGExtension<TBase> : HGExtension where TBase : HGBaseExtension
    {
        /// Где находится расширение по отношению к родителю (по умолчанию как компонент этого же объекта)
        [HGBorders] [HGReadOnly] [SerializeField]
        protected HGExtensionPositions _extensionPosition = HGExtensionPositions.Self;

        protected TBase _base;

        /// Родительское расширение (если не задан, то пытается найти его самостоятельно)
        public TBase Base
        {
            get
            {
                if (_base == null)
                    switch (_extensionPosition)
                    {
                        case HGExtensionPositions.Self:
                            _base = GetComponent<TBase>();
                            break;
                        case HGExtensionPositions.Parent:
                            _base = GetComponentInParent<TBase>();
                            break;
                        case HGExtensionPositions.Hierarchy:
                            _base = FindObjectOfType<TBase>();
                            break;
                    }

                return _base;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            // Регистрируем расширение в базовом классе
            if (!ExtensionAuthorized) AddExtension();

            // Если базовый класс уже инициализирован, то делаем принудительную инициализацию
            // Иначе ждем пока родитель сам ее не вызовет
            Base.Initialization(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (ExtensionAuthorized)
                if (Base != null)
                    RemoveExtension();
        }

        protected override void AddExtension()
        {
            base.AddExtension();

            Base.AddExtension(this);
        }

        protected override void RemoveExtension()
        {
            base.RemoveExtension();

            Base.RemoveExtension(this);
        }

#if UNITY_EDITOR && ODIN_INSPECTOR
        [OnInspectorInit]
        protected virtual void EditorInitExtensionPosition()
        {
            if (Application.isPlaying) return;

            if (GetComponent<TBase>() != null)
                _extensionPosition = HGExtensionPositions.Self;
            else if (GetComponentInParent<TBase>() != null)
                _extensionPosition = HGExtensionPositions.Parent;
            else
                _extensionPosition = HGExtensionPositions.Hierarchy;
        }
#endif
    }

    public abstract class HGExtension<TBase, TLinked> : HGExtension<TBase>
        where TBase : HGBaseExtension
        where TLinked : HGExtension<TBase>
    {
        protected TLinked _parent;

        public TLinked Parent
        {
            get
            {
                if (_parent == null)
                    _parent = Base.GetExtension<TLinked>();
                return _parent;
            }
        }

        public override bool ExtensionEnabled => base.ExtensionEnabled && Parent.ExtensionEnabled;

        protected override void AddExtension()
        {
            Base.AddLinkedExtension(typeof(TLinked), this);
        }

        protected override void RemoveExtension()
        {
            Base.RemoveLinkedExtension(typeof(TLinked), this);
        }
    }
}