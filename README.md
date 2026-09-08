# CrossApp

Проєкт з крос-платформного програмування.

Предметна область: Бібліотека

Сутності:
- Book - видання
- BookCopy - примірник книги
- Reader - читач
- Loan - видача

Призначення: облік видач примірників книг читачам і повернень.

## Запуск

```
dotnet build
dotnet run --project src/Cli
```

### Self-contained розмір каталогів publish:
- Windows (win-x64): 76 MB  
- Linux (linux-x64): 78 MB