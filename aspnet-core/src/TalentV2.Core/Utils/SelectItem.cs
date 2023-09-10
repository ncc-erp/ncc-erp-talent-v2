namespace TalentV2.Utils
{
    public class SelectItem<TValue>
    {
        public SelectItem()
        {
        }

        public SelectItem(TValue value, string text)
        {
            Value = value;
            Text = text;
        }

        public TValue Value { get; set; }
        public string Text { get; set; }
        public string Ten { get; set; }
    }

    public class MaTenSelectItem : SelectItem<string>
    {
        public MaTenSelectItem()
        {
        }

        public MaTenSelectItem(string value, string ten)
        {
            Value = value;
            Ten = ten;
        }

        public new string Text => $"{Value} - {Ten}";
    }
}