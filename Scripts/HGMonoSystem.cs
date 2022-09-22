using System;
using UnityEngine;

namespace Hushigoeuf.MonoSystems
{
    public abstract class HGMonoSystem : MonoBehaviour
    {
        protected bool _initialized;
        protected Transform _transform;

        public bool SystemAuthorized { get; set; }
        public bool Initialized => _initialized;

        public Type SystemType => GetType();
        public bool IsDependentSystem => DependentType != null;

        public virtual Type DependentType => null;
        public virtual bool SystemEnabled => isActiveAndEnabled && SystemAuthorized;

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

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            OnInitialize();
        }

        protected virtual void Subscribe()
        {
        }

        protected virtual void Unsubscribe()
        {
        }

        protected virtual void OnInitialize()
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

    public abstract class HGMonoSystem<TContainer> : HGMonoSystem where TContainer : HGSystemContainer
    {
        protected TContainer _container;

        public TContainer Container
        {
            get
            {
                _container ??= GetComponent<TContainer>();
                _container ??= GetComponentInParent<TContainer>();
                _container ??= FindObjectOfType<TContainer>();
                return _container;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!SystemAuthorized) Subscribe();

            Container.Initialize(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (SystemAuthorized)
                if (Container != null)
                    Unsubscribe();
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            Container.Subscribe(this);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();

            Container.Unsubscribe(this);
        }
    }

    public abstract class HGMonoSystem<TContainer, TParent> : HGMonoSystem<TContainer>
        where TContainer : HGSystemContainer
        where TParent : HGMonoSystem<TContainer>
    {
        public override Type DependentType => typeof(TParent);
        public override bool SystemEnabled => base.SystemEnabled && DependentSystem.SystemEnabled;

        protected TParent _dependentSystem;

        public TParent DependentSystem
        {
            get
            {
                if (_dependentSystem == null)
                    _dependentSystem = Container.GetSystem<TParent>();
                return _dependentSystem;
            }
        }
    }
}