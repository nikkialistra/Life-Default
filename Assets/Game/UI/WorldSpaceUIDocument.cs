using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Game.UI
{
    // class from internet while waiting for official unity ui toolkit world space panels
    public class WorldSpaceUIDocument : MonoBehaviour
    {
        public event Action RebuildFinish;
        
        public UIDocument UiDocument => _uiDocument;
        
        [SerializeField] private int _panelWidth = 1280;
        [SerializeField] private int _panelHeight = 720;
        [SerializeField] private float _panelScale = 1.0f;
        [SerializeField] private float _pixelsPerUnit = 1280.0f;
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private PanelSettings _panelSettingsPrefab;
        [SerializeField] private RenderTexture _renderTexturePrefab;

        private MeshRenderer _meshRenderer;
        private PanelEventHandler _panelEventHandler;
        
        // runtime rebuildable stuff
        private UIDocument _uiDocument;
        private PanelSettings _panelSettings;
        private RenderTexture _renderTexture;
        private Material _material;

        void Awake ()
        {
            // dynamically a MeshFilter, MeshRenderer and BoxCollider
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.allowOcclusionWhenDynamic = false;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            Vector3 size = boxCollider.size;
            size.z = 0;
            boxCollider.size = size;

            // set the primitive quad mesh to the mesh filter
            GameObject quadGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            meshFilter.sharedMesh = quadGo.GetComponent<MeshFilter>().sharedMesh;
            Destroy(quadGo);
        }

        void Start()
        {
            RebuildPanel();
        }
        
        private void RebuildPanel ()
        {
            DestroyGeneratedAssets();

            // generate render texture
            RenderTextureDescriptor textureDescriptor = _renderTexturePrefab.descriptor;
            textureDescriptor.width = _panelWidth;
            textureDescriptor.height = _panelHeight;
            _renderTexture = new RenderTexture(textureDescriptor);

            // generate panel settings
            _panelSettings = Instantiate(_panelSettingsPrefab);
            _panelSettings.targetTexture = _renderTexture;
            _panelSettings.clearColor = true; // ConstantPixelSize and clearColor are mandatory configs
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _renderTexture.name = $"{name} - RenderTexture";
            _panelSettings.name = $"{name} - PanelSettings";

            // generate UIDocument
            _uiDocument = gameObject.AddComponent<UIDocument>();
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;

            // generate material
            if (_panelSettings.colorClearValue.a < 1.0f)
                _material = new Material(Shader.Find("Unlit/Transparent"));
            else
                _material = new Material(Shader.Find("Unlit/Texture"));
            
            _material.SetTexture("_MainTex", _renderTexture);
            _meshRenderer.sharedMaterial = _material;

            RefreshPanelSize();

            // find the automatically generated PanelEventHandler and PanelRaycaster for this panel and disable the raycaster
            PanelEventHandler[] handlers = FindObjectsOfType<PanelEventHandler>();

            foreach (PanelEventHandler handler in handlers)
            {
                if (handler.panel == _uiDocument.rootVisualElement.panel)
                {
                    _panelEventHandler = handler;
                    PanelRaycaster panelRaycaster = _panelEventHandler.GetComponent<PanelRaycaster>();
                    if (panelRaycaster != null)
                        panelRaycaster.enabled = false;
                    
                    break;
                }
            }
            
            RebuildFinish?.Invoke();
        }

        private void RefreshPanelSize ()
        {
            if (_renderTexture != null && (_renderTexture.width != _panelWidth || _renderTexture.height != _panelHeight))
            {
                _renderTexture.Release();
                _renderTexture.width = _panelWidth;
                _renderTexture.height = _panelHeight;
                _renderTexture.Create();

                if (_uiDocument != null)
                    _uiDocument.rootVisualElement?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(_panelWidth / _pixelsPerUnit, _panelHeight / _pixelsPerUnit, 1.0f);
        }

        private void DestroyGeneratedAssets ()
        {
            if (_uiDocument) Destroy(_uiDocument);
            if (_renderTexture) Destroy(_renderTexture);
            if (_panelSettings) Destroy(_panelSettings);
            if (_material) Destroy(_material);
        }

        void OnDestroy ()
        {
            DestroyGeneratedAssets();
        }
    }
}