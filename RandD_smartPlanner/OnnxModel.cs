using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Reflection;
using System.IO;
using System;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;



namespace RandD_smartPlanner
{
    public class OnnxModel
    {

        private InferenceSession session;
        private static OnnxModel _instance;

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
            this.session = new InferenceSession(tempFilePath);

            

            File.Delete(tempFilePath);

        }

        public float Predict(double income, double expenses, double savingsGoal, int timeframe, double incomeDiff, double minSavingsLimit, OnnxModel UserModel )
        {
            // Convert your inputs into a tensor
            float[] inputData = new float[] { (float)income, (float)expenses, (float)savingsGoal, (float)timeframe, (float)incomeDiff, (float)minSavingsLimit };
            var input = new DenseTensor<float>(inputData, new[] { 1, inputData.Length });

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", input) }; // replace "input_name" with the actual input name defined in your model
            using (var results = UserModel.session.Run(inputs))
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

            // Use the model to make a prediction
            var prediction = UserModel.Predict(Income, Expenses, SavingsGoal, Timeframe, IncomeDifference, MinimumSavingsPayment, UserModel);

            // Do something with the prediction
            float AISuggestedSavings = prediction;
            
            return AISuggestedSavings;
        }
    }
}