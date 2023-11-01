using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itProgerWPF.Models
{
    // Описываем модель для товаров (Id, название, краткое описание, полное описание, цена и фото)
    internal class Item
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string BriefText { get; set; }

        public string FullText { get; set; }

        public float Price { get; set; }

        public string Image { get; set; }

        public string DisplayImage
        {
            get { return "/Images/" + Image; }
        }
        public Item()
        {
        }

        public Item(string title, string briefText, string fullText, float price, string image)
        {
            Title = title;
            BriefText = briefText;
            FullText = fullText;
            Price = price;
            Image = image;
        }
    }
}
