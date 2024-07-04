using FLG.Cs.Datamodel;
using FLG.Cs.ServiceLocator;

using RockPaperScissors;


public class GamepageServer : IPage {
    private const string PAGE_ID = "GamepageServer";

    private const string NO_CHOICE = "WAITING ...";

    public string PageId { get => PAGE_ID; }
    public string LayoutId { get; set; } = "";

    IText _portInfo;
    private IText[] _playerTexts;

    public GamepageServer(IUIFactory factory)
    {
        _portInfo = factory.Text("port-info", "", new(height: 30, margin: new(0, 0, 0, 10)), new());
        var player1text = factory.Text("choice-player1", "Player1: " + NO_CHOICE, new(height: 30, margin: new(0, 0, 0, 10)), new());
        var player2text = factory.Text("choice-player2", "Player2: " + NO_CHOICE, new(height: 30), new());

        _playerTexts = new IText[] {
            player1text, player2text
        };
    }

    public void Setup(IUIManager ui, IUIFactory factory)
    {
        var layout = ui.GetLayout(LayoutId);
        var target = layout.GetTarget("content");
        target.AddChild(_portInfo, PageId);
        foreach (var player in _playerTexts)
        {
            target.AddChild(player, PageId);
        }
    }

    public void OnRegister() { }
    public void OnOpen()
    {
        var networking = Locator.Instance.Get<INetworkingManager>();
        var port = networking.ServerPort;
        _portInfo.Content = "Port: " + port.ToString();
    }
    public void OnClose() { }

    public void SetPlayerChoice(int playerId, EChoices choice, bool confirmed)
    {
        _playerTexts[playerId].Content = $"Player{playerId + 1}: {choice.ToPrettyString()} (confirmed={confirmed})";
    }
}
