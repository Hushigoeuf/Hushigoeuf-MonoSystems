using UnityEngine;

namespace Hushigoeuf
{
    public enum HGExtensionPositions
    {
        Self,
        Parent,
        Hierarchy
    }

    public abstract class HGBaseExtension : HGMonoBehaviour
    {
        /// Где находится расширение по отношению к родителю (по умолчанию как компонент этого же объекта)
        protected virtual HGExtensionPositions ExtensionPosition => HGExtensionPositions.Self;

        /// ID расширения по которому с ним можно работать в дальнейшем
        public virtual string ExtensionID => null;

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
            Initialization();
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
            if (!_initialized)
            {
                _initialized = true;
                OnInitialization();
            }
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

        /// <summary>
        /// Регистрирует новое дочерние расширение.
        /// </summary>
        public virtual void RegisterSecondExtension(HGBaseExtension target)
        {
        }

        public virtual void UnregisterSecondExtension(HGBaseExtension target)
        {
        }

        /// <summary>
        /// Проверяет, является ли заданная цель родителем.
        /// </summary>
        public virtual bool CompareFirstExtension(HGBaseExtension target) => false;

        /// <summary>
        /// Кастомный сброс расширения.
        /// </summary>
        public virtual void ResetExtension(bool value)
        {
        }
    }
}