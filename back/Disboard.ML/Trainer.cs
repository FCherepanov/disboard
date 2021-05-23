namespace Disboard.ML
{
    using Microsoft.ML;
    using Microsoft.ML.Data;
    using Microsoft.ML.Transforms.TimeSeries;
    using Model;

    class Trainer
    {
        private readonly MLContext _mlContext;
        public Trainer()
        {
            _mlContext = new MLContext();
        }

        public void TrainAndSave(int disId)
        {
            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "Forecasted",
                inputColumnName: "Input",
                windowSize: 14,
                seriesLength: 30,
                trainSize: 365,
                horizon: 7,
                confidenceLevel: 0.90f);

            var connectionString = $"Data Source=Host=localhost;Database=dis_data;Username=postgres;Password=1";
            string query = $"SELECT * as Input FROM data_in WHERE disId = {disId}";
            DatabaseSource dbSource = new DatabaseSource(Npgsql.NpgsqlFactory.Instance,
                connectionString,
                query);
            DatabaseLoader loader = _mlContext.Data.CreateDatabaseLoader<Input>();
            IDataView dataView = loader.Load(dbSource);
            IDataView data = _mlContext.Data.ShuffleRows(dataView);
            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(data);
            var forecastEngine = forecaster.CreateTimeSeriesEngine<Input, Output>(_mlContext);
            forecastEngine.CheckPoint(_mlContext, $"model{disId}.zip");
          
        }
    }
}
