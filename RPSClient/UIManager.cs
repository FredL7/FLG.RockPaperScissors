using Godot;
using System.Collections.Generic;

using FLG.Cs.Datamodel;
using FLG.Cs.Framework;
using FLG.Cs.Math;
using FLG.Cs.ServiceLocator;
using FLG.Cs.UI;
using FLG.Godot.Framework;
using FLG.Godot.Helpers;
using FLG.Godot.UI;

using sysV2 = System.Numerics.Vector2;
using gdV2 = Godot.Vector2;
using flgLabel = FLG.Godot.UI.Label;
using flgButton = FLG.Godot.UI.Button;


namespace FLG.Godot.Sample {
    [Tool]
    public partial class UIManager : Control, IUIObserver {
        private static readonly string[] UI_RELATIVE_PATH = { "../commons/ProjectDefs.UI/ui/general/", "../commons/ProjectDefs.UI/ui/client/" };
        private const string HOMEPAGE = "HomepageClient";


        private Size _window = new(1920, 1080);

        private ILogManager _logger;
        private IUIFactory _factory;
        private IUIManager _ui;

        private Dictionary<string, Node> _layouts = new();
        private Dictionary<string, List<Node>> _pages = new();
        private string _currentLayout = string.Empty;
        private string _currentPage = string.Empty;

        public override void _Ready()
        {
            base._Ready();

            if (Engine.IsEditorHint())
            {
                InitializeFromEditor();
            }
        }

        private string[] GetGlobalizedUiDirs()
        {
            var globalUiDir = new string[UI_RELATIVE_PATH.Length];
            for (int i = 0; i < globalUiDir.Length; ++i)
                globalUiDir[i] = ProjectSettings.GlobalizePath("res://" + UI_RELATIVE_PATH[i]);
            return globalUiDir;
        }

        private void InitializeFromEditor()
        {
            _logger = new GodotLogger();
            _factory = new UIFactory();

            PreferencesUI prefsUI = new()
            {
                uiDirs = GetGlobalizedUiDirs(),
                windowSize = _window,

                logger = _logger,
                factory = _factory
            };

            _ui = new FLG.Cs.UI.UIManager(prefsUI);
            _ui.OnServiceRegistered();

            Clear();
            DrawUI();
        }

        public void Initialize()
        {
            _logger = Locator.Instance.Get<ILogManager>();

            _factory = new UIFactory();

            PreferencesUI prefsUI = new()
            {
                uiDirs = GetGlobalizedUiDirs(),
                windowSize = _window,

                logger = _logger,
                factory = _factory
            };
            FrameworkManager.Instance.InitializeUI(prefsUI);

            _ui = Locator.Instance.Get<IUIManager>();
            _ui.AddObserver(this);

            SetupUI();
        }

        private void SetupUI()
        {
            Clear();
            DrawUI();
            _ui.SetCurrentPage(HOMEPAGE);
        }

        public void OnCurrentPageChanged(string pageId, string layoutId)
        {
            if (_currentPage != pageId)
            {
                foreach (var page in _pages)
                    foreach (var pageItem in page.Value)
                        pageItem.Set("visible", false);

                foreach (var pageItem in _pages[pageId])
                    pageItem.Set("visible", true);

                if (_currentLayout != layoutId)
                {
                    foreach (var layout in _layouts)
                        layout.Value.Set("visible", false);
                    _layouts[layoutId].Set("visible", true);
                }
            }
        }

        private void Clear()
        {
            SceneHelper.RemoveAllChildrensImmediately(this);
        }

        private void DrawUI()
        {
            DrawLayouts();
        }

        private Node AddNode(string name, ILayoutElement layoutElement, Node parent)
        {
            var position = layoutElement.Position;
            var dimensions = layoutElement.Dimensions;

            return AddNode(name, position, dimensions, parent);
        }

        private Node AddNode(string name, sysV2 position, Size dimensions, Node parent)
        {
            Control node = new()
            {
                Name = name,
                Position = new gdV2(position.X, position.Y),
                Size = new gdV2(dimensions.Width, dimensions.Height),
            };
            parent.AddChild(node);
            node.Owner = GetTree().EditedSceneRoot;
            return node;
        }

