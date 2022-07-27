using System.Collections.Generic;
using UnityEngine;

namespace Hushigoeuf
{
    /// <summary>
    /// Этот класс определяет компонент как контейнер-одиночку с расширениями.
    /// Такой контейнер может существовать только в одном экземпляре.
    /// </summary>
    public abstract class HGSingletonExtension<TCurrent, TExtension> : HGBaseExtension<TExtension>
        where TCurrent : HGSingletonExtension<TCurrent, TExtension> where TExtension : class
    {
        protected static TCurrent _instance;

        public static TCurrent Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<TCurrent>();
                return _instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            _instance = this as TCurrent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_instance == this)
                _instance = null;
        }
    }

    public abstract class HGSingletonListExtension<TCurrent, TExtension> : HGBaseExtension<TExtension>
        where TCurrent : HGSingletonExtension<TCurrent, TExtension> where TExtension : class
    {
        public static Dictionary<string, TCurrent> Instances = new Dictionary<string, TCurrent>();

        public virtual string InstanceID => null;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            if (InstanceID == null) return;

            if (Instances.ContainsKey(InstanceID))
            {
                Destroy(this);
                return;
            }

            Instances.Add(InstanceID, this as TCurrent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (InstanceID == null) return;

            if (Instances.ContainsKey(InstanceID))
                Instances.Remove(InstanceID);
        }
    }
}