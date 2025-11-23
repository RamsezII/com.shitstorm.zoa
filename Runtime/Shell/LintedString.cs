namespace _ZOA_
{
    public readonly struct LintedString
    {
        public readonly string text, lint;
        public LintedString(in string text, in string lint = null)
        {
            this.text = text;
            this.lint = lint ?? text;
        }
    }
}