using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
    public class Person
    {
        public string PersonId { get; }
        public string FullName { get; }
        public string PhoneNumber { get; }

        protected Person(string id, string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty.");

            PersonId = id;
            FullName = name;
            PhoneNumber = phone;
        }
    }

    // ==========================================
    // MEMBERS
    // ==========================================
    public class Member : Person
    {
        public int MaxItemsOnLoan { get; }
        public decimal DiscountPercentage { get; }

        private List<Loan> _loans = new List<Loan>();

        public IReadOnlyList<Loan> Loans
        {
            get
            {
                return _loans;
            }
        }

        protected Member(string id, string name, string phone, int maxItems, decimal discount)
            : base(id, name, phone)
        {
            MaxItemsOnLoan = maxItems;
            DiscountPercentage = discount;
        }

        public void Borrow(LibraryItem item, string loanId, DateTime borrowDate)
        {
            if (item.IsWithdrawn)
            {
                throw new InvalidOperationException("Cannot borrow a withdrawn item.");
            }
            if (item.IsOnLoan)
            {
                throw new InvalidOperationException("This item is already on loan.");
            }

            int activeLoans = 0;
            foreach (Loan loan in _loans)
            {
                if (loan.Status == LoanStatus.Borrowed)
                {
                    activeLoans++;
                }
            }

            if (activeLoans >= MaxItemsOnLoan)
            {
                throw new InvalidOperationException("Member has reached their maximum allowed active loans.");
            }

            Loan newLoan = new Loan(loanId, this, item, borrowDate);
            _loans.Add(newLoan);
            item.SetOnLoan(true);
        }
    }

    public class StudentMember : Member
    {
        public StudentMember(string id, string name, string phone)
            : base(id, name, phone, 3, 0m)
        {
        }
    }

    public class PremiumMember : Member
    {
        public PremiumMember(string id, string name, string phone, decimal discount)
            : base(id, name, phone, 10, discount)
        {
        }

        public int ReadingPoints
        {
            get
            {
                int points = 0;
                foreach (Loan loan in Loans)
                {
                    if (loan.Status == LoanStatus.Returned)
                    {
                        points += 5;
                    }
                }
                return points;
            }
        }
    }

    // ==========================================
    // STAFF
    // ==========================================
    public class StaffMember : Person
    {
        public DateTime HireDate { get; }
        public decimal Salary { get; private set; }
        protected decimal ResponsibilityAllowance { get; set; }

        public decimal MonthlyPay
        {
            get
            {
                return Salary + ResponsibilityAllowance;
            }
        }

        protected StaffMember(string id, string name, string phone, DateTime hireDate, decimal salary)
            : base(id, name, phone)
        {
            if (salary <= 0)
                throw new ArgumentException("Salary must be positive.");

            HireDate = hireDate;
            Salary = salary;
            ResponsibilityAllowance = 0m;
        }

        public void GiveRaise(decimal percentage)
        {
            if (percentage <= 0)
            {
                throw new ArgumentException("Raise percentage must be strictly positive.");
            }
            decimal raiseAmount = Salary * (percentage / 100m);
            Salary += raiseAmount;
        }
    }

    public class Librarian : StaffMember
    {
        public Librarian(string id, string name, string phone, DateTime hireDate, decimal salary)
            : base(id, name, phone, hireDate, salary)
        {
        }

        public void ProcessReturn(Loan loan, DateTime returnDate)
        {
            loan.ReturnItem(returnDate);
        }

        public void MarkAsLost(Loan loan)
        {
            loan.MarkLost();
        }
    }

    public class Shelver : StaffMember
    {
        public string Section { get; private set; }

        public Shelver(string id, string name, string phone, DateTime hireDate, decimal salary, string section)
            : base(id, name, phone, hireDate, salary)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Section cannot be empty.");

            Section = section;
        }

        public void Reassign(string newSection)
        {
            if (string.IsNullOrWhiteSpace(newSection))
                throw new ArgumentException("New section cannot be empty.");

            Section = newSection;
        }
    }

    public class HeadLibrarian : StaffMember
    {
        public HeadLibrarian(string id, string name, string phone, DateTime hireDate, decimal salary)
            : base(id, name, phone, hireDate, salary)
        {
            ResponsibilityAllowance = 400m; // Sets the shared protected field provided by StaffMember parent
        }

        public void AdjustBaseLateFee(LibraryItem item, decimal newFee)
        {
            item.SetBaseLateFee(newFee);
        }

        public void WithdrawItem(LibraryItem item)
        {
            item.Withdraw();
        }

        public void RestoreItem(LibraryItem item)
        {
            item.Restore();
        }
    }
}
