namespace Sprache.Union
{

    public class UnionResultValue<T>
    {
        public T Value { get; set; }

        public string ParserName { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public bool WasSuccessful { get; set; }

        public IInput Reminder { get; set; }
    }
}