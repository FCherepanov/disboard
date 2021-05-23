namespace Disboard.ML
{
    using System.IO;
    using Microsoft.ML;
    using Microsoft.ML.Transforms.TimeSeries;
    using Model;

    public class Forecaster
    {
        private readonly TimeSeriesPredictionEngine<Input, Output> _forecaster;

        public Forecaster(string modelFile, int disasterId)
        {
            DisasterId = disasterId;
            var ml = new MLContext();
            using var stream = new FileStream(modelFile, FileMode.Open);
            var model = ml.Model.Load(stream, out _);
             _forecaster = model.CreateTimeSeriesEngine<Input, Output>(ml);
        }

        public int DisasterId { get; }

        public float[] Forecast(int dt, int regionId)
        {
            var forecast = _forecaster.Predict(dt);
            return forecast.Forecasted;
        }

    }
}
