namespace App.Shared.Models
{
    public class TokenModel
    {
        public string Id { get; set; }
        public string UserInfo { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }

        public string Accesses { get; set; }
        public string Token { get; set; }
    }
}
