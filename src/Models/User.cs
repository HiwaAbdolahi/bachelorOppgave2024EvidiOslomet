using System;

namespace bachelorOppgave13.Models
{
    public class User
    {
        public int Id { get; set; } // Dette er den primære nøkkelen

        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        
    }
}
