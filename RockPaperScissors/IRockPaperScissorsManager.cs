using FLG.Cs.Datamodel;


namespace RockPaperScissors {
    public interface IRockPaperScissorsManager : IServiceInstance {
        public EChoices GetChoice(int playerId);
        public void MakeChoice(int playerId, EChoices choice);
        public void RequestMakeChoice(int playerId, int choice);
        public void RequestConfirmChoice(int playerId);
        public void DeclareWinner(int winnerId, bool tie, int player1Choice, int player2Choice);
    }
}
