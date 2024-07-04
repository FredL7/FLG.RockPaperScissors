using FLG.Cs.Datamodel;
using FLG.Cs.Model;
using FLG.Cs.ServiceLocator;
using RockPaperScissors;
using System.ComponentModel;

namespace RPSClient {
    internal class RockPaperScissorsManager : IRockPaperScissorsManager {
        private EChoices _choice;

        #region IServiceInstance
        public void OnServiceRegisteredFail() { }
        public void OnServiceRegistered()
        {
            Locator.Instance.Get<ILogManager>().Debug("RockPaperScissors Manager (Client) Registered");
        }
        #endregion IServiceInstance

        public EChoices GetChoice(int _) => _choice;

        public void MakeChoice(int _, EChoices choice)
        {
            _choice = choice;
        }

        public void RequestMakeChoice(int playerId, int choice)
        {
            var request = new Command<IRockPaperScissorsManager>("RequestMakeChoice");
            request.AddParam(playerId);
            request.AddParam(choice);

            var networking = Locator.Instance.Get<INetworkingManager>();
            networking.SendCommand(request);
        }

        public void RequestConfirmChoice(int playerId)
        {
            var request = new Command<IRockPaperScissorsManager>("RequestConfirmChoice");
            request.AddParam(playerId);

            var networking = Locator.Instance.Get<INetworkingManager>();
            networking.SendCommand(request);
        }

        public void DeclareWinner(int winnerId, bool tie, int player1Choice, int player2Choice)
        {
            var logger = Locator.Instance.Get<ILogManager>();
            var networking = Locator.Instance.Get<INetworkingManager>();
            var id = networking.Id;

            EChoices p1Choice = (EChoices)player1Choice;
            EChoices p2Choice = (EChoices)player2Choice;

            EChoices playerChoice = id == 0 ? p1Choice : p2Choice;
            EChoices opponentChoice = id == 0 ? p2Choice : p1Choice;

            string result = "Game result: ";

            if (tie)
            {
                result += "Tie.";
            }
            else if (winnerId == id)
            {
                result += "Win.";
            }
            else
            {
                result += "Loss.";
            }

            result += $" Your choice: {playerChoice.ToPrettyString()}, opponent choice: {opponentChoice.ToPrettyString()}";
            logger.Info(result);

            // TODO: cast vs EventSystem vs Observer
            var ui = Locator.Instance.Get<IUIManager>();
            var page = (ResultpageClient)ui.GetPage("ResultpageClient");
            page.SetResultText(result);
            ui.SetCurrentPage("ResultpageClient");
        }
    }
}
