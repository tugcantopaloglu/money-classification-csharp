# Banknote Classification and Distance Matrices

A C# console exercise from a data structures course. It generates random 2D points, prints distance matrices, classifies a supplied banknote with k-nearest neighbors, and evaluates the committed test dataset. The original variable names and console prompts are Turkish.

## Run

Install the .NET 8 SDK or a compatible newer SDK, then run from the repository root:

```sh
dotnet build MoneyClassification.csproj --configuration Release
dotnet run --project MoneyClassification.csproj --configuration Release --no-build
```

The program asks for variance, skewness, kurtosis, entropy, and the neighbor count `k`. It reads `data_banknote_authentication.txt` and `test_data.txt` from the working directory, so keep those files beside the working directory when launching a built binary elsewhere. Each CSV row contains four numeric features followed by the class label. File decimals use a period and are parsed independently of the machine's locale; interactive numeric input follows the current console culture.

Nearest neighbors are ranked by squared Euclidean distance across all four features. Equal distances retain all rows, with original row order as the deterministic tie-breaker. The neighbor count must be between one and the reference dataset's row count. A tied class vote selects label 1, matching the existing exercise convention.

The evaluation prints the prediction for each test row and the fraction of correctly classified rows. This is an educational classifier, not a validated currency authentication product. Features are not normalized and the exercise rounds loaded features to two decimal places.

## Tests

```sh
dotnet test tests/MoneyClassification.Tests/MoneyClassification.Tests.csproj --configuration Release
```

The regression suite covers actual nearest-row selection, equal-distance neighbors, use of all four features, invalid neighbor counts, locale-independent dataset parsing, and the console's per-row predictions and fractional accuracy. No network service or extra dataset is required.
