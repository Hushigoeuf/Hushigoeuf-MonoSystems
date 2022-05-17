namespace Hushigoeuf
{
    /// <summary>
    /// Этот класс определяет компонент как дочернее расширение с управляемым триггером.
    /// Такие компоненты можно включать и выключать помимо стандартного enabled.
    /// </summary>
    public abstract class HGWorkingExtension<TFirstExtension, TSecondExtension> :
        HGSecondExtension<TFirstExtension, TSecondExtension>
        where TFirstExtension : HGBaseExtension where TSecondExtension : HGBaseExtension
    {
        protected bool _working;

        /// <summary>
        /// Работает ли в данный момент расширение.
        /// </summary>
        public bool IsWorking
        {
            get => _working;
            set
            {
                if (value) StartWorking();
                else StopWorking();
            }
        }

        /// <summary>
        /// Запускает работу расширения.
        /// </summary>
        public virtual void StartWorking()
        {
            if (_working) return;
            _working = true;
            OnStartWorking();
        }

        /// <summary>
        /// Останавливает работу расширения.
        /// </summary>
        public virtual void StopWorking()
        {
            if (!_working) return;
            _working = false;
            OnStopWorking();
        }

        /// <summary>
        /// Вызывается только на старте работы.
        /// </summary>
        protected virtual void OnStartWorking()
        {
        }

        /// <summary>
        /// Вызывается только при остановке работы.
        /// </summary>
        protected virtual void OnStopWorking()
        {
        }
    }

    /// <summary>
    /// Можно переопределить этот класс, чтобы не задавать тип дочернего расширения.
    /// </summary>
    public abstract class HGWorkingExtension<TFirstExtension> :
        HGWorkingExtension<TFirstExtension, HGWorkingExtension<HGBaseExtension>>
        where TFirstExtension : HGBaseExtension
    {
    }
}