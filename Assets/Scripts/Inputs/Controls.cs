// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f2205548-c8c9-48f0-a77e-a4fe930abe4b"",
            ""actions"": [
                {
                    ""name"": ""Move Camera"",
                    ""type"": ""Value"",
                    ""id"": ""89cb2db5-ac2c-40cc-98da-34b998fbb29b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6d9af3bf-a6cf-4da2-94d7-16a89b8451dd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BuildMine"",
                    ""type"": ""Button"",
                    ""id"": ""2eb4b626-97af-4981-9fce-c3da060fe3e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BuildSpawner"",
                    ""type"": ""Button"",
                    ""id"": ""1fa44663-1efe-4349-a497-f1484efb3e4b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpawnUnit"",
                    ""type"": ""Button"",
                    ""id"": ""509700dc-4ad4-4645-8b42-e600e118ca0b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SendUnits"",
                    ""type"": ""Button"",
                    ""id"": ""2978d7e0-bbe4-47c6-b4da-61c1354a1f2b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""1521a26c-75b3-4132-af46-309e57b24e91"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""62ca024f-0933-460a-946c-ef3e356a98eb"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d837d549-298d-428b-baa6-38abd14a005d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26b087e7-4e66-4481-a46c-189fd2e611db"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""13b44ee8-4f65-4f0e-b2b7-48446ae5ade1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""5e8c4fe3-fba5-40b3-9b0b-e9dba7c616aa"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""06799117-d55f-4a4a-b46f-3461a33d847b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8818c774-d2f7-426a-9e77-c7453b9aaa26"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fcecb331-edc7-42ee-bf2e-27b533f04c18"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2ac3bf87-8f73-415c-b158-f08de3ca78f7"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""42e35d04-5adb-48ee-b6de-182c6b2f9ee8"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""984d71de-4e59-41bf-a674-c1078a0f8d14"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""BuildMine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd3842d9-de47-4b27-9af9-cc80d3c202d6"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""BuildSpawner"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04a59a05-8a16-43be-baff-38d76212a9fb"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""SpawnUnit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ac8a8fe-c2b0-413b-9238-c464a3a81343"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & mouse"",
                    ""action"": ""SendUnits"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & mouse"",
            ""bindingGroup"": ""Keyboard & mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MoveCamera = m_Player.FindAction("Move Camera", throwIfNotFound: true);
        m_Player_ZoomCamera = m_Player.FindAction("Zoom Camera", throwIfNotFound: true);
        m_Player_BuildMine = m_Player.FindAction("BuildMine", throwIfNotFound: true);
        m_Player_BuildSpawner = m_Player.FindAction("BuildSpawner", throwIfNotFound: true);
        m_Player_SpawnUnit = m_Player.FindAction("SpawnUnit", throwIfNotFound: true);
        m_Player_SendUnits = m_Player.FindAction("SendUnits", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MoveCamera;
    private readonly InputAction m_Player_ZoomCamera;
    private readonly InputAction m_Player_BuildMine;
    private readonly InputAction m_Player_BuildSpawner;
    private readonly InputAction m_Player_SpawnUnit;
    private readonly InputAction m_Player_SendUnits;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveCamera => m_Wrapper.m_Player_MoveCamera;
        public InputAction @ZoomCamera => m_Wrapper.m_Player_ZoomCamera;
        public InputAction @BuildMine => m_Wrapper.m_Player_BuildMine;
        public InputAction @BuildSpawner => m_Wrapper.m_Player_BuildSpawner;
        public InputAction @SpawnUnit => m_Wrapper.m_Player_SpawnUnit;
        public InputAction @SendUnits => m_Wrapper.m_Player_SendUnits;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MoveCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @ZoomCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                @ZoomCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                @ZoomCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                @BuildMine.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildMine;
                @BuildMine.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildMine;
                @BuildMine.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildMine;
                @BuildSpawner.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildSpawner;
                @BuildSpawner.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildSpawner;
                @BuildSpawner.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBuildSpawner;
                @SpawnUnit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpawnUnit;
                @SpawnUnit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpawnUnit;
                @SpawnUnit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpawnUnit;
                @SendUnits.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSendUnits;
                @SendUnits.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSendUnits;
                @SendUnits.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSendUnits;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
                @ZoomCamera.started += instance.OnZoomCamera;
                @ZoomCamera.performed += instance.OnZoomCamera;
                @ZoomCamera.canceled += instance.OnZoomCamera;
                @BuildMine.started += instance.OnBuildMine;
                @BuildMine.performed += instance.OnBuildMine;
                @BuildMine.canceled += instance.OnBuildMine;
                @BuildSpawner.started += instance.OnBuildSpawner;
                @BuildSpawner.performed += instance.OnBuildSpawner;
                @BuildSpawner.canceled += instance.OnBuildSpawner;
                @SpawnUnit.started += instance.OnSpawnUnit;
                @SpawnUnit.performed += instance.OnSpawnUnit;
                @SpawnUnit.canceled += instance.OnSpawnUnit;
                @SendUnits.started += instance.OnSendUnits;
                @SendUnits.performed += instance.OnSendUnits;
                @SendUnits.canceled += instance.OnSendUnits;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardmouseSchemeIndex = -1;
    public InputControlScheme KeyboardmouseScheme
    {
        get
        {
            if (m_KeyboardmouseSchemeIndex == -1) m_KeyboardmouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & mouse");
            return asset.controlSchemes[m_KeyboardmouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnZoomCamera(InputAction.CallbackContext context);
        void OnBuildMine(InputAction.CallbackContext context);
        void OnBuildSpawner(InputAction.CallbackContext context);
        void OnSpawnUnit(InputAction.CallbackContext context);
        void OnSendUnits(InputAction.CallbackContext context);
    }
}
