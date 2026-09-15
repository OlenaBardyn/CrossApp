# CrossApp

Проєкт з крос-платформного програмування.

Предметна область: Бібліотека

Сутності:
- Book - видання
- BookCopy - примірник книги
- Reader - читач
- Loan - видача

Призначення: облік видач примірників книг читачам і повернень.

## Команди

```
dotnet build
dotnet run --project src/Cli

dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```



## Структура solution

```
CrossApp/
 CrossApp.sln
 README.md
 .gitignore
 src/
     Core/
         Core.csproj
         EnvironmentInfo.cs
         Dto/  -  record-типи формату даних (тиждень 3)
             BookDto
         Domain/  –  сутності з поведінкою та інваріантами (тиждень 4)
         Storage/ – реалізації сховищ (тиждень 5)
     Cli/
         Cli.csproj
         Program.cs
```


## Порівняння режимів публікації

| RID | Режим | Розмір publish | Потрібен runtime |
|-----|-------|----------------|------------------|
| win-x64 | self-contained | ~76 МБ | ні |
| win-x64 | framework-dependent | ~0.2 МБ | так (.NET 10) |
| linux-x64 | self-contained | ~78 МБ | ні |