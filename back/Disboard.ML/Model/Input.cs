using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disboard.ML.Model
{
    public class Input
    {
        // Ид субъекта (гипотеза что некоторые из событий специфичны для региона)
        public int region { get; set; }

        // Месяц
        public int month { get; set; }

        // Кол-во пожаров в субъекте на текущий момент
        public int fire { get; set; }

        // Кол-во пожаров в соседних субъектах на текущий момент
        public int near_fire { get; set; }

        // Кол-во осадков (мм)
        public int precipitation { get; set; }

        // Кол-во осадков (%, от нормы)
        public float precipitation_per { get; set; }

        // Кол-во температура воздуха
        public float temp { get; set; }

    }
}
