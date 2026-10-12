using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
    public class Loan
    {
        public string LoanId { get; }
        public DateTime BorrowDate { get; }
        public Member Member { get; }
        public LibraryItem Item { get; }
        public LoanStatus Status { get; private set; }
        public DateTime ReturnDate { get; private set; }

        public DateTime DueDate
        {
            get
            {
                return BorrowDate.AddDays(Item.LoanPeriodDays);
            }
        }

        // Internal constructor so outside code can't arbitrarily create loans.
        // Loans are created via Member.Borrow()
        internal Loan(string loanId, Member member, LibraryItem item, DateTime borrowDate)
        {
            if (string.IsNullOrWhiteSpace(loanId))
                throw new ArgumentException("Loan ID cannot be empty.");

            LoanId = loanId;
            Member = member;
            Item = item;
            BorrowDate = borrowDate;
            Status = LoanStatus.Borrowed;
        }

        internal void ReturnItem(DateTime returnDate)
        {
            if (Status != LoanStatus.Borrowed)
            {
                throw new InvalidOperationException("Only borrowed items can be returned.");
            }
            if (returnDate < BorrowDate)
            {
                throw new ArgumentException("Return date cannot be earlier than the borrow date.");
            }

            Status = LoanStatus.Returned;
            ReturnDate = returnDate;
            Item.SetOnLoan(false);
        }

        internal void MarkLost()
        {
            if (Status != LoanStatus.Borrowed)
            {
                throw new InvalidOperationException("Only borrowed items can be marked as lost.");
            }

            Status = LoanStatus.Lost;
            Item.SetOnLoan(false);
        }

        public decimal CalculateLateFee()
        {
            if (Status != LoanStatus.Returned)
            {
                return 0m;
            }

            if (ReturnDate <= DueDate)
            {
                return 0m;
            }

            int daysLate = (ReturnDate - DueDate).Days;
            decimal fee = daysLate * Item.DailyLateFee;

            if (Member.DiscountPercentage > 0)
            {
                decimal discountAmount = fee * (Member.DiscountPercentage / 100m);
                fee = fee - discountAmount;
            }

            if (fee < 0)
            {
                return 0m;
            }

            return fee;
        }
    }
}
