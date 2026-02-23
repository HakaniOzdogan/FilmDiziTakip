using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class DiziSezon : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int SezonNo { get; set; }
        public List<DiziBolum> Bolumler { get; set; } = new List<DiziBolum>();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

   

}
