namespace FilmDiziTakipApi.Models
{
    public class IzlemeDetayModel
    {
        public int DiziId { get; set; }
        public int BolumId { get; set; }
        public string DiziAdi { get; set; } = string.Empty;
        public int SezonNo { get; set; } 
        public int BolumNo { get; set; } 
        public double KalanSure { get; set; }
        public string AfisUrl { get; set; } = string.Empty;
        public string KalanSureFormatted
        {
            get
            {
                int totalSeconds = (int)KalanSure;
                int hours = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;
                int seconds = totalSeconds % 60;

                if (hours > 0)
                    return $"{hours} sa {minutes} dk {seconds} sn";
                else if (minutes > 0)
                    return $"{minutes} dk {seconds} sn";
                else
                    return $"{seconds} sn";
            }
        }
    }

}
