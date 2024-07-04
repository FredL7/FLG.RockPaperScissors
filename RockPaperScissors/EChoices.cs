namespace RockPaperScissors {
    public enum EChoices {
        NONE = 0, ROCK = 1, PAPER = 2, SCISSORS = 3
    }

    public static class EChoicesExtension {
        public static int ToCommandValue(this EChoices choice) => (int)choice;
        public static EChoices FromCommandValue(this int choice) => (EChoices)choice;
        public static string ToPrettyString(this EChoices choice)
        {
            return choice switch
            {
                EChoices.NONE => "None",
                EChoices.ROCK => "Paladin (Rock)",
                EChoices.PAPER => "Mage (Paper)",
                EChoices.SCISSORS => "Assassin (Scissors)",
                _ => throw new ArgumentException($"Invalid choice {choice}")
            };
        }

        // 0=tie, 1=choice1 wins, -1=choice2 wins
        public static int Compare(EChoices choice1, EChoices choice2)
        {
            int difference = (int)choice1 - (int)choice2;
            if (difference == 0)
            {
                return 0;
            }
            if (Math.Abs(difference) == 1)
            {
                return difference;
            }
            else
            {
                return -Math.Sign(difference);
            }
        }
    }
}
