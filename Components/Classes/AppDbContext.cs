using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

using Microsoft.EntityFrameworkCore;

namespace ERPClinic.Components.Classes
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<ExpenseID> ExpensesID { get; set; }
    }

    public class DBSelection
    {
        public bool EF_Enabled { get; set; } = false;

    }

    [Table("Calendar")]
    public class CalendarEvent
    {
        public int id { get; set; }
        public string? Title { get; set; }
        public DateTime DateTime { get; set; }
        public string? Details { get; set; }
        public int ClientID { get; set; }
        public decimal? Amount { get; set; }
        public string? Paid { get; set; }
        public decimal? Received { get; set; }
        public string? Notes { get; set; }
        public virtual Clients? Client { get; set; }

    }


    [Table("Clients")]
    public class Clients
    {
        public int id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Sex { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? ClientStatus { get; set; }
        public string? Odontograma { get; set; }
    }


    [Table("Expenses")]
    public class Expenses
    {
        public int id { get; set; }
        public string? Notes { get; set; }
        public int ExpenseID { get; set; }
        public string? Date { get; set; }
        public decimal? Amount { get; set; }
        public string? Paid { get; set; }
        public virtual ExpenseID? Expense { get; set; }
    }


    [Table("ExpenseID")]
    public class ExpenseID
    {
        public int id { get; set; }
        public required string Expense { get; set; }
        public string? Despesa { get; set; }

    }

    [Table("ListsMaterials")]
    public class ListsMaterials
    {
        public int id { get; set; }
        public string? Notes { get; set; }
        public int MaterialID { get; set; }
        public decimal? Value { get; set; }
        public virtual MaterialID? Material { get; set; }
    }


    [Table("MaterialID")]
    public class MaterialID
    {
        public int id { get; set; }
        public required string Description { get; set; }
        public string? Desricao { get; set; }
        public decimal? Value { get; set; }

    }

    [Table("ListsMaterialsDetails")]
    public class ListsMaterialsDetails
    {
        public int id { get; set; }
        public required string Name { get; set; }
        public int ListMaterialID { get; set; }
        public string? Notes { get; set; }
        public decimal? Total { get; set; }
        public DateOnly? Date { get; set; }
        public virtual ListsMaterials? ListsMaterial { get; set; }
    }
}
