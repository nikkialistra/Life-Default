//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.1.1
//     from Assets/Scripts/Controls/Control.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Controls
{
    public partial class @Control : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Control()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Control"",
    ""maps"": [
        {
            ""name"": ""Management"",
            ""id"": ""4cc9433d-d740-4593-a299-245a457e2ffd"",
            ""actions"": [
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""8653e1ae-6cec-4d4d-b899-be655e768b2f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""6cb4cd5a-fe25-455c-8835-330c76a727e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SetFollow"",
                    ""type"": ""Button"",
                    ""id"": ""3a2302e8-af2d-4adc-bae2-e850d2eaa14b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ResetFollow"",
                    ""type"": ""Button"",
                    ""id"": ""e72a7247-f0fc-4d95-826c-d8c03af268d7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ZoomScroll"",
                    ""type"": ""Value"",
                    ""id"": ""d6a2616d-3501-4776-b0e6-9bd0f48d43b4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Drag"",
                    ""type"": ""Button"",
                    ""id"": ""d1531ab8-d879-4368-96bb-d14c4c98fe0b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a08e44b6-96db-4738-afa7-f075bafe4bdf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""862651f6-dc28-4bde-a835-01291b3077b5"",
                    ""expectedControlType"": ""Digital"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""141de89e-0dc4-4a48-bf02-3f6283acbf65"",
                    ""expectedControlType"": ""Digital"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SetDestination"",
                    ""type"": ""Button"",
                    ""id"": ""c4cb563a-cce4-4986-ad62-135ea6b1147f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShowMenu"",
                    ""type"": ""Button"",
                    ""id"": ""bb7a1b1a-bb06-4025-af3c-a2ac225e6f9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Stop"",
                    ""type"": ""Button"",
                    ""id"": ""d17fb049-f097-4c8e-a006-affbaf307f8a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2d0e6b84-dafe-4c53-89ef-b2fba507309f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetFollow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d268ebbb-6978-48b2-b70b-abcdc96a7c7d"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ce75ced-74d6-4161-88f0-0f9da41aed35"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drag"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""06620c29-a204-42ac-8041-11e75e5171de"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2d7a783f-c0ba-44b0-b121-ce9aa443ce39"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""93d8d940-be56-4044-a0a9-16b8128a8e1f"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a886be5f-f36c-48b4-a5f5-eadf136da2f9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3c329feb-b09c-4955-bd83-7111318addee"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""8bd81bf0-9177-46e2-83cb-5188acd0ee76"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""09cbffb9-f22b-406c-8132-38658d51915b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""51711646-ead8-40f0-8b53-a43d6a264301"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""87ca861a-8e3d-4c54-b441-cd3043588626"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d91ea792-9a77-4041-a96d-ee6410f5c610"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""11304acd-7a97-4df6-be95-961f09dab8bf"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""110361ca-368d-4760-81f7-0e20e40eda38"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""854101b2-d0e3-4224-9b65-5b212b2dd351"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""7f23b91f-67a3-4631-bde7-07511c16f1f0"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetFollow"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""9e3f8d34-eb9a-4611-9be3-fea0cf5ba263"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetFollow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""d57f1661-b86a-45d5-9174-a14ec713410f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetFollow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4405cb42-318b-4ec1-b034-87065b1316ac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""25d38ff6-1a48-483a-89fc-6a8b8d553edb"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b74507dd-c6f6-4d64-a198-aabfd1ffd290"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetDestination"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42e513ea-3ea2-4a20-951d-41e7c34124cc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e5b4a34-192b-43f5-92d4-816a1a355632"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menus"",
            ""id"": ""2a18fe8d-6421-4bed-bce6-1abde9ee45bc"",
            ""actions"": [
                {
                    ""name"": ""HideMenu"",
                    ""type"": ""Button"",
                    ""id"": ""e2cd4319-5092-4863-abc8-b2054a5cd28f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fb4bff77-1002-4636-a206-46a7ec082234"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HideMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Management
            m_Management = asset.FindActionMap("Management", throwIfNotFound: true);
            m_Management_Position = m_Management.FindAction("Position", throwIfNotFound: true);
            m_Management_Select = m_Management.FindAction("Select", throwIfNotFound: true);
            m_Management_SetFollow = m_Management.FindAction("SetFollow", throwIfNotFound: true);
            m_Management_ResetFollow = m_Management.FindAction("ResetFollow", throwIfNotFound: true);
            m_Management_ZoomScroll = m_Management.FindAction("ZoomScroll", throwIfNotFound: true);
            m_Management_Drag = m_Management.FindAction("Drag", throwIfNotFound: true);
            m_Management_Movement = m_Management.FindAction("Movement", throwIfNotFound: true);
            m_Management_Rotate = m_Management.FindAction("Rotate", throwIfNotFound: true);
            m_Management_Zoom = m_Management.FindAction("Zoom", throwIfNotFound: true);
            m_Management_SetDestination = m_Management.FindAction("SetDestination", throwIfNotFound: true);
            m_Management_ShowMenu = m_Management.FindAction("ShowMenu", throwIfNotFound: true);
            m_Management_Stop = m_Management.FindAction("Stop", throwIfNotFound: true);
            // Menus
            m_Menus = asset.FindActionMap("Menus", throwIfNotFound: true);
            m_Menus_HideMenu = m_Menus.FindAction("HideMenu", throwIfNotFound: true);
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
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Management
        private readonly InputActionMap m_Management;
        private IManagementActions m_ManagementActionsCallbackInterface;
        private readonly InputAction m_Management_Position;
        private readonly InputAction m_Management_Select;
        private readonly InputAction m_Management_SetFollow;
        private readonly InputAction m_Management_ResetFollow;
        private readonly InputAction m_Management_ZoomScroll;
        private readonly InputAction m_Management_Drag;
        private readonly InputAction m_Management_Movement;
        private readonly InputAction m_Management_Rotate;
        private readonly InputAction m_Management_Zoom;
        private readonly InputAction m_Management_SetDestination;
        private readonly InputAction m_Management_ShowMenu;
        private readonly InputAction m_Management_Stop;
        public struct ManagementActions
        {
            private @Control m_Wrapper;
            public ManagementActions(@Control wrapper) { m_Wrapper = wrapper; }
            public InputAction @Position => m_Wrapper.m_Management_Position;
            public InputAction @Select => m_Wrapper.m_Management_Select;
            public InputAction @SetFollow => m_Wrapper.m_Management_SetFollow;
            public InputAction @ResetFollow => m_Wrapper.m_Management_ResetFollow;
            public InputAction @ZoomScroll => m_Wrapper.m_Management_ZoomScroll;
            public InputAction @Drag => m_Wrapper.m_Management_Drag;
            public InputAction @Movement => m_Wrapper.m_Management_Movement;
            public InputAction @Rotate => m_Wrapper.m_Management_Rotate;
            public InputAction @Zoom => m_Wrapper.m_Management_Zoom;
            public InputAction @SetDestination => m_Wrapper.m_Management_SetDestination;
            public InputAction @ShowMenu => m_Wrapper.m_Management_ShowMenu;
            public InputAction @Stop => m_Wrapper.m_Management_Stop;
            public InputActionMap Get() { return m_Wrapper.m_Management; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ManagementActions set) { return set.Get(); }
            public void SetCallbacks(IManagementActions instance)
            {
                if (m_Wrapper.m_ManagementActionsCallbackInterface != null)
                {
                    @Position.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnPosition;
                    @Position.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnPosition;
                    @Position.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnPosition;
                    @Select.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSelect;
                    @Select.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSelect;
                    @Select.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSelect;
                    @SetFollow.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetFollow;
                    @SetFollow.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetFollow;
                    @SetFollow.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetFollow;
                    @ResetFollow.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnResetFollow;
                    @ResetFollow.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnResetFollow;
                    @ResetFollow.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnResetFollow;
                    @ZoomScroll.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoomScroll;
                    @ZoomScroll.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoomScroll;
                    @ZoomScroll.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoomScroll;
                    @Drag.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnDrag;
                    @Drag.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnDrag;
                    @Drag.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnDrag;
                    @Movement.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnMovement;
                    @Rotate.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnRotate;
                    @Rotate.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnRotate;
                    @Rotate.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnRotate;
                    @Zoom.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoom;
                    @Zoom.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoom;
                    @Zoom.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnZoom;
                    @SetDestination.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetDestination;
                    @SetDestination.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetDestination;
                    @SetDestination.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnSetDestination;
                    @ShowMenu.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnShowMenu;
                    @ShowMenu.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnShowMenu;
                    @ShowMenu.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnShowMenu;
                    @Stop.started -= m_Wrapper.m_ManagementActionsCallbackInterface.OnStop;
                    @Stop.performed -= m_Wrapper.m_ManagementActionsCallbackInterface.OnStop;
                    @Stop.canceled -= m_Wrapper.m_ManagementActionsCallbackInterface.OnStop;
                }
                m_Wrapper.m_ManagementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Position.started += instance.OnPosition;
                    @Position.performed += instance.OnPosition;
                    @Position.canceled += instance.OnPosition;
                    @Select.started += instance.OnSelect;
                    @Select.performed += instance.OnSelect;
                    @Select.canceled += instance.OnSelect;
                    @SetFollow.started += instance.OnSetFollow;
                    @SetFollow.performed += instance.OnSetFollow;
                    @SetFollow.canceled += instance.OnSetFollow;
                    @ResetFollow.started += instance.OnResetFollow;
                    @ResetFollow.performed += instance.OnResetFollow;
                    @ResetFollow.canceled += instance.OnResetFollow;
                    @ZoomScroll.started += instance.OnZoomScroll;
                    @ZoomScroll.performed += instance.OnZoomScroll;
                    @ZoomScroll.canceled += instance.OnZoomScroll;
                    @Drag.started += instance.OnDrag;
                    @Drag.performed += instance.OnDrag;
                    @Drag.canceled += instance.OnDrag;
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Rotate.started += instance.OnRotate;
                    @Rotate.performed += instance.OnRotate;
                    @Rotate.canceled += instance.OnRotate;
                    @Zoom.started += instance.OnZoom;
                    @Zoom.performed += instance.OnZoom;
                    @Zoom.canceled += instance.OnZoom;
                    @SetDestination.started += instance.OnSetDestination;
                    @SetDestination.performed += instance.OnSetDestination;
                    @SetDestination.canceled += instance.OnSetDestination;
                    @ShowMenu.started += instance.OnShowMenu;
                    @ShowMenu.performed += instance.OnShowMenu;
                    @ShowMenu.canceled += instance.OnShowMenu;
                    @Stop.started += instance.OnStop;
                    @Stop.performed += instance.OnStop;
                    @Stop.canceled += instance.OnStop;
                }
            }
        }
        public ManagementActions @Management => new ManagementActions(this);

        // Menus
        private readonly InputActionMap m_Menus;
        private IMenusActions m_MenusActionsCallbackInterface;
        private readonly InputAction m_Menus_HideMenu;
        public struct MenusActions
        {
            private @Control m_Wrapper;
            public MenusActions(@Control wrapper) { m_Wrapper = wrapper; }
            public InputAction @HideMenu => m_Wrapper.m_Menus_HideMenu;
            public InputActionMap Get() { return m_Wrapper.m_Menus; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MenusActions set) { return set.Get(); }
            public void SetCallbacks(IMenusActions instance)
            {
                if (m_Wrapper.m_MenusActionsCallbackInterface != null)
                {
                    @HideMenu.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnHideMenu;
                    @HideMenu.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnHideMenu;
                    @HideMenu.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnHideMenu;
                }
                m_Wrapper.m_MenusActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @HideMenu.started += instance.OnHideMenu;
                    @HideMenu.performed += instance.OnHideMenu;
                    @HideMenu.canceled += instance.OnHideMenu;
                }
            }
        }
        public MenusActions @Menus => new MenusActions(this);
        public interface IManagementActions
        {
            void OnPosition(InputAction.CallbackContext context);
            void OnSelect(InputAction.CallbackContext context);
            void OnSetFollow(InputAction.CallbackContext context);
            void OnResetFollow(InputAction.CallbackContext context);
            void OnZoomScroll(InputAction.CallbackContext context);
            void OnDrag(InputAction.CallbackContext context);
            void OnMovement(InputAction.CallbackContext context);
            void OnRotate(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
            void OnSetDestination(InputAction.CallbackContext context);
            void OnShowMenu(InputAction.CallbackContext context);
            void OnStop(InputAction.CallbackContext context);
        }
        public interface IMenusActions
        {
            void OnHideMenu(InputAction.CallbackContext context);
        }
    }
}
