// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.Handler.#ctor")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.Handler.#ctor(AutoMapper.IMapper)")]
[assembly: SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Infrastructure.Interfaces.IGetEnvironmentVariables.GetPersonApiUrl~System.String")]
[assembly: SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Infrastructure.GetEnvironmentVariables.GetPersonApiUrl~System.String")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Infrastructure.EsServiceInitializer.ConfigureElasticSearch(Microsoft.Extensions.DependencyInjection.IServiceCollection,Microsoft.Extensions.Configuration.IConfiguration)")]
[assembly: SuppressMessage("Property Values", "DynamoDB1000:Property value too short", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Gateways.TenureGateway.GetAll(System.Collections.Generic.Dictionary{System.String,Amazon.DynamoDBv2.Model.AttributeValue})~System.Threading.Tasks.Task{FinanceDataMigrationApi.V1.Boundary.Response.TenurePaginationResponse}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Controllers.AssetController.SaveAllInInterimFinanceSystem~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.ExceptionMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.Controllers.ChargeController.ExtractChargeEntity~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.ExceptionMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:FinanceDataMigrationApi.V1.UseCase.LoadChargeEntityUseCase.ExecuteAsync(System.Int32)~System.Threading.Tasks.Task{FinanceDataMigrationApi.V1.Boundary.Response.StepResponse}")]
