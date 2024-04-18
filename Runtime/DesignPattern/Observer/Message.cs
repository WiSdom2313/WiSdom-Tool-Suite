namespace WiSdom.DesignPattern
{
    // Example message class implementing the resettable interface
    public class ExampleMessage : IResettable
    {
        public int Score { get; set; }
        public string PlayerId { get; set; }

        public void Reset()
        {
            Score = 0;
            PlayerId = null;
        }
    }
}