using System.Runtime.InteropServices;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var environmentInfo = new
{
    Description = "CrossApp – практикум з крос-платформного програмування",
    Name = "Бардин Олена",
    Group = "Група ФЕІ-36",
    OSDescription = RuntimeInformation.OSDescription,
    EnvironmentVer = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    DotNetVer = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    BaseDir = AppContext.BaseDirectory,
    CurrentDir = Environment.CurrentDirectory,
    Theme = "Бібліотека (видання, примірник, читач, видача)",
};

if (args.Contains("--json"))
{
    var optionsForJson = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    string json = JsonSerializer.Serialize(environmentInfo, optionsForJson);
    Console.WriteLine(json);
}
else
{
    Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
    Console.WriteLine("Студентка: Бардин Олена, група ФЕІ-36");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"ОС (OSDescription) : {RuntimeInformation.OSDescription}");
    Console.WriteLine($"ОС (Environment) : {Environment.OSVersion}");
    Console.WriteLine($"Архітектура процесу : {RuntimeInformation.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR) : {Environment.Version}");
    Console.WriteLine($"Runtime : {RuntimeInformation.FrameworkDescription}");
    Console.WriteLine($"Каталог застосунку : {AppContext.BaseDirectory}");
    Console.WriteLine($"Поточний каталог : {Environment.CurrentDirectory}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine("Предметна область: Бібліотека (видання, примірник, читач, видача)");
}

Console.ReadKey();