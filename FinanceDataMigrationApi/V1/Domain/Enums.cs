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
        LoadFailed,
        NothingToMigrate
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

        [Display(Name = "Service Charges")]
        ServiceCharges,

        [Display(Name = "Housing Benefit")]
        HousingBenefit,

        [Display(Name = "Grounds Maintenance")]
        GroundsMaintenance,

        [Display(Name = "Bank Payment")]
        BankPayment,

        [Display(Name = "Basic Rent (No VAT)")]
        BasicRentNoVAT, 

        [Display(Name = "Cleaning (Estate)")]
        CleaningEstate,

        [Display(Name = "Direct Debit Unpaid ")]
        DirectDebitUnpaid,

        [Display(Name = "Landlord Lighting")]
        LandlordLighting,

        [Display(Name = "Debit / Credit Card")]
        DebitCreditCard,

        [Display(Name = "DSS Transfer")]
        DSSTransfer,

        [Display(Name = "Direct Debit")]
        DirectDebit,

        [Display(Name = "Tenants Levy")]
        TenantsLevy,

        [Display(Name = "PayPoint/Post Office")]
        PayPointPostOffice,

        [Display(Name = "Cleaning (Block)")]
        CleaningBlock,    

        [Display(Name = "Contents Insurance")]
        ContentsInsurance,


    }

}
