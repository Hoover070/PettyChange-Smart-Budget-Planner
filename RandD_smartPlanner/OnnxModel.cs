using Intents;
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
        public float Predict(int UserIncome, int UserHousingExpense, int HouseholdSize, int UserPhoneBill, int CurrnetSavingsAmount, float CurrentEmergencyFund, int UserEntertainmentExpense, int UserFoodExpense, 
            int UserHealthInsuranceCost, int UserCarInsuranceCost, int UserRentInsuranceCost, int UserEducationCost, int UserLifeInsuranceCost, int UserFuelCost, int medianIncomeForCounty, 
            int AverageIncomePerArea, int MedianGrossRent, int Average1Bdr, int Average2Bdr, int Average3Bdr, int AverageTotalUtilityBills, int AveragePhoneBill, int AverageInternetBill, int AverageMinimumHouseholdSize, 
            int AverageMaximumHouseholdSize, int AverageEntertainmentExpense, int AverageFoodBill1Person, int AverageFoodBill4Poeple, int AverageFeulCost, int AverageCarInsurance, int AverageHealthInsuranceCost, int AverageEducationCost,
            float AverageChildcareCost, int AverageRentInsuranceCost, int AverageLifeInsuraceCost,
            OnnxModel UserModel)
        {

            float[] inputData = new float[] { (int)UserIncome, (int)UserHousingExpense, (int)HouseholdSize, (int)UserPhoneBill, (int)CurrnetSavingsAmount, (float)CurrentEmergencyFund, (int)UserEntertainmentExpense, 
            (int)UserFoodExpense, (int)UserHealthInsuranceCost, (int)UserCarInsuranceCost, (int)UserRentInsuranceCost, (int)UserEducationCost, (int)UserLifeInsuranceCost, (int)UserFuelCost, (int)medianIncomeForCounty, 
            (int)AverageIncomePerArea, (int)MedianGrossRent,(int)Average1Bdr, (int)Average2Bdr, (int)Average3Bdr, (int)AverageTotalUtilityBills, (int)AveragePhoneBill, (int)AverageInternetBill, (int)AverageMinimumHouseholdSize, (int)AverageMaximumHouseholdSize, (int)AverageEntertainmentExpense, 
            (int)AverageFoodBill1Person, (int)AverageFoodBill4Poeple, (int)AverageFeulCost,(int)AverageCarInsurance, (int)AverageHealthInsuranceCost, (int)AverageEducationCost, (float)AverageChildcareCost, (int)AverageRentInsuranceCost, (int)AverageLifeInsuraceCost};
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


        public float Predict_old(double income, double expenses, double savingsGoal, int timeframe, double minSavingsLimit, OnnxModel UserModel )
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

        public float UseAi(int UserIncome, int UserHousingExpense, int HouseholdSize, int UserPhoneBill, int CurrnetSavingsAmount, float CurrentEmergencyFund, int UserEntertainmentExpense, int UserFoodExpense,
            int UserHealthInsuranceCost, int UserCarInsuranceCost, int UserRentInsuranceCost, int UserEducationCost, int UserLifeInsuranceCost, int UserFuelCost, int medianIncomeForCounty,
            int AverageIncomePerArea, int MedianGrossRent, int Average1Bdr, int Average2Bdr, int Average3Bdr, int AverageTotalUtilityBills, int AveragePhoneBill, int AverageInternetBill, int AverageMinimumHouseholdSize,
            int AverageMaximumHouseholdSize, int AverageEntertainmentExpense, int AverageFoodBill1Person, int AverageFoodBill4Poeple, int AverageFeulCost, int AverageCarInsurance, int AverageHealthInsuranceCost, int AverageEducationCost,
            float AverageChildcareCost, int AverageRentInsuranceCost, int AverageLifeInsuraceCost,
            OnnxModel UserModel)
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe

            //Properties of the model
            var expenses = UserHousingExpense + UserPhoneBill + UserEntertainmentExpense + UserFoodExpense + UserHealthInsuranceCost + UserCarInsuranceCost + UserRentInsuranceCost + UserEducationCost + UserLifeInsuranceCost + UserFuelCost;
            var IncomeDifference = UserIncome - expenses;
            var MinimumSavingsPayment = UserIncome * .10 ;
            if (IncomeDifference < MinimumSavingsPayment)
            {
                MinimumSavingsPayment = IncomeDifference * .50;
            }
            else if (IncomeDifference > MinimumSavingsPayment)
            {
                MinimumSavingsPayment = IncomeDifference * .10;
            }

            // Use the model to make a prediction
            var prediction = UserModel.Predict( UserIncome,  UserHousingExpense,  HouseholdSize,  UserPhoneBill,  CurrnetSavingsAmount,  CurrentEmergencyFund,  UserEntertainmentExpense,  UserFoodExpense,
             UserHealthInsuranceCost,  UserCarInsuranceCost,  UserRentInsuranceCost,  UserEducationCost,  UserLifeInsuranceCost,  UserFuelCost,  medianIncomeForCounty,
             AverageIncomePerArea,  MedianGrossRent,  Average1Bdr,  Average2Bdr,  Average3Bdr,  AverageTotalUtilityBills,  AveragePhoneBill,  AverageInternetBill,  AverageMinimumHouseholdSize,
             AverageMaximumHouseholdSize,  AverageEntertainmentExpense,  AverageFoodBill1Person,  AverageFoodBill4Poeple,  AverageFeulCost,  AverageCarInsurance,  AverageHealthInsuranceCost,  AverageEducationCost,
             AverageChildcareCost,  AverageRentInsuranceCost,  AverageLifeInsuraceCost,
             UserModel);

            // Do something with the prediction
            float AISuggestedSavings = prediction;

            return AISuggestedSavings;
        }


    }
}