namespace App.Shared.Models
{
    public sealed class CodeModel
    {
        public string Code {  get; set; }
        public int Status { get; set; }

        public CodeModel(string code, int status)
        {
            Code = code;
            Status = status;
        }
    }
}
