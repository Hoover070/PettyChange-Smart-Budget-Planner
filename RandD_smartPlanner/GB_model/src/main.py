import onnx
from sklearn.ensemble import GradientBoostingRegressor
from sklearn.model_selection import GridSearchCV, train_test_split, learning_curve
from sklearn.metrics import mean_squared_error, mean_absolute_error, r2_score, explained_variance_score
import pandas as pd
import joblib
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType
import numpy as np
import matplotlib.pyplot as plt
import os
from skopt import BayesSearchCV
from skopt.space import Real, Integer
import json
import logging

# Initialize logging
logging.basicConfig(filename='model_training.log', level=logging.INFO,
                    format='%(asctime)s:%(levelname)s:%(message)s')

def load_config(config_path="data/config.json"):
    try:
        with open(config_path) as config_file:
            return json.load(config_file)
    except FileNotFoundError:
        logging.error(f"Configuration file {config_path} not found.")
        return {}

def save_plots(model, X_train, y_train, X_test, y_test, output_path):
    # Learning Curve
    train_sizes, train_scores, test_scores = learning_curve(
        model, X_train, y_train, cv=3, scoring='neg_mean_squared_error', n_jobs=-1,
        train_sizes=np.linspace(0.1, 1.0, 5))
    train_scores_mean = -np.mean(train_scores, axis=1)
    test_scores_mean = -np.mean(test_scores, axis=1)

    plt.figure()
    plt.plot(train_sizes, train_scores_mean, 'o-', color="r", label="Training score")
    plt.plot(train_sizes, test_scores_mean, 'o-', color="g", label="Cross-validation score")
    plt.title("Learning Curve")
    plt.xlabel("Training examples")
    plt.ylabel("MSE")
    plt.legend(loc="best")
    learning_curve_path = os.path.join(output_path, "learning_curve.png")
    plt.savefig(learning_curve_path)
    logging.info(f"Learning curve saved to {learning_curve_path}")

    # Save learning curve plot
    learning_curve_path = os.path.join(output_path, "learning_curve.png")
    plt.savefig(learning_curve_path)
    plt.close()  # Close the plot to avoid display issues in some environments
    logging.info(f"Learning curve saved to {learning_curve_path}")

    # Residual Plot
    y_pred_train = model.predict(X_train)
    y_pred_test = model.predict(X_test)

    plt.figure()
    plt.scatter(y_pred_train, y_pred_train - y_train, color="blue", s=10, label="Training data")
    plt.scatter(y_pred_test, y_pred_test - y_test, color="green", s=10, label="Test data")
    plt.hlines(y=0, xmin=min(y_pred_train.min(), y_pred_test.min()), xmax=max(y_pred_train.max(), y_pred_test.max()),
               colors="red")
    plt.title("Residual Error Plot")
    plt.xlabel("Predicted Values")
    plt.ylabel("Residuals")
    plt.legend(loc="upper right")
    residual_plot_path = get_unique_filename(output_path, "residual_plot", "png")
    plt.savefig(residual_plot_path)
    logging.info(f"Residual plot saved to {residual_plot_path}")

    # Save residual plot
    residual_plot_path = get_unique_filename(output_path, "residual_plot", "png")
    plt.savefig(residual_plot_path)
    plt.close()  # Close the plot to avoid display issues in some environments
    logging.info(f"Residual plot saved to {residual_plot_path}")+9


def get_unique_filename(directory, filename, extension):
    counter = 1
    unique_filename = f"{filename}_{counter}.{extension}"
    while os.path.exists(os.path.join(directory, unique_filename)):
        counter += 1
        unique_filename = f"{filename}_{counter}.{extension}"
    return unique_filename


def evaluate_model(model, X_test, y_test):
    # Calculate and log different evaluation metrics
    mse = mean_squared_error(y_test, model.predict(X_test))
    mae = mean_absolute_error(y_test, model.predict(X_test))
    rmse = np.sqrt(mse)
    r2 = r2_score(y_test, model.predict(X_test))
    explained_var = explained_variance_score(y_test, model.predict(X_test))

    logging.info(f"Evaluation Metrics:\n"
                 f"Mean Squared Error: {mse}\n"
                 f"Mean Absolute Error: {mae}\n"
                 f"Root Mean Squared Error: {rmse}\n"
                 f"R-Squared: {r2}\n"
                 f"Explained Variance Score: {explained_var}")


