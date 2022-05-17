using UnityEngine;

namespace Hushigoeuf
{
    /// <summary>
    /// Этот класс определяет компонент как расширение-одиночку.
    /// Такое расширение может быть только первичным и существовать в одном экземпляре.
    /// </summary>
    public abstract class HGSingletonExtension<TFirstExtension, TSecondExtension> : HGFirstExtension<TSecondExtension>
        where TFirstExtension : HGBaseExtension where TSecondExtension : HGBaseExtension
    {
        protected static TFirstExtension _instance;

        public static TFirstExtension Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<TFirstExtension>();
                return _instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as TFirstExtension;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_instance == this)
                _instance = null;
        }
    }

    /// <summary>
    /// Можно переопределить этот класс, чтобы не задавать тип дочернего расширения.
    /// </summary>
    public abstract class HGSingletonExtension<TFirstExtension> :
        HGSingletonExtension<TFirstExtension, HGFirstExtension<HGBaseExtension>>
        where TFirstExtension : HGBaseExtension
    {
    }
}