#scaffold database
dotnet ef dbcontext scaffold "Server=.;Database=Exam;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Model