using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioTek.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; } = "User";
        public List<Favori> Favoris { get; set; }
    }
}
