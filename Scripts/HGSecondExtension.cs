using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Hushigoeuf
{
    /// <summary>
    /// Этот класс определяет компонент как дочернее расширение.
    /// Дочернее расширение так же может содержать дочерние, но и имеет родителя.
    ///
    /// К примеру, компонент Health может определять здоровье персонажа и быть частью родителя Player.
    /// </summary>
    public abstract class HGSecondExtension<TFirstExtension, TSecondExtension> : HGFirstExtension<TSecondExtension>
        where TFirstExtension : HGBaseExtension where TSecondExtension : HGBaseExtension
    {
        /// Фиксированный ID расширения (если задан, то в инспекторе его изменить уже нельзя)
        protected virtual string CustomExtensionID => null;

        /// Этот параметр будет виден при условии если можно задать ID в инспекторе
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup(nameof(_dynamicExtensionID), false)]
        [ShowIf("$" + nameof(EditorDynamicIDEnabled))]
        [LabelText("$" + nameof(EditorExtensionIDLabel))]
#endif
        [SerializeField]
        protected string _dynamicExtensionID = string.Empty;

        /// Этот параметр выводится в виде выпадающего списка если задан статичный список с ID
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup(nameof(_dynamicExtensionID), false)]
        [ValueDropdown(nameof(EditorGetDropdownExtensionIDValues))]
        [ShowIf("$" + nameof(EditorStaticIDEnabled))]
        [LabelText("$" + nameof(EditorExtensionIDLabel))]
#endif
        [SerializeField]
        protected string _staticExtensionID = string.Empty;

        /// Этот параметр виден в инспекторе если родителя надо указать вручную
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup(nameof(_parent), false)]
        [ShowIf("$" + nameof(ExtensionPosition), HGExtensionPositions.Custom)]
#endif
        [SerializeField]
        private TFirstExtension _parent;

        /// Родительское расширение (если не задан, то пытается найти его самостоятельно)
        public TFirstExtension Parent
        {
            get
            {
                if (_parent == null)
                    switch (ExtensionPosition)
                    {
                        case HGExtensionPositions.Self:
                            _parent = GetComponent<TFirstExtension>();
                            break;
                        case HGExtensionPositions.Parent:
                            _parent = GetComponentInParent<TFirstExtension>();
                            break;
                        case HGExtensionPositions.Hierarchy:
                            _parent = FindObjectOfType<TFirstExtension>();
                            break;
                    }

                return _parent;
            }
        }

        public override string ExtensionID
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomExtensionID))
                    return CustomExtensionID;
                if (!string.IsNullOrEmpty(_staticExtensionID))
                    return _staticExtensionID;
                return _dynamicExtensionID;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // Регистрируем этот компонент как дочернее расширения для родителя
            if (Parent != null)
                Parent.RegisterSecondExtension(this);
        }

        protected override void Start()
        {
            base.Start();

            // Если родитель уже инициализирован, то делаем принудительную инициализацию
            // Иначе ждем пока родитель сам ее не вызовет
            if (Parent.Initialized)
                Initialization();
        }

        /// <summary>
        /// Проверяет, является ли заданная цель родителем.
        /// </summary>
        public override bool CompareFirstExtension(HGBaseExtension target) => Parent == target as TFirstExtension;

        /// <summary>
        /// Здесь можно определить статичный список ID, которые можно задать в инспекторе.
        /// Если этот список определен, то задать значение вне этого станет невозможным.
        /// </summary>
        protected virtual void InitExtensionIDList(List<string> list)
        {
        }

#if UNITY_EDITOR && ODIN_INSPECTOR
        /// Просто выводит кастомный ID в инспектор как информацию
        [BoxGroup(nameof(_dynamicExtensionID), false)]
        [ShowIf("$" + nameof(EditorCustomIDEnabled))]
        [LabelText("$" + nameof(EditorExtensionIDLabel))]
        [ShowInInspector]
        protected string EditorExtensionID => CustomExtensionID;

        protected List<string> _editorExtensionIDList;
        protected ValueDropdownList<string> _editorDropdownExtensionIDValues;

        /// Имя ID-параметра в инспекторе
        protected string EditorExtensionIDLabel => "Extension ID";

        /// Можно ли менять ID в инспекторе
        protected bool EditorDynamicIDEnabled => !EditorCustomIDEnabled && !EditorIDListEnabled;

        /// Можно ли вывести выпадающий список со списком ID
        protected bool EditorStaticIDEnabled => !EditorCustomIDEnabled && EditorIDListEnabled;

        /// Можно ли вывести фиксированный ID
        protected bool EditorCustomIDEnabled => CustomExtensionID != null;

        /// Содержит ли статичный список ID хоть одно значение
        protected bool EditorIDListEnabled => (_editorExtensionIDList?.Count ?? 0) != 0;

        /// <summary>
        /// Инициализирует параметры для вывода в инспектор.
        /// </summary>
        [OnInspectorInit]
        protected virtual void EditorInitExtensionIDList()
        {
            _editorExtensionIDList = new List<string>();
            InitExtensionIDList(_editorExtensionIDList);

            if (_editorExtensionIDList.Count == 0)
                _staticExtensionID = string.Empty;
            else
                _dynamicExtensionID = string.Empty;

            _editorDropdownExtensionIDValues = new ValueDropdownList<string>();
            _editorDropdownExtensionIDValues.Add("None", string.Empty);
            foreach (var id in _editorExtensionIDList)
                _editorDropdownExtensionIDValues.Add(id);
        }

        protected ValueDropdownList<string> EditorGetDropdownExtensionIDValues() => _editorDropdownExtensionIDValues;
#endif
    }

    /// <summary>
    /// Можно переопределить этот класс, чтобы не задавать тип дочернего расширения.
    /// </summary>
    public abstract class HGSecondExtension<TFirstExtension> :
        HGSecondExtension<TFirstExtension, HGSecondExtension<TFirstExtension>>
        where TFirstExtension : HGBaseExtension
    {
    }
}