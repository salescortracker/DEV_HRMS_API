using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string? InvoiceNumber { get; set; }

    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public int? RegionId { get; set; }

    public int PlanId { get; set; }

    public decimal Amount { get; set; }

    public decimal? TaxPercentage { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }

    public string? PaymentId { get; set; }

    public string? OrderId { get; set; }

    public DateTime? BillingDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public bool? IsActive { get; set; }
}
