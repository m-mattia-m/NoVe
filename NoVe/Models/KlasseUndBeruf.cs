using System.Collections.Generic;

namespace NoVe.Models
{
    public class KlasseUndBeruf
    {

        public KlasseUndBeruf(Klasse klasse, List<Beruf> berufe)
        {
            this.klasse = klasse;
            this.berufe = berufe;
        }

        public Klasse klasse { get; set; }
        public List<Beruf> berufe { get; set; }
    }
}
