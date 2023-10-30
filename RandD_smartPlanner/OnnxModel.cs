using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Reflection;


namespace RandD_smartPlanner
{
    public class OnnxModel
    {

        private InferenceSession session;

        public OnnxModel()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "RandD_smartPlanner.trained_model.best_gb_model_15.onnx"; 

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                // Copy the embedded resource to a temporary file
                var tempFilePath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                // Load the ONNX model from the temporary file
                this.session = new InferenceSession(tempFilePath);

                File.Delete(tempFilePath);
            }

        }

        public float Predict(double income, double expenses, double savingsGoal, int timeframe, double incomeDiff, double minSavingsLimit )
        {
            // Convert your inputs into a tensor
            float[] inputData = new float[] { (float)income, (float)expenses, (float)savingsGoal, (float)timeframe, (float)incomeDiff, (float)minSavingsLimit };
            var input = new DenseTensor<float>(inputData, new[] { 1, inputData.Length });

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", input) }; // replace "input_name" with the actual input name defined in your model
            using (var results = session.Run(inputs))
            {
               
                var resultTensor = results.First().AsTensor<float>();
                float result = resultTensor[0];
                return result;
            }
        }

        public float UseAi(double Income, double Expenses, double SavingsGoal, int Timeframe, double IncomeDiff, double MinSavingsLimit )
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe
            OnnxModel model = new OnnxModel();

            // Use the model to make a prediction
            float prediction = model.Predict(Income, Expenses, SavingsGoal, Timeframe, IncomeDiff, MinSavingsLimit);

            // Do something with the prediction
            float AISuggestedSavings = prediction;
            



            return AISuggestedSavings;
        }
    }
}