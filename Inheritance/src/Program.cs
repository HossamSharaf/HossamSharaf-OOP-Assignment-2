namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. COMPILE-TIME CONSTRAINTS (Must Not Compile) ===");
            // The following lines are commented out because they would prevent compilation:

            // Cannot instantiate protected parent classes directly:
            // Person p = new Person("P1", "John", "555-1111");
            // Member m = new Member("M1", "Jane", "555-2222", 5, 0m);
            // StaffMember sm = new StaffMember("S1", "Bob", "555-3333", DateTime.Now, 2000m);
            // LibraryItem item = new LibraryItem("I1", "Generic", 1.0m);

            // Cannot modify get-only properties or private-set properties:
            // StudentMember student = new StudentMember("ST1", "Alice", "555-0000");
            // student.FullName = "Alice Smith"; 

            // Cannot bypass encapsulation to add items directly to the list:
            // student.Loans.Add(new Loan("L1", student, new Book("B1", "T", 1m), DateTime.Now)); 

            // Cannot modify internal state from outside:
            // Book book = new Book("B1", "Title", 1.0m);
            // book.IsOnLoan = true; 

            Console.WriteLine("Verified: Code protecting direct instantiation and modification is active.\n");

            // =========================================================================

            Console.WriteLine("=== 2. RUNTIME FAILURES: BORROWING RULES ===");
            StudentMember student = new StudentMember("ST1", "Student One", "555-1234");
            Book book1 = new Book("B001", "C# Basics", 1.0m);
            Book book2 = new Book("B002", "Advanced C#", 1.0m);
            Book book3 = new Book("B003", "Design Patterns", 1.0m);
            Book book4 = new Book("B004", "Refactoring", 1.0m);
            HeadLibrarian admin = new HeadLibrarian("HL1", "Admin", "555-9999", DateTime.Now, 3000m);
            Librarian lib = new Librarian("L1", "Lib One", "555-8888", DateTime.Now, 2500m);

            // Rule: Borrow a withdrawn item
            admin.WithdrawItem(book1);
            try
            {
                student.Borrow(book1, "LN001", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Rejected] Borrow Withdrawn Item: {ex.Message}");
            }

            // Rule: Borrow an item already on loan
            admin.RestoreItem(book1);
            student.Borrow(book1, "LN001", DateTime.Now); // Successfully borrows
            StudentMember student2 = new StudentMember("ST2", "Student Two", "555-0000");
            try
            {
                student2.Borrow(book1, "LN002", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Rejected] Borrow Item Already On Loan: {ex.Message}");
            }

            // Rule: Student borrows a 4th item (limit is 3)
            student.Borrow(book2, "LN003", DateTime.Now); // 2nd item
            student.Borrow(book3, "LN004", DateTime.Now); // 3rd item
            try
            {
                student.Borrow(book4, "LN005", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Rejected] Exceed Loan Limit: {ex.Message}");
            }

            Console.WriteLine();

            // =========================================================================

            Console.WriteLine("=== 3. POLYMORPHISM: STAFF PAY ===");
            List<StaffMember> staffList = new List<StaffMember>();
            staffList.Add(new Librarian("S01", "Anna Librarian", "111", DateTime.Now, 2500m));
            staffList.Add(new HeadLibrarian("S02", "Mark Head", "222", DateTime.Now, 3000m));
            staffList.Add(new Shelver("S03", "Tom Shelver", "333", DateTime.Now, 2000m, "Fiction"));

            foreach (StaffMember staff in staffList)
            {
                Console.WriteLine($"Role/Name: {staff.FullName,-15} | Monthly Pay: {staff.MonthlyPay:C}");
            }

            Console.WriteLine();

            // =========================================================================

            Console.WriteLine("=== 4. POLYMORPHISM: ITEM RULES ===");
            List<LibraryItem> catalog = new List<LibraryItem>();
            catalog.Add(new Book("BK1", "The Great Gatsby", 2.0m));
            catalog.Add(new DVD("DV1", "Inception", 2.0m));
            catalog.Add(new Magazine("MG1", "Time Magazine", 2.0m));

            foreach (LibraryItem item in catalog)
            {
                Console.WriteLine($"Item: {item.Title,-18} | Loan Period: {item.LoanPeriodDays,2} days | Daily Late Fee: {item.DailyLateFee:C}");
            }

            Console.WriteLine();

            // =========================================================================

            Console.WriteLine("=== 5. PREMIUM MEMBER CALCULATIONS ===");
            // Base fee 2.0m. DVD multiplier is 2. Daily fee is 4.0m.
            DVD movie = new DVD("DV2", "The Matrix", 2.0m);
            PremiumMember premium = new PremiumMember("PM1", "Sarah Premium", "999", 10m); // 10% discount

            // DVD loan period is 7 days. If borrowed 12 days ago, it was due 5 days ago (so it is 5 days late).
            DateTime borrowDate = DateTime.Now.AddDays(-12);
            premium.Borrow(movie, "LN006", borrowDate);
            Loan lateLoan = premium.Loans[0];

            lib.ProcessReturn(lateLoan, DateTime.Now);

            Console.WriteLine($"Item Borrowed:      {movie.Title}");
            Console.WriteLine($"Due Date was:       {lateLoan.DueDate.ToShortDateString()}");
            // Fee: 5 days * 4.0m = 20.0m. 10% discount = -2.0m. Total = 18.0m.
            Console.WriteLine($"Calculated Fee:     {lateLoan.CalculateLateFee():C}");
            Console.WriteLine($"Reading Points:     {premium.ReadingPoints}");

            Console.WriteLine();

            // =========================================================================

            Console.WriteLine("=== 6. RUNTIME FAILURES: STATUS CHANGES ===");
            // Rule: Return the same loan twice
            try
            {
                lib.ProcessReturn(lateLoan, DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Rejected] Double Return: {ex.Message}");
            }

            // Rule: Mark a returned loan as lost
            try
            {
                lib.MarkAsLost(lateLoan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Rejected] Mark Returned as Lost: {ex.Message}");
            }

            Console.WriteLine("\nAll tests completed. Press any key to exit.");
            Console.ReadKey();
        }
    }

    }

