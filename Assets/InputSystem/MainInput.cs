// GENERATED AUTOMATICALLY FROM 'Assets/InputSystem/MainInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MainInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainInput"",
    ""maps"": [
        {
            ""name"": ""BattleActions"",
            ""id"": ""dbfd6c29-fd0c-4107-9288-e7cde97eceb2"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""95f07def-6942-4391-a972-965e1cec9aa0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""a1b5c8f0-8cbd-471c-8762-8285c122edec"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c01cc124-10b2-49be-813b-93eb80a2f6e1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8883075d-c7a3-42ff-a79e-6e4e3bded7f3"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""NormalActions"",
            ""id"": ""3b64f373-267e-4a05-896a-3e771f140261"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""f124ff13-ee6c-4e4b-a47b-a4c22eb03a9d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""598bf107-8cdb-4f14-81d7-61abc156107e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // BattleActions
        m_BattleActions = asset.FindActionMap("BattleActions", throwIfNotFound: true);
        m_BattleActions_Select = m_BattleActions.FindAction("Select", throwIfNotFound: true);
        m_BattleActions_MousePosition = m_BattleActions.FindAction("MousePosition", throwIfNotFound: true);
        // NormalActions
        m_NormalActions = asset.FindActionMap("NormalActions", throwIfNotFound: true);
        m_NormalActions_Select = m_NormalActions.FindAction("Select", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // BattleActions
    private readonly InputActionMap m_BattleActions;
    private IBattleActionsActions m_BattleActionsActionsCallbackInterface;
    private readonly InputAction m_BattleActions_Select;
    private readonly InputAction m_BattleActions_MousePosition;
    public struct BattleActionsActions
    {
        private @MainInput m_Wrapper;
        public BattleActionsActions(@MainInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_BattleActions_Select;
        public InputAction @MousePosition => m_Wrapper.m_BattleActions_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_BattleActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BattleActionsActions set) { return set.Get(); }
        public void SetCallbacks(IBattleActionsActions instance)
        {
            if (m_Wrapper.m_BattleActionsActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnSelect;
                @MousePosition.started -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_BattleActionsActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_BattleActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public BattleActionsActions @BattleActions => new BattleActionsActions(this);

    // NormalActions
    private readonly InputActionMap m_NormalActions;
    private INormalActionsActions m_NormalActionsActionsCallbackInterface;
    private readonly InputAction m_NormalActions_Select;
    public struct NormalActionsActions
    {
        private @MainInput m_Wrapper;
        public NormalActionsActions(@MainInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_NormalActions_Select;
        public InputActionMap Get() { return m_Wrapper.m_NormalActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NormalActionsActions set) { return set.Get(); }
        public void SetCallbacks(INormalActionsActions instance)
        {
            if (m_Wrapper.m_NormalActionsActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_NormalActionsActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_NormalActionsActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_NormalActionsActionsCallbackInterface.OnSelect;
            }
            m_Wrapper.m_NormalActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
            }
        }
    }
    public NormalActionsActions @NormalActions => new NormalActionsActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IBattleActionsActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
    public interface INormalActionsActions
    {
        void OnSelect(InputAction.CallbackContext context);
    }
}
