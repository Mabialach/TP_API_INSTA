using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioTek.Models
{
    public class Livre
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }

}
