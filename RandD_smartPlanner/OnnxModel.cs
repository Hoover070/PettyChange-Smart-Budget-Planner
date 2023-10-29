using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OnnxModel
{
    private string modelPath;
    private InferenceSession session;

    public OnnxModel(string modelPath)
    {
        this.modelPath = modelPath;
        this.session = new InferenceSession(modelPath);
    
    }

    public float Predict(double income, double expenses, double savingsGoal, int timeframe)
    {
        // Convert your inputs into a tensor
        float[] inputData = new float[] { (float)income, (float)expenses, (float)savingsGoal, (float)timeframe };
        var input = new DenseTensor<float>(inputData, new[] { 1, inputData.Length });

        // Run prediction
        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input_name", input) }; // replace "input_name" with the actual input name defined in your model
        using (var results = session.Run(inputs))
        {
            // Fetch the results
            var resultTensor = results.First().AsTensor<float>();
            float result = resultTensor[0];
            return result;
        }
    }
}