
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Diagnostics;

namespace RandD_smartPlanner
{
    public class OnnxModel
    {

        
        private static OnnxModel _instance;
        public InferenceSession Session { get; private set; }
        private static string _modelPath = "trained_model.best_gb_model_15.onnx";
        private static OnnxModel _userModel;

        public OnnxModel() 
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public OnnxModel(string filePath)
        {
            try
            {
                var tempFilePath = Path.GetTempFileName();
                File.Copy(filePath, tempFilePath, true);
                this.Session = new InferenceSession(tempFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while creating InferenceSession: {ex.Message}");
                throw; // Re-throw the exception to handle it further up the call stack.
            }

        }
        public double Predict(OnnxModel UserModel, double UserIncome = 0, double UserHousingExpense = 0, double UserPhoneBill=0, double CurrnetSavingsAmount = 0, double CurrentEmergencyFund = 0, double UserEntertainmentExpense = 0, double UserFoodExpense =  0,
            double UserHealthInsuranceCost = 0, double UserCarInsuranceCost = 0, double UserRentInsuranceCost = 0, double UserEducationCost = 0, double UserLifeInsuranceCost = 0, double UserFuelCost = 0, double HouseholdSize = 1, double medianIncomeForCounty = 3478,
            double AverageIncomePerArea = 4690, double MedianGrossRent = 2150, double Average1Bdr = 1300, double Average2Bdr = 2800, double Average3Bdr = 3000, double AverageTotalUtilityBills = 258, double AveragePhoneBill = 49, double AverageInternetBill = 61, double AverageMinimumHouseholdSize = 1,
            double AverageMaximumHouseholdSize = 4, double AverageEntertainmentExpense = 418, double AverageFoodBill1Person = 528, double AverageFoodBill4Poeple = 1385, double AverageFeulCost = 200, double AverageCarInsurance = 212, double AverageHealthInsuranceCost = 1465.21, double AverageEducationCost = 1316,
            double AverageChildcareCost = 671.61, double AverageRentInsuranceCost = 18, double AverageLifeInsuraceCost = 26)
        {

            float[] inputData = new float[] { (float)UserIncome, (float)UserHousingExpense, (float)HouseholdSize, (float)UserPhoneBill, (float)CurrnetSavingsAmount, (float)CurrentEmergencyFund, (float)UserEntertainmentExpense, 
            (float)UserFoodExpense, (float)UserHealthInsuranceCost, (float)UserCarInsuranceCost, (float)UserRentInsuranceCost, (float)UserEducationCost, (float)UserLifeInsuranceCost, (float)UserFuelCost, (float)medianIncomeForCounty, 
            (float)AverageIncomePerArea, (float)MedianGrossRent,(float)Average1Bdr, (float)Average2Bdr, (float)Average3Bdr, (float)AverageTotalUtilityBills, (float)AveragePhoneBill, (float)AverageInternetBill, (float)AverageMinimumHouseholdSize, (float)AverageMaximumHouseholdSize, (float)AverageEntertainmentExpense, 
            (float)AverageFoodBill1Person, (float)AverageFoodBill4Poeple, (float)AverageFeulCost,(float)AverageCarInsurance, (float)AverageHealthInsuranceCost, (float)AverageEducationCost, (float)AverageChildcareCost, (float)AverageRentInsuranceCost, (float)AverageLifeInsuraceCost};
           
            var input = new DenseTensor<float>(inputData,new[] { 1, inputData.Length });
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

        public double UseAi(OnnxModel UserModel, double UserIncome = 0, double UserHousingExpense = 0, double UserPhoneBill = 0, double CurrnetSavingsAmount = 0, double CurrentEmergencyFund = 0, double UserEntertainmentExpense = 0, double UserFoodExpense = 0,
            double UserHealthInsuranceCost = 0, double UserCarInsuranceCost = 0, double UserRentInsuranceCost = 0, double UserEducationCost = 0, double UserLifeInsuranceCost = 0, double UserFuelCost = 0, double HouseholdSize = 1, double medianIncomeForCounty = 3478,
            double AverageIncomePerArea = 4690, double MedianGrossRent = 2150, double Average1Bdr = 1300, double Average2Bdr = 2800, double Average3Bdr = 3000, double AverageTotalUtilityBills = 258, double AveragePhoneBill = 49, double AverageInternetBill = 61, double AverageMinimumHouseholdSize = 1,
            double AverageMaximumHouseholdSize = 4, double AverageEntertainmentExpense = 418, double AverageFoodBill1Person = 528, double AverageFoodBill4Poeple = 1385, double AverageFeulCost = 200, double AverageCarInsurance = 212, double AverageHealthInsuranceCost = 1465.21, double AverageEducationCost = 1316,
            double AverageChildcareCost = 671.61, double AverageRentInsuranceCost = 18, double AverageLifeInsuraceCost = 26)
        {

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
            var prediction = UserModel.Predict(UserModel, UserIncome,  UserHousingExpense,  HouseholdSize,  UserPhoneBill,  CurrnetSavingsAmount,  CurrentEmergencyFund,  UserEntertainmentExpense,  UserFoodExpense,
             UserHealthInsuranceCost,  UserCarInsuranceCost,  UserRentInsuranceCost,  UserEducationCost,  UserLifeInsuranceCost,  UserFuelCost,  medianIncomeForCounty,
             AverageIncomePerArea,  MedianGrossRent,  Average1Bdr,  Average2Bdr,  Average3Bdr,  AverageTotalUtilityBills,  AveragePhoneBill,  AverageInternetBill,  AverageMinimumHouseholdSize,
             AverageMaximumHouseholdSize,  AverageEntertainmentExpense,  AverageFoodBill1Person,  AverageFoodBill4Poeple,  AverageFeulCost,  AverageCarInsurance,  AverageHealthInsuranceCost,  AverageEducationCost,
             AverageChildcareCost,  AverageRentInsuranceCost,  AverageLifeInsuraceCost);

            
            double AISuggestedSavings = prediction;

            return AISuggestedSavings;
        }
  

    }

    
}