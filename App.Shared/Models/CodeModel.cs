namespace App.Shared.Models
{
    public sealed class CodeModel
    {
        public string Code {  get; set; }

        public CodeModel(string code)
        {
            Code = code;
        }
    }
}
