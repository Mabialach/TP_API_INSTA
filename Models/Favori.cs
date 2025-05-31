using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioTek.Models
{
    public class Favori
    {
        public int Id { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }

        public int LivreId { get; set; }
        public Livre Livre { get; set; }
    }

}
