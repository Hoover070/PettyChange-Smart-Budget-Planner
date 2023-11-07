using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;




namespace RandD_smartPlanner
{
    public class OnnxModel
    {

        
        private static OnnxModel _instance;
        public InferenceSession Session { get; private set; }

        public OnnxModel() 
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public OnnxModel(string filePath)
        {
            var tempFilePath = Path.GetTempFileName();
            File.Copy(filePath, tempFilePath, true);

            // Load the ONNX model from the temporary file
            this.Session = new InferenceSession(tempFilePath);

            File.Delete(tempFilePath);
            
        }

        public float Predict(double income, double expenses, double savingsGoal, int timeframe, double minSavingsLimit, OnnxModel UserModel )
        {
            double incomeDiff = income - expenses;

            float[] inputData = new float[] { (float)income, (float)expenses, (float)savingsGoal, (float)timeframe, (float)incomeDiff, (float)minSavingsLimit };
            var input = new DenseTensor<float>(inputData,
                                               new[] { 1, inputData.Length });

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", input) }; 
            using (var results = UserModel.Session.Run(inputs))
            {
               
                var resultTensor = results.First().AsTensor<float>();
                float result = resultTensor[0];
                return result;
            }
        }

        public float UseAi(double Income, double Expenses, double SavingsGoal, int Timeframe, OnnxModel UserModel)
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe

            //Properties of the model
            var IncomeDifference = Income - Expenses;
            var MinimumSavingsPayment = SavingsGoal/Timeframe ;
            if (IncomeDifference < MinimumSavingsPayment)
            {
                MinimumSavingsPayment = IncomeDifference * .15;
            }
            else if (IncomeDifference > MinimumSavingsPayment)
            {
                MinimumSavingsPayment = IncomeDifference * .50;
            }

            // Use the model to make a prediction
            var prediction = UserModel.Predict(Income, Expenses, SavingsGoal, Timeframe, MinimumSavingsPayment, UserModel);

            // Do something with the prediction
            float AISuggestedSavings = prediction;

            return AISuggestedSavings;
        }


    }
}