        private Node AddGridNode(string name, ILayoutElement layoutElement, Node parent)
        {
            var node = AddNode(name, layoutElement, parent);
            if (layoutElement.BackgroundImage != string.Empty)
            {
                _logger.Debug($"background image path: {layoutElement.BackgroundImage}");
                var rect = new TextureRect
                {
                    Name = "backgroundimg",
                    Position = new Vector2(layoutElement.Position.X, layoutElement.Position.Y),
                    Texture = ResourceLoader.Load<Texture2D>("res://" + layoutElement.BackgroundImage),
                    AnchorLeft = 0.5f,
                    AnchorRight = 0.5f,
                    AnchorTop = 0.5f,
                    AnchorBottom = 0.5f
                };

                var originalSize = rect.Texture.GetSize();
                float ratio = 1;
                if (originalSize.X < layoutElement.Dimensions.Width)
                {
                    ratio = layoutElement.Dimensions.Width / originalSize.X;
                }
                else if (originalSize.Y < layoutElement.Dimensions.Height)
                {
                    ratio = layoutElement.Dimensions.Height / originalSize.Y;
                }

                var newSize = new Vector2(originalSize.X * ratio, originalSize.Y * ratio);
                rect.OffsetLeft = -newSize.X / 2f;
                rect.OffsetRight = newSize.X / 2f;
                rect.OffsetTop = -newSize.Y / 2f;
                rect.OffsetBottom = newSize.Y / 2f;
                rect.PivotOffset = new Vector2(newSize.X / 2f, newSize.Y / 2f);
                _logger.Debug($"layoutDimensions={layoutElement.Dimensions} OriginalSize={originalSize}, ratio={ratio}, newSize={newSize}");

                node.AddChild(rect);
                rect.Owner = GetTree().EditedSceneRoot;
            }
            return node;
        }

        private void DrawLayouts()
        {
            foreach (var layout in _ui.GetLayouts())
                DrawLayout(layout);
        }

        private void DrawLayout(ILayout layout)
        {
            string id = layout.Name;
            var root = layout.Root;
            var layoutNode = AddNode("layout " + id, root, this);
            _layouts.Add(id, layoutNode);
            DrawLayoutRecursive(layoutNode, root);
        }

        private void DrawLayoutRecursive(Node parentNode, ILayoutElement layoutElementParent)
        {
            var targets = layoutElementParent.GetTargets();
            foreach (var target in targets)
            {
                if (layoutElementParent.HasChildren(target))
                {
                    var parentForAddNode = parentNode;
                    if (target != ILayoutElement.DEFAULT_CHILDREN_TARGET)
                    {
                        var targetNode = AddNode(target, sysV2.Zero, layoutElementParent.Dimensions, parentNode);

                        if (!_pages.ContainsKey(target))
                            _pages.Add(target, new List<Node>());
                        _pages[target].Add(targetNode);

                        targetNode.Set("visible", false);
                        parentForAddNode = targetNode;
                    }

                    foreach (ILayoutElement child in layoutElementParent.GetChildrens(target))
                    {
                        var node = DrawNode(child, parentForAddNode);
                        DrawLayoutRecursive(node, child);
                    }
                }
            }
        }

        private Node DrawNode(ILayoutElement layoutElement, Node parentNode)
        {
            Node node;
            var root = GetTree().EditedSceneRoot;
            var fromEditor = Engine.IsEditorHint();
            bool parentSetter = true;
            switch (layoutElement.Type)
            {
                case ELayoutElement.BUTTON:
                    IWidget<IButton> btn = new flgButton((IButton)layoutElement);
                    node = btn.Draw(parentNode, fromEditor);
                    break;
                case ELayoutElement.LABEL:
                    IWidget<ILabel> label = new flgLabel((ILabel)layoutElement);
                    node = label.Draw(parentNode, fromEditor);
                    break;
                case ELayoutElement.SPRITE:
                    IWidget<ISprite> sprite = new Sprite((ISprite)layoutElement);
                    node = sprite.Draw(parentNode, fromEditor);
                    break;
                case ELayoutElement.TEXT:
                    IWidget<IText> text = new Text((IText)layoutElement);
                    node = text.Draw(parentNode, fromEditor);
                    break;
                case ELayoutElement.INPUTFIELD:
                    IWidget<IInputField> inputField = new InputField((IInputField)layoutElement);
                    node = inputField.Draw(parentNode, fromEditor);
                    break;
                case ELayoutElement.CONTAINER:
                case ELayoutElement.HSTACK:
                case ELayoutElement.VSTACK:
                    node = AddGridNode(layoutElement.Name, layoutElement, parentNode);
                    parentSetter = false;
                    break;
                default:
                    node = AddNode(layoutElement.Name, layoutElement, parentNode);
                    parentSetter = false;
                    break;
            }

            if (parentSetter)
            {
                parentNode.AddChild(node);
                node.Owner = root;
            }

            return node;
        }
    }
}
