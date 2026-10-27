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


## Інваріанти

| # | Правило | Тип винятку | Метод |
|---|---------|-------------|-------|
| 1 | Ідентифікатор примірника не може бути порожнім | ArgumentException | BookCopy.Create |
| 2 | ISBN не може бути порожнім | ArgumentException | BookCopy.Create |
| 3 | Назва не може бути порожньою | ArgumentException | BookCopy.Create |
| 4 | Не можна видати примірник, який уже виданий | InvalidOperationException | BookCopy.Issue |
| 5 | Не можна повернути примірник, який не виданий | InvalidOperationException | BookCopy.Return |
| 6 | Ідентифікатор видачі не може бути порожнім | ArgumentException | Loan.Open |
| 7 | Ідентифікатор читача не може бути порожнім | ArgumentException | Loan.Open |
| 8 | Примірник не може бути null | ArgumentNullException | Loan.Open |
| 9 | Не можна відкрити видачу для вже виданого примірника | InvalidOperationException | Loan.Open |
| 10 | Не можна закрити видачу, яка вже закрита | InvalidOperationException | Loan.Close |
| 11 | Дата повернення не може бути раніше дати видачі | ArgumentOutOfRangeException | Loan.Close |

## Інтерфейс
Проєкт має два сховища з одним інтерфейсом IBookStore:

- InMemoryBookStore — дані лише в пам'яті, зникають після виходу
- FileBookStore — дані у файлі data/catalog.json

Назва сервісу - LendingService


## Типи подій

- Added — додано новий примірник
- Issued — видано читачу
- Returned — повернено


## Відкладене виконання LINQ
У Program.cs створюється запит `var query = source.Where(b => b.Year < 1900);`. 
Спочатку він не виконується. Перший виклик `query.Count()` повертає 3 книги. 
Коли ми додаємо нову книгу в `source`, наступний `query.Count()` повертає вже 4, 
бо запит виконується заново. Але після `query.ToList()` результат фіксується 
в пам'яті (snapshot), і навіть після `source.Clear()` у знімку залишаються ті самі 4 книги.

## Синтаксис LINQ
Для фільтрації та сортування я використовую методний синтаксис (Where, OrderBy), 
бо він коротший. Для групування в методі ByYear я використала
синтаксис запитів (from ... group ... into), він нагадує SQL і робить 
логіку групування зрозумілішою.

## Тести

Щоб запустити юніт-тести проєкту:
```bash
dotnet test 
```
Як перевірити скасування імпорту:
1. Запустіть команду імпорту з консолі: ```dotnet run --project src/Cli -- report-import```
2. Під час виконання імпорту натисніть Ctrl+C на клавіатурі