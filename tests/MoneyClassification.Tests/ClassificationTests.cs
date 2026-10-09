using System.Globalization;
using System.Reflection;
using PROJE_TEST;
using Xunit;

public class ClassificationTests
{
    [Fact]
    public void ConsoleEvaluationUsesEachPredictionAndReportsFractionalAccuracy()
    {
        var testDirectory = Directory.CreateTempSubdirectory("MoneyClassificationTests");
        var previousDirectory = Directory.GetCurrentDirectory();
        var previousInput = Console.In;
        var previousOutput = Console.Out;
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            File.WriteAllText(Path.Combine(testDirectory.FullName, "data_banknote_authentication.txt"),
                "0,0,0,0,0\n1,0,0,0,0\n2,0,0,0,0\n100,100,100,100,1\n101,100,100,100,1\n102,100,100,100,1");
            File.WriteAllText(Path.Combine(testDirectory.FullName, "test_data.txt"), "0,0,0,0,0\n100,100,100,100,0");
            Directory.SetCurrentDirectory(testDirectory.FullName);
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Console.SetIn(new StringReader("0\n0\n0\n0\n1"));
            using var output = new StringWriter();
            Console.SetOut(output);
            typeof(Program).GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, new object[] { Array.Empty<string>() });
            Assert.Contains("PARANIN TAHMİN EDİLEN TÜRÜ:1", output.ToString());
            Assert.Contains("BAŞARI ORANI:" + Environment.NewLine + "0.5", output.ToString());
        }
        finally
        {
            Console.SetIn(previousInput);
            Console.SetOut(previousOutput);
            CultureInfo.CurrentCulture = previousCulture;
            Directory.SetCurrentDirectory(previousDirectory);
            testDirectory.Delete(true);
        }
    }

    [Fact]
    public void NearestRowsAreChosenRegardlessOfInputOrder()
    {
        var rows = new double[,] { { 100, 100, 100, 100, 0 }, { 1, 1, 1, 1, 1 }, { 2, 2, 2, 2, 1 } };
        var result = Program.KNN(0, 0, 0, 0, 1, rows);
        Assert.Equal(1, result.Item2);
        Assert.Equal(1, result.Item1[0, 0]);
    }

    [Fact]
    public void EquidistantRowsRemainEligibleNeighbors()
    {
        var rows = new double[,] { { 1, 0, 0, 0, 1 }, { -1, 0, 0, 0, 1 }, { 0, 1, 0, 0, 0 } };
        Assert.Equal(1, Program.KNN(0, 0, 0, 0, 3, rows).Item2);
    }

    [Fact]
    public void EuclideanDistanceUsesAllFourFeatures()
    {
        var rows = new double[,] { { 3, 0, 0, 0, 0 }, { 2, 2, 0, 0, 1 } };
        Assert.Equal(1, Program.KNN(0, 0, 0, 0, 1, rows).Item2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void NeighborCountMustFitDataset(int k)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.KNN(0, 0, 0, 0, k, new double[2, 5]));
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("tr-TR")]
    public void DatasetDecimalParsingDoesNotDependOnMachineCulture(string culture)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var path = Path.GetTempFileName();
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            File.WriteAllText(path, "3.6216,8.6661,-2.8073,-0.44699,0");
            var rows = Program.ReadFile(path);
            Assert.Equal(3.62, rows[0, 0]);
            Assert.Equal(-2.81, rows[0, 2]);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            File.Delete(path);
        }
    }
}
