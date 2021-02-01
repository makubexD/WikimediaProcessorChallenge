# WikimediaProcessorChallenge

### Installation

1. Make sure pointing a correct DB, otherwise create one with the name Wikimedia
2. In order to create the tables and SP's, please run the following code in Package Manager Console (Default Project: **WikimediaProcessor.Data**)
```shell
update-database
```

### Todos

+ Write MORE Tests
+ The idea was to avoid it (run process to create scripts), this will be improved
+ To use BulkCopy or EF Core extension linq2db.EntityFrameworkCore, good results
+ To move it to web and avoid some issues from console due to .Net 5.0 such as: **(Web version in progress)**
    * This was added because of this issue: https://github.com/dotnet/efcore/issues/18116
    * DI in .NET Core in console app needs to handle its own scopes
