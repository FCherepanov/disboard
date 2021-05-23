using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disboard.ML.Model
{
    class Output
    {
        // Вероятность события с прогнозированием на неделю вперед (7 вероятностей 0-10)
        public float[] Forecasted { get; set; }
    }
}
