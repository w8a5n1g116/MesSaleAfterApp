//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MesSaleAfterApp.Models.OrbitModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class AfterWorkSchedule
    {
        public string AfterWorkScheduleId { get; set; }
        public Nullable<int> SerialNumber { get; set; }
        public string CustomerId { get; set; }
        public string ProductFamilyId { get; set; }
        public string Model { get; set; }
        public Nullable<System.DateTime> UsingDate { get; set; }
        public Nullable<System.DateTime> ComplaintTime { get; set; }
        public string ComplaintContent { get; set; }
        public string Protocol { get; set; }
        public Nullable<int> PlanningCycle { get; set; }
        public string SparePartsDelivery { get; set; }
        public string Aftermarket { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerContact { get; set; }
        public Nullable<double> TravelExpenses { get; set; }
        public string AfterSalesCall { get; set; }
        public string AfterSalesEvaluation { get; set; }
        public string Remark { get; set; }
        public string ProcessRecord { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductFamilyName { get; set; }
        public string AsResultEvaluation { get; set; }
        public string Ter_Customer_Com_RecordId { get; set; }
    }
}