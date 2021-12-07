using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceDataMigrationApi.V1.Domain
{
    public enum MigrationRunStatus 
    {
        ReadyToMigrate,
        ExtractStarted,
        ExtractInprogress,
        ExtractCompleted,
        ExtractFailed,
        TransformInprogress,
        TransformCompleted,
        LoadInprogress,
        LoadCompleted,
        LoadFailed
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        Tenure
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Rent,

        Charge,

        [Display(Name = "Grounds Maintenance")]
        GroundsMaintenance,

        [Display(Name = "Bank Payment")]
        BankPayment,

        [Display(Name = "Basic Rent (No VAT)")]
        BasicRentNoVAT, 

        [Display(Name = "Cleaning (Estate)")]
        CleaningEstate,   

        [Display(Name = "Landlord Lighting")]
        LandlordLighting,

        [Display(Name = "Debit / Credit Card")]
        DebitCreditCard,

        [Display(Name = "PayPoint/Post Office")]
        PayPointPostOffice,

        [Display(Name = "Cleaning (Block)")]
        CleaningBlock,    

        [Display(Name = "Contents Insurance")]
        ContentsInsurance,

        [Display(Name = "Tenants Levy")]
        TenantsLevy

    }

}
