using FLG.Cs.Datamodel;
using FLG.Cs.Model;
using FLG.Cs.ServiceLocator;
using RockPaperScissors;

namespace RPSServer {
    internal class RockPaperScissorsManager : IRockPaperScissorsManager {
        private EChoices[] _choices;
        private bool[] _confirmed;

        public RockPaperScissorsManager()
        {
            _choices = new EChoices[2];
            _confirmed = new bool[2];
        }

        #region IServiceInstance
        public void OnServiceRegisteredFail() { }
        public void OnServiceRegistered()
        {
            Locator.Instance.Get<ILogManager>().Debug("RockPaperScissors Manager (Server) Registered");
        }
        #endregion IServiceInstance

        public EChoices GetChoice(int playerId) => _choices[playerId];

        public void MakeChoice(int playerId, EChoices choice)
        {
            if (!_confirmed[playerId])
            {
                _choices[playerId] = choice;
                NotifyUI(playerId);
            }
        }

        public void RequestMakeChoice(int playerId, int choice)
        {
            MakeChoice(playerId, choice.FromCommandValue());
        }

        public void RequestConfirmChoice(int playerId)
        {
            _confirmed[playerId] = true;
            NotifyUI(playerId);
            if (_confirmed[0] && _confirmed[1])
            {
                var logger = Locator.Instance.Get<ILogManager>();
                logger.Info($"All player confirmed: Player1={_choices[0].ToPrettyString()}, Player2={_choices[1].ToPrettyString()}");
                DeclareWinner();
            }
        }

        public void DeclareWinner(int winnerId, bool tie, int player1Choice, int player2Choice)
        {
            var command = new Command<IRockPaperScissorsManager>("DeclareWinner");
            command.AddParam(winnerId);
            command.AddParam(tie);
            command.AddParam(player1Choice);
            command.AddParam(player2Choice);

            var networking = Locator.Instance.Get<INetworkingManager>();
            networking.SendServerCommandToAll(command);
        }

        private void DeclareWinner()
        {
            int result = EChoicesExtension.Compare(_choices[0], _choices[1]);
            if (result == 0)
            {
                DeclareWinner(-1, true, (int)_choices[0], (int)_choices[1]);
            }
            else if (result > 0)
            {
                DeclareWinner(0, false, (int)_choices[0], (int)_choices[1]);
            }
            else
            {
                DeclareWinner(1, false, (int)_choices[0], (int)_choices[1]);
            }
        }

        private void NotifyUI(int playerId)
        {
            // TODO: cast vs EventSystem vs Observer
            var ui = Locator.Instance.Get<IUIManager>();
            var page = (GamepageServer)ui.GetPage("GamepageServer");
            page.SetPlayerChoice(playerId, _choices[playerId], _confirmed[playerId]);
        }
    }
}
