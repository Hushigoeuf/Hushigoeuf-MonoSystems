namespace Hushigoeuf
{
    public enum HGExtensionPositions
    {
        Self,
        Parent,
        Hierarchy,
        Custom
    }

    public abstract class HGBaseExtension : HGMonoBehaviour
    {
        /// Где находится расширение по отношению к родителю (по умолчанию как компонент этого же объекта)
        public virtual HGExtensionPositions ExtensionPosition => HGExtensionPositions.Self;

        /// ID расширения по которому с ним можно работать в дальнейшем
        public virtual string ExtensionID => null;

        protected bool _initialized;

        public bool Initialized => _initialized;

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

        /// <summary>
        /// Регистрирует новое дочерние расширение.
        /// </summary>
        public virtual void RegisterSecondExtension(HGBaseExtension target)
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