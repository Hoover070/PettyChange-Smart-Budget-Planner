import random
import pandas as pd

# Function to generate random data based on the provided criteria. I have attempted to make the data as realistic as
# possible. The data is based on the zip code 34747 in Florida, USA. The data is based on the year 2023.
def generate_random_data(num_records):
    data = []
    housing_choice = None
    household_size = None
    user_income = None
    user_housing_expense = None
    user_phone_bill = None
    user_entertainment_expense = None
    user_food_expense = None
    user_health_insurance = None
    user_car_insurance_cost = None
    user_rent_insurance_cost = None
    user_fuel_cost = None

    def calculate_ai_savings(user_income, current_emergency_fund, total_expenses):
        # User should save 10% of their income each month.
        # If emergency fund is less than 6 months of expenses, split the savings.
        total_savings_rate = user_income * 0.10
        emergency_fund_goal = total_expenses * 6

        if current_emergency_fund < emergency_fund_goal:
            # Split savings between emergency fund and general savings
            half_savings = total_savings_rate / 2
            return half_savings
        else:
            # All savings go into general savings
            return total_savings_rate

    def calculate_ai_emergency_deposit(user_income, current_emergency_fund, total_expenses):
        # User should continue contributing to the emergency fund until it reaches 6 months of expenses.
        total_savings_goal = user_income * 0.10
        emergency_fund_goal = total_expenses * 6

        if current_emergency_fund < emergency_fund_goal:
            # Split savings between emergency fund and general savings
            half_savings = total_savings_goal / 2
            return half_savings
        else:
            # No more contributions to the emergency fund needed
            return 0


    for _ in range(num_records):
        # Randomly generate each field based on the specified criteria
        user_income = random.randint(1500, 6500)
        if user_income < 2000:
            housing_choice = random.choice(['studio', '1bdr'])
        elif user_income < 2000 & user_income >= 3000:
            housing_choice = random.choice(['studio', '1bdr', '2bdr'])
        elif user_income < 3001:
            housing_choice = random.choice(['1bdr', '2bdr', '3bdr'])

        else:
            housing_choice = random.choice(['studio', '1bdr', '2bdr', '3bdr'])


        housing_choice = random.choice(['studio', '1bdr', '2bdr', '3bdr'])
        if housing_choice == 'studio':
            user_housing_expense = random.randint(800, 984)
        elif housing_choice == '1bdr':
            user_housing_expense = random.randint(1100, 1450)
        elif housing_choice == '2bdr':
            user_housing_expense = random.randint(1550, 1950)
        else:
            user_housing_expense = random.randint(2000, 3000)


        # user Generated info
        household_size = random.randint(1, 4)
        user_phone_bill = random.randint(46, 96) * household_size
        user_entertainment_expense = random.randint(0, 1500)
        user_food_expense = random.randint(450, 1300)
        user_health_insurance = random.randint(0, 420) * household_size
        user_car_insurance_cost = random.randint(0, 180)
        user_rent_insurance_cost = random.randint(0, 36)
        user_fuel_cost = random.randint(0, 120)

        #Supplied via API from Zip. All info based accurately on zip 34747
        median_income_for_county = 3478
        average_income_for_county = 4690
        median_gross_rent = 2150
        average_1Bdr = 1300
        average_2Bdr = 2100
        average_3Bdr = 3000
        average_total_utility_bills = 258
        average_phone_bill = 49
        average_internet_bill = 61
        average_minimum_household_size = 1
        average_maximum_household_size = 4
        average_entertainment_expense = 418
        average_foodbill_1_person = 524
        average_foodbill_4_people = 1385
        average_fuel_cost = 200
        average_car_insurance = 212
        average_health_insurance = 1465
        average_childcare_cost = 671.67
        average_education_cost = 1316
        average_rent_insurance_cost = 18
        average_life_insurance_cost = 26


        # Distribution for current savings account
        savings_distribution = [0] * 55 + list(range(0, 1501)) * 25 + list(range(1600, 6001)) * 15 + [6000] * 5
        current_savings_amount = random.choice(savings_distribution)

        # Distribution for current emergency fund
        emergency_fund_distribution = [0] * 75 + [0.25 * user_income] * 15 + [0.75 * user_income] * 5 + [
            user_income] * 5
        current_emergency_fund = random.choice(emergency_fund_distribution)

        # Distribution for user education cost
        if random.random() < 0.75:
            user_education_cost = 0
        else:
            user_education_cost = random.randint(300, 650)

        user_life_insurance_cost = random.randint(12, 36)

        total_expenses = (user_housing_expense + user_phone_bill + user_entertainment_expense +
                          user_food_expense + user_health_insurance + user_car_insurance_cost +
                          user_rent_insurance_cost + user_education_cost + user_life_insurance_cost)

        # Determine GoNegative
        go_negative = (total_expenses <= user_income + 200) and (total_expenses >= user_income - 100)

        # Placeholder values for AI suggested deposits (to be filled by the model)
        ai_suggested_savings_deposit = None
        ai_suggested_emergency_deposit = None

        # Calculate AI suggested deposits if the user is not expected to go negative
        if not go_negative:
            ai_suggested_emergency_deposit = calculate_ai_emergency_deposit(user_income, current_emergency_fund,
                                                                            total_expenses)
            ai_suggested_savings_deposit = calculate_ai_savings(user_income,
                                                                current_emergency_fund, total_expenses)

        if go_negative:
            ai_suggested_emergency_deposit = 0
            ai_suggested_savings_deposit = 0



        # Create a dictionary of the generated data with a header that names each field
        record = {



            'UserIncome': user_income,
            'UserHousingExpense': user_housing_expense,
            'HouseholdSize': household_size,
            'UserPhoneBill': user_phone_bill,
            'CurrentSavingsAmount': current_savings_amount,
            'CurrentEmergencyFund': current_emergency_fund,
            'UserEntertainmentExpense': user_entertainment_expense,
            'UserFoodExpense': user_food_expense,
            'UserHealthInsuranceCost': user_health_insurance,
            'UserCarInsuranceCost': user_car_insurance_cost,
            'UserRentInsuranceCost': user_rent_insurance_cost,
            'UserEducationCost': user_education_cost,
            'UserLifeInsuranceCost': user_life_insurance_cost,
            'GoNegative': go_negative,
            'AISuggestedSavingsDeposit': ai_suggested_savings_deposit,
            'AISuggestedEmergencyDeposit': ai_suggested_emergency_deposit,
            'UserFuelCost': user_fuel_cost,


            # Zip Code generated. All info for zip code 34747 accurate 11/10/2023
            'medianIncomeForCounty': median_income_for_county,
            'AverageIncomeForArea': average_income_for_county,
            'MedianGrossRent': median_gross_rent,
            'Average1Bdr': average_1Bdr,
            'Average2Bdr': average_2Bdr,
            'Average3Bdr': average_3Bdr,
            'AverageTotalUtilityBills': average_total_utility_bills,
            'AveragePhoneBill': average_phone_bill,
            'AverageInternetBill': average_internet_bill,
            'AverageMinimumHouseholdSize': average_minimum_household_size,
            'AverageMaximumHouseholdSize': average_maximum_household_size,
            'AverageEntertainmentExpense': average_entertainment_expense,
            'AverageFoodBill1Person': average_foodbill_1_person,
            'AverageFoodBill4People': average_foodbill_4_people,
            'AverageFuelCost': average_fuel_cost,
            'AverageCarInsurance': average_car_insurance,
            'AverageHealthInsuranceCost': average_health_insurance,
            'AverageEducationCost': average_education_cost,
            'AverageChildcareCost': average_childcare_cost,
            'AverageRentInsuranceCost': average_rent_insurance_cost,
            'AverageLifeInsuranceCost': average_life_insurance_cost,



        }

        data.append(record)

    return data


# Generate 100 records
random_data = generate_random_data(10000)

# Convert the list of dictionaries to a DataFrame
random_data_df = pd.DataFrame(random_data)

# Display the first few rows of the generated data
random_data_df.head()

# Save the generated data to a CSV file and add it to the already existing dataset
random_data_df.to_csv('training_Data.csv', mode='a', header=True, index=False)


