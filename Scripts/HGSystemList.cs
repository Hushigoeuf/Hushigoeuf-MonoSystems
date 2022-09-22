using System;
using System.Collections.Generic;

namespace Hushigoeuf.MonoSystems
{
    public class HGSystemList
    {
        public readonly Dictionary<Type, HGMonoSystem> AuthorizedSystems = new Dictionary<Type, HGMonoSystem>();
        public readonly List<HGMonoSystem> ActivatedSystems = new List<HGMonoSystem>();

        private readonly List<HGMonoSystem> _tempSystems = new List<HGMonoSystem>();

        public HGMonoSystem this[Type type] => AuthorizedSystems?[type];

        public bool Contains(Type type) => AuthorizedSystems.ContainsKey(type);
        public bool Contains(HGMonoSystem system) => Contains(system.GetType());

        public void Subscribe(HGMonoSystem system)
        {
            if (Contains(system)) return;
            AddActivatedSystem(system);
            system.SystemAuthorized = true;
            AuthorizedSystems.Add(system.GetType(), system);
        }

        private void AddActivatedSystem(HGMonoSystem system)
        {
            if (system.DependentType == null)
            {
                AddActivatedSystem(system, ActivatedSystems.Count);
                return;
            }

            int i = ActivatedSystems.FindIndex(0, current => current.SystemType == system.DependentType);
            if (i >= 0) AddActivatedSystem(system, i + 1);
        }

        private void AddActivatedSystem(HGMonoSystem system, int index)
        {
            ActivatedSystems.Insert(index, system);
            foreach (var s in AuthorizedSystems.Values)
                if (s.DependentType == system.SystemType)
                    AddActivatedSystem(s, index + 1);
        }

        public void Unsubscribe(HGMonoSystem system)
        {
            if (!Contains(system)) return;

            RemoveActivatedSystem(system);
            _tempSystems.Clear();

            system.SystemAuthorized = false;
            AuthorizedSystems.Remove(system.GetType());
        }

        private void RemoveActivatedSystem(HGMonoSystem system)
        {
            ActivatedSystems.Remove(system);

            int startIndex = _tempSystems.Count;
            foreach (var s in ActivatedSystems)
                if (s.DependentType == system.SystemType)
                    _tempSystems.Add(s);
            int count = _tempSystems.Count - startIndex;
            for (int i = startIndex; i < startIndex + count; i++)
                RemoveActivatedSystem(_tempSystems[i]);
        }

        public void Initialize()
        {
            foreach (var s in AuthorizedSystems.Values)
                Initialize(s);
        }

        public void Initialize(Type systemType)
        {
            if (!Contains(systemType)) return;
            Initialize(AuthorizedSystems[systemType]);
        }

        private void Initialize(HGMonoSystem system)
        {
            if (!system.Initialized) system.Initialize();
            foreach (var s in AuthorizedSystems.Values)
                if (s.DependentType == system.SystemType)
                    Initialize(s);
        }
    }
}