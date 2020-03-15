using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    public class Field:TextBlock
    {
        public bool isFlag{ get; set; } = false; // На клетку установлен флаг        
        public bool isBomb = false; // Клетка является бомбой
        public int BombsNum { get; set; } = 0; // Число бомб вокруг
        
        public Field()
        {
            Width = 25;
            Height = 25;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0);
            Tag = 0;
            FontSize = 20;
            TextAlignment = TextAlignment.Center;
            FontFamily = new FontFamily("Arial Black");
            Background = Brushes.Silver;

        }

    }
}
