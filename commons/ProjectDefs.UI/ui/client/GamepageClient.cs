using FLG.Cs.Datamodel;
using FLG.Cs.ServiceLocator;
using RockPaperScissors;

public class GamepageClient : IPage {
    private const string PAGE_ID = "GamepageClient";
    private const string CHOICE_TEXT_PREPEND = "Selected: ";

    public string PageId { get => PAGE_ID; }
    public string LayoutId { get; set; } = "";

    private IText _choiceText;
    private bool _confirmed;

    public GamepageClient(IUIFactory factory)
    {
        _choiceText = factory.Text("choice-selection", CHOICE_TEXT_PREPEND + "N/A",
            new() { width = 300, margin = new(40, 0, 0, 0) },
            new(alignHorizontal: ETextAlignHorizontal.RIGHT, alignVertical: ETextAlignVertical.CENTER));
        _confirmed = false;
    }

    public void Setup(IUIManager ui, IUIFactory factory)
    {
        var btnPaladin = factory.Button("btn-choice-paladin", "Select paladin", OnBtnPaladin, new() { height = 40 });
        var btnMage = factory.Button("btn-choice-paladin", "Select Mage", OnBtnMage, new() { height = 40 });
        var btnAssassin = factory.Button("btn-choice-paladin", "Select Assassin", OnBtnAssassin, new() { height = 40 });
        var btnConfirm = factory.Button("btn-choice-configm", "Confirm choice", OnBtnConfirm, new() { width = 300 });

        var layout = ui.GetLayout(LayoutId);
        layout.GetTarget("client-choice-paladin").AddChild(btnPaladin, PageId);
        layout.GetTarget("client-choice-mage").AddChild(btnMage, PageId);
        layout.GetTarget("client-choice-assassin").AddChild(btnAssassin, PageId);

        var confirmTarget = layout.GetTarget("client-confirm");
        confirmTarget.AddChild(_choiceText, PageId);
        confirmTarget.AddChild(btnConfirm, PageId);
    }

    public void OnBtnPaladin()
    {
        if (!_confirmed)
        {
            var networking = Locator.Instance.Get<INetworkingManager>();
            var id = networking.Id;
            var manager = Locator.Instance.Get<IRockPaperScissorsManager>();
            manager.RequestMakeChoice(id, (int)EChoices.ROCK);

            _choiceText.Content = CHOICE_TEXT_PREPEND + EChoices.ROCK.ToPrettyString();
        }
    }

    public void OnBtnMage()
    {
        if (!_confirmed)
        {
            var networking = Locator.Instance.Get<INetworkingManager>();
            var id = networking.Id;
            var manager = Locator.Instance.Get<IRockPaperScissorsManager>();
            manager.RequestMakeChoice(id, (int)EChoices.PAPER);

            _choiceText.Content = CHOICE_TEXT_PREPEND + EChoices.PAPER.ToPrettyString();
        }
    }

    public void OnBtnAssassin()
    {
        if (!_confirmed)
        {
            var networking = Locator.Instance.Get<INetworkingManager>();
            var id = networking.Id;
            var manager = Locator.Instance.Get<IRockPaperScissorsManager>();
            manager.RequestMakeChoice(id, (int)EChoices.SCISSORS);

            _choiceText.Content = CHOICE_TEXT_PREPEND + EChoices.SCISSORS.ToPrettyString();
        }
    }

    public void OnBtnConfirm()
    {
        if (!_confirmed)
        {
            var networking = Locator.Instance.Get<INetworkingManager>();
            var id = networking.Id;
            var manager = Locator.Instance.Get<IRockPaperScissorsManager>();
            manager.RequestConfirmChoice(id);
            _confirmed = true; // TODO: Use server callback to confirm success (with ConfirmChoice method instead of _confirmed = true)
        }
    }

    public void OnRegister() { }
    public void OnOpen() { }
    public void OnClose() { }
}
