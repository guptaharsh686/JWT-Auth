namespace FormulaOneApp.Models
{
    public class AuthResult // To reply for an authentication request if sucessful provide Token if not provide errors
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }
}
