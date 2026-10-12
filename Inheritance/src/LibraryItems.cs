using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
    public class LibraryItem
    {
        public string CatalogNumber { get; }
        public string Title { get; }
        public decimal BaseLateFee { get; private set; }
        public bool IsWithdrawn { get; private set; }
        public bool IsOnLoan { get; private set; }

        // Protected fields for child classes to define their behaviors without using abstract/virtual
        public int LoanPeriodDays { get; protected set; }
        protected decimal FeeMultiplier { get; set; }

        public decimal DailyLateFee
        {
            get
            {
                return BaseLateFee * FeeMultiplier;
            }
        }

        protected LibraryItem(string catalogNumber, string title, decimal baseLateFee)
        {
            if (string.IsNullOrWhiteSpace(catalogNumber))
                throw new ArgumentException("Catalog number cannot be empty.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (baseLateFee <= 0)
                throw new ArgumentException("Base late fee must be strictly positive.");

            CatalogNumber = catalogNumber;
            Title = title;
            BaseLateFee = baseLateFee;
            IsWithdrawn = false;
            IsOnLoan = false;
        }

        internal void SetBaseLateFee(decimal newFee)
        {
            if (newFee <= 0)
            {
                throw new ArgumentException("New late fee must be strictly positive.");
            }
            BaseLateFee = newFee;
        }

        internal void Withdraw()
        {
            IsWithdrawn = true;
        }

        internal void Restore()
        {
            IsWithdrawn = false;
        }

        internal void SetOnLoan(bool status)
        {
            IsOnLoan = status;
        }
    }

    public class Book : LibraryItem
    {
        public Book(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
            LoanPeriodDays = 21;
            FeeMultiplier = 1m;
        }
    }

    public class DVD : LibraryItem
    {
        public DVD(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
            LoanPeriodDays = 7;
            FeeMultiplier = 2m;
        }
    }

    public class Magazine : LibraryItem
    {
        public Magazine(string catalogNumber, string title, decimal baseLateFee)
            : base(catalogNumber, title, baseLateFee)
        {
            LoanPeriodDays = 3;
            FeeMultiplier = 0.5m;
        }
    }
}

