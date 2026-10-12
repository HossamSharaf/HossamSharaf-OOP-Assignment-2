using SrpLab;

Console.WriteLine("SrpLab — Refactored SRP Solution");
Console.WriteLine("=================================================");

// 1. Ward Domain
var census = new WardCensus();
var acuityCalc = new AcuityCalculator();
var pagerLog = new WardPagerLog();
var noteFormatter = new HandoffNoteFormatter();

int acuity = acuityCalc.Score(heartRate: 130, spo2: 89);
census.Assign(1, "p-88", acuity);
if (acuity >= 8) pagerLog.RecordCodeYellow(1);

Console.WriteLine(noteFormatter.Format(1, census.GetPatient(1), census.GetAcuity(1)));
Console.WriteLine(string.Join(" | ", pagerLog.Drain()));


// 2. Checkout Domain
var cart = new Cart();
var pricer = new CartPricer();
var couponParser = new MarketingCouponParser();
var paymentEncoder = new PaymentPayloadEncoder();

cart.AddLine("SKU-1", 40m, 2);
decimal subTotal = pricer.SubTotal(cart);
decimal discount = couponParser.CalculateDiscount("SAVE10", subTotal);
decimal grandTotal = pricer.GrandTotal(subTotal, discount, hasGiftWrap: true);

Console.WriteLine($"basket total={grandTotal} auth={paymentEncoder.Encode(grandTotal, "4242", cart.Lines.Count)}");


// 3. Support Ticket Domain
var ticketData = new SupportTicketData("T-1", "cannot login", "prod is down for me", DateTimeOffset.UtcNow);
var priorityScanner = new TicketPriorityScanner();
var slaCalc = new SlaCalculator();
var replyFormatter = new CustomerReplyFormatter();

string priority = priorityScanner.Scan(ticketData.Subject, ticketData.Body);
DateTimeOffset deadline = slaCalc.Deadline(ticketData.OpenedAt, priority);

Console.WriteLine(replyFormatter.Format(ticketData.Id, "Nora", priority, deadline));


// 4. Loan Domain
var loanApp = new LoanApplication(RequestedAmount: 60_000m, CreditScore: 640, EmploymentMonths: 4, HasCollateral: false);
var riskCalc = new RiskCalculator();
var checklist = new ComplianceChecklist();
var decisionFormatter = new DecisionLetterFormatter();

decimal score = riskCalc.Score(loanApp);
bool isEligible = riskCalc.IsEligible(score, loanApp.CreditScore);
var requiredDocs = checklist.GetRequiredDocs(loanApp, isEligible);

Console.WriteLine(decisionFormatter.Format("Omar", loanApp.RequestedAmount, score, isEligible, requiredDocs));


// 5. Course Enrollment Domain
var roster = new CourseRoster("SEF-101", capacity: 1);
var enrollmentProc = new EnrollmentProcessor();
var welcomeFormatter = new WelcomePacketFormatter();

Console.WriteLine(enrollmentProc.Register(roster, "a@mail.com"));
Console.WriteLine(enrollmentProc.Register(roster, "b@mail.com"));

bool isSeated = roster.Seated.Contains("b@mail.com");
int waitPosition = enrollmentProc.GetWaitlistPosition(roster, "b@mail.com");
Console.WriteLine(welcomeFormatter.Format(roster.CourseCode, "Bea", isSeated, waitPosition));


// 6. Kitchen Domain
var kitchenOrder = new KitchenOrder();
var allergenDetector = new AllergenDetector();
var prepEstimator = new PrepTimeEstimator();
var thermalFormatter = new ThermalTicketFormatter();

kitchenOrder.Add("Pasta", new[] { "wheat", "milk" }, 12);
var allergens = allergenDetector.Detect(kitchenOrder.Items);
int eta = prepEstimator.Estimate(kitchenOrder.Items, stations: 2, hasAllergies: allergens.Count > 0);

Console.WriteLine(thermalFormatter.Format(42, kitchenOrder.Items, eta, allergens));


// 7. Subscription Domain
var subData = new SubscriptionData("c-9", 99m, new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 1));
var prorationCalc = new ProrationCalculator();
var invoiceSeq = new InvoiceSequence();
var dunningFormatter = new DunningEmailFormatter();

subData.RegisterFailure();
decimal amount = prorationCalc.Calculate(subData.MonthlyPrice, subData.PeriodStart, subData.PeriodEnd, activeFrom: subData.PeriodStart);
string invoiceNumber = invoiceSeq.Next(subData.PeriodStart);

Console.WriteLine(dunningFormatter.Format("Sara", invoiceNumber, amount, new DateOnly(2026, 9, 20), subData.FailedPayments));


// 8. Warehouse Domain
var pickOrder = new PickOrder();
var inventoryAlloc = new InventoryAllocator();
var pathFinder = new WarehousePathFinder();
var scriptFormatter = new PickerScriptFormatter();

pickOrder.AddNeed("BOLT", "A", 3, 10, 7);
pickOrder.AddNeed("NUT", "B", 1, 5, 5);

var path = pathFinder.SortWalkingOrder(pickOrder.Lines);
var allocations = inventoryAlloc.Allocate(pickOrder.Lines);

Console.WriteLine(scriptFormatter.Format(path, allocations));


// 9. GradeBook Domain
var studentScores = new StudentScores();
var averager = new ScoreAverager();
var academicPolicy = new AcademicPolicy();
var transcriptFormatter = new TranscriptFormatter();

studentScores.Record("s1", 92);
studentScores.Record("s1", 88);

var s1Scores = studentScores.Get("s1");
decimal avg = averager.Calculate(s1Scores);
string letter = academicPolicy.GetLetter(avg);
bool honor = academicPolicy.IsHonorRoll(avg);

Console.WriteLine(transcriptFormatter.Format("s1", "Ali", avg, letter, honor));


// 10. Appointment Domain
var clinicSchedule = new ClinicSchedule();
var businessHours = new BusinessHoursPolicy(new TimeOnly(9, 0), new TimeOnly(17, 0), slotMinutes: 30);
var slotFinder = new SlotFinder();
var smsFormatter = new SmsReminderFormatter();

var slot = slotFinder.FindNext(DateTimeOffset.Parse("2026-09-21T08:00:00Z"), 48, clinicSchedule, businessHours);
if (slot is null) throw new InvalidOperationException("no slot");

clinicSchedule.Book(slot.Value);

Console.WriteLine(smsFormatter.Format(slot.Value, "0100"));
Console.WriteLine("Done. Separated responsibilities — preserved behavior.");
