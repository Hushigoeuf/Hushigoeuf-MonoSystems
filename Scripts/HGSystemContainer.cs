using System;
using UnityEngine;

namespace Hushigoeuf.MonoSystems
{
    public abstract class HGSystemContainer : MonoBehaviour
    {
        public readonly HGSystemList SystemList = new HGSystemList();

        private bool _initialized;
        private Transform _transform;

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
            Initialize();
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
            float dt = Time.deltaTime;
            OnUpdate(dt);
            foreach (var e in SystemList.ActivatedSystems)
                if (e.SystemEnabled)
                    e.OnUpdate(dt);
        }

        protected virtual void FixedUpdate()
        {
            float fdt = Time.fixedDeltaTime;
            OnFixedUpdate(fdt);
            foreach (var e in SystemList.ActivatedSystems)
                if (e.SystemEnabled)
                    e.OnFixedUpdate(fdt);
        }

        protected virtual void LateUpdate()
        {
            float dt = Time.deltaTime;
            OnLateUpdate(dt);
            foreach (var e in SystemList.ActivatedSystems)
                if (e.SystemEnabled)
                    e.OnLateUpdate(dt);
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            OnInitialize();
            SystemList.Initialize();
        }

        public void Initialize(Type systemType)
        {
            if (!_initialized) return;
            SystemList.Initialize(systemType);
        }

        public void Initialize(HGMonoSystem systemType) => Initialize(systemType.GetType());

        public bool ContainsSystem(HGMonoSystem system) => SystemList.Contains(system);

        public bool ContainsSystem(Type systemType) => SystemList.Contains(systemType);

        public void Subscribe(HGMonoSystem system) => SystemList.Subscribe(system);

        public void Unsubscribe(HGMonoSystem system) => SystemList.Unsubscribe(system);

        public HGMonoSystem GetSystem(Type systemType) => SystemList[systemType];

        public T GetSystem<T>() where T : HGMonoSystem => GetSystem(typeof(T)) as T;

        protected virtual void OnInitialize()
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
    }
}