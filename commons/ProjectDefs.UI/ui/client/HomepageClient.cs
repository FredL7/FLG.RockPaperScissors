using FLG.Cs.Datamodel;
using FLG.Cs.Model;
using FLG.Cs.ServiceLocator;


public class HomepageClient : IPage {
    private const string PAGE_ID = "HomepageClient";
    private const string FORM_IP = "IP Adress";
    private const string FORM_PORT = "Port";

    public string PageId { get => PAGE_ID; }
    public string LayoutId { get; set; } = "";

    public void Setup(IUIManager ui, IUIFactory factory)
    {
        var formFields = new List<IInputField>()
        {
            factory.InputField("form-port", FORM_PORT, "0000", new SimpleIntegerModel(8066), new()),
            factory.InputField("form-ip", FORM_IP, "127.0.0.1", new NetworkingInputModel("127.0.0.1"), new())
        };
        var form = factory.Form("form", "Client Form", formFields, OnFormSubmit, new(width: 500), new());

        var layout = ui.GetLayout(LayoutId);
        var target = layout.GetTarget("content");
        target.AddChild(form, PageId);
    }

    private void OnFormSubmit(string name, IFormModel model)
    {
        var logger = Locator.Instance.Get<ILogManager>();
        string ip = model.GetItem(FORM_IP).GetValueAsString();
        int port = model.GetItem(FORM_PORT).GetValueAsInt();
        logger.Info($"Form {name} submitted with value: IP={ip} Port={port}.");

        var networking = Locator.Instance.Get<INetworkingManager>();
        networking.InitializeClient(ip, port);

        var ui = Locator.Instance.Get<IUIManager>();
        ui.SetCurrentPage("GamepageClient");
    }

    public void OnRegister() { }
    public void OnOpen() { }
    public void OnClose() { }
}