if __name__ == '__main__':

    # Load the dataset
    config = load_config()
    # data_path = os.path.join("..", config.get("data_path", "training_Data.csv"))
    output_path = os.path.join(config.get("output_path", "output"))

    data_path = os.path.join("data", "training_Data.csv")
    os.makedirs(output_path, exist_ok=True)
    print(data_path)
    print(output_path)
    print(os.getcwd())

    train_ai_savings_deposit = True
    train_is_negative = False
    train_ai_savings_emergency = False

    # Load the dataset
    try:
        data = pd.read_csv(data_path)
        print(data.head())
    except FileNotFoundError:
        logging.error(f"Data file {data_path} not found.")
        exit()

    print(data.info())

    if train_ai_savings_deposit is True:
        y = data['AISuggestedSavingsDeposit']
        X = data.drop(columns=['AISuggestedSavingsDeposit', 'AISuggestedEmergencyDeposit', 'GoNegative'])

        # convert all data to float
        X = X.astype(float)
        y = y.astype(float)

        # X['GoNegative'] = X['GoNegative'].astype(int)

        # print(X.info())
        # print(y.info())

        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

        # Fit the model to Gradiant Boosting algorithm
        gbm = GradientBoostingRegressor(random_state=42)
        param_grid_on = False
        bey_grid_on = config.get('bayes_search', True)

        if bey_grid_on:
            # Define the search space for hyperparameters
            search_spaces = {
                'n_estimators': Integer(50, 350),
                'learning_rate': Real(0.005, 0.2, prior='log-uniform'),
                'max_depth': Integer(3, 10),
                'min_samples_split': Integer(2, 5),
                'min_samples_leaf': Integer(1, 5),
            }

            # Bayesian optimization
            bayes_search = BayesSearchCV(
                estimator=gbm,
                search_spaces=search_spaces,
                n_iter=config['n_iter'],
                scoring='neg_mean_squared_error',
                n_jobs=-1,
                cv=config['cv']
            )

            # Perform the search
            bayes_search.fit(X_train, y_train)

            if bayes_search.best_estimator_:  # if the search was successful
                best_model = bayes_search.best_estimator_

                # Saving the model
                model_path = os.path.join(output_path, 'AiSavingsModel_Bayes.pkl')
                joblib.dump(best_model, model_path)
                logging.info(f"Best model saved to {model_path}")

                # Saving the parameters
                params_path = os.path.join(output_path, 'AiSavingModelParams_Bayes.txt')
                with open(params_path, 'w') as f:
                    f.write(str(bayes_search.best_params_))
                logging.info(f"Best parameters saved to {params_path}")

                # Saving the search results
                results_path = os.path.join(output_path, 'bayes_search_results.csv')
                bayes_results = pd.DataFrame(bayes_search.cv_results_)
                bayes_results.to_csv(results_path, index=False)
                logging.info(f"Search results saved to {results_path}")

                # Evaluate the model
                evaluate_model(best_model, X_test, y_test)
                y_pred_bayes = best_model.predict(X_test)

                # Logging evaluation metrics
                mse_bayes = mean_squared_error(y_test, y_pred_bayes)
                mae_bayes = mean_absolute_error(y_test, y_pred_bayes)
                rmse_bayes = np.sqrt(mse_bayes)
                r2_bayes = r2_score(y_test, y_pred_bayes)
                explained_variance = explained_variance_score(y_test, y_pred_bayes)

                logging.info(f"Mean Squared Error: {mse_bayes}")
                logging.info(f"Mean Absolute Error: {mae_bayes}")
                logging.info(f"Root Mean Squared Error: {rmse_bayes}")
                logging.info(f"R-Squared: {r2_bayes}")
                logging.info(f"Explained Variance Score: {explained_variance}")

                # Saving feature importances plot
                feature_importances = best_model.feature_importances_
                fig, ax = plt.subplots()
                ax.barh(range(len(feature_importances)), feature_importances, align='center')
                ax.set_yticks(np.arange(len(X.columns)))
                ax.set_yticklabels(X.columns)

                # Save plots
                plot_directory = os.path.join(output_path, 'plots')
                os.makedirs(plot_directory, exist_ok=True)
                save_plots(best_model, X_train, y_train, X_test, y_test, plot_directory)

                # Specify the opset version supported by your ONNX Runtime
                target_opset_version = 15  # Adjust the opset version as needed

                # Define the initial types
                initial_type = [('float_input', FloatTensorType([None, X_train.shape[1]]))]

                # Convert the scikit-learn model to ONNX using the specified opset version
                onnx_model = convert_sklearn(best_model, initial_types=initial_type, target_opset=target_opset_version)

                # Save the ONNX model to a file
                onnx_model_path = os.path.join(output_path, 'AiSavingsModel_new15.onnx')
                with open(onnx_model_path, "wb") as f:
                    f.write(onnx_model.SerializeToString())
                logging.info(f"ONNX model saved to {onnx_model_path}")






        if param_grid_on:
            # Define the hyperparameters and their possible values
            param_grid = {
                'n_estimators': [50, 100, 150, 200, 250],
                'learning_rate': [0.005, 0.01, 0.05, 0.1, 0.2],
                'max_depth': [3, 4, 5, 6, 7, 8, 9, 10],
                'min_samples_split': [2, 3, 4, 5],
                'min_samples_leaf': [1, 2, 3, 4, 5],
            }

            # Initialize the Grid Search
            grid_search = GridSearchCV(estimator=gbm, param_grid=param_grid, cv=4, n_jobs=-1, verbose=2)
            grid_search.fit(X_train, y_train)
            best_params = grid_search.best_params_

            # after fitting the grid search, save the cv_results to a csv file
            cv_results = pd.DataFrame(grid_search.cv_results_)
            cv_results.to_csv(output_path, 'cv_results.csv', index=False)

            best_gbm = GradientBoostingRegressor(**best_params, random_state=42)
            best_gbm.fit(X_train, y_train)

            # save the best params
            joblib.dump(best_gbm, 'AiSavingsModel.pkl')

            # Evaluate the model
            y_pred = best_gbm.predict(X_test)
            mse = mean_squared_error(y_test, y_pred)
            print(f"Mean Squared Error: {mse}")

            # Define the initial types based on your input features.
            initial_type = [('float_input', FloatTensorType([None, len(X.columns)]))]

            # Convert and save the scikit-learn model to ONNX
            AiSavings_onnx_model = convert_sklearn(best_gbm, initial_types=initial_type, target_opset=15)

            with open("AiSavingsModel.onnx", "wb") as f:
                f.write(AiSavings_onnx_model.SerializeToString())

            # Save the best parameters to a file
            with open('AiSavingModelParams.txt', 'w') as f:
                f.write(str(best_params))

                # Plots
                # Learning Curve
            train_sizes, train_scores, test_scores = learning_curve(best_gbm, X, y, cv=3,
                                                                    scoring='neg_mean_squared_error',
                                                                    train_sizes=np.linspace(0.1, 1.0, 5))
            train_scores_mean = -train_scores.mean(axis=1)
            test_scores_mean = -test_scores.mean(axis=1)

            plt.figure()
            plt.plot(train_sizes, train_scores_mean, 'o-', color="r", label="Training score")

            plt.plot(train_sizes, test_scores_mean, 'o-', color="g", label="Cross-validation score")
            plt.title("Learning Curve")
            plt.xlabel("Training examples")
            plt.ylabel("MSE")
            plt.legend(loc="best")
            save_path = os.path.join(output_path, "learning_curve.png")
            plt.savefig(save_path)

            # Residual Plot
            y_pred_train = best_gbm.predict(X_train)
            y_pred_test = best_gbm.predict(X_test)

            plt.figure()
            plt.scatter(y_pred_train, y_pred_train - y_train, color="blue", s=10, label="Training data")
            plt.scatter(y_pred_test, y_pred_test - y_test, color="green", s=10, label="Test data")
            plt.hlines(y=0, xmin=y_pred_test.min(), xmax=y_pred_test.max(), color="red")
            plt.title("Residual Error Plot")
            plt.xlabel("Predicted Values")
            plt.ylabel("Residuals")
            plt.legend(loc="upper right")
            # everytime a plot is saved the 'residual_plot_{number} should increase by 1 to avoid overwriting
            save_path = os.path.join(output_path, "residual_plot_1.png")
            plt.savefig(save_path)

    #
    # if train_ai_savings_emergency:
    #     y = data['AISuggestedEmergencyDeposit']
    #     X = data.drop(columns=['AISuggestedEmergencyDeposit'])
    #     print(X.head())
    #     print(y.head())
    #
    #     X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    #
    #     # Fit the model to Gradiant Boosting algorithm
    #     gbm = GradientBoostingRegressor(random_state=42)
    #
    #     # Define the hyperparameters and their possible values
    #     param_grid = {
    #         'n_estimators': [150],  # [50, 100, 150, 200, 250],
    #         'learning_rate': [0.1],  # [0.005, 0.01, 0.05, 0.1, 0.2],
    #         'max_depth': [5],  # [3, 4, 5, 6, 7],
    #         'min_samples_split': [2],  # [2, 3, 4, 5],
    #         'min_samples_leaf': [1],  # [1, 2, 3, 4, 5],
    #     }
    #
    #     # Initialize the Grid Search
    #     grid_search = GridSearchCV(estimator=gbm, param_grid=param_grid, cv=3, n_jobs=-1, verbose=2)
    #     grid_search.fit(X_train, y_train)
    #     best_params = grid_search.best_params_
    #     best_gbm = GradientBoostingRegressor(**best_params, random_state=42)
    #     best_gbm.fit(X_train, y_train)
    #
    #     # Evaluate the model
    #     y_pred = best_gbm.predict(X_test)
    #     mse = mean_squared_error(y_test, y_pred)
    #     print(f"Mean Squared Error: {mse}")
    #
    #     # Define the initial types based on your input features.
    #     initial_type = [('float_input', FloatTensorType([None, len(X.columns)]))]
    #
    #     # Convert and save the scikit-learn model to ONNX
    #     AiSavings_onnx_model = convert_sklearn(best_gbm, initial_types=initial_type, target_opset=15)
    #
    #     with open("AiSavingsModel.onnx", "wb") as f:
    #         f.write(AiSavings_onnx_model.SerializeToString())
    #
    #     # Save the best parameters to a file
    #     with open('AiSavingModelParams.txt', 'w') as f:
    #         f.write(str(best_params))
    #
