using FLG.Cs.Datamodel;

public class ResultpageClient : IPage {
    private const string PAGE_ID = "ResultpageClient";

    public string PageId { get => PAGE_ID; }
    public string LayoutId { get; set; } = "";

    private IText _text;

    public ResultpageClient(IUIFactory factory) {
        _text = factory.Text("result-text", "N/A", new(), new(alignHorizontal: ETextAlignHorizontal.CENTER, alignVertical: ETextAlignVertical.CENTER));
    }

    public void Setup(IUIManager ui, IUIFactory factory)
    {
        var layout = ui.GetLayout(LayoutId);
        var target = layout.GetTarget("content");
        target.AddChild(_text, PageId);
    }

    public void SetResultText(string resultText)
    {
        _text.Content = resultText;
    }

    public void OnRegister() { }
    public void OnOpen() { }
    public void OnClose() { }
}

