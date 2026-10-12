# Single Responsibility Principle (SRP) Code Smells

## Overview
This repository contains a collection of C# classes that intentionally violate the **Single Responsibility Principle (SRP)**. Each class acts as a "God Class" for its respective domain, improperly mixing core business logic with presentation concerns, external integrations, hardware formatting, and unrelated operational policies. 

This document analyzes the distinct responsibilities hidden within each class and explains why keeping them coupled together creates brittle, hard-to-maintain code.

---

## Class Analysis

### 1. `WardBoard`
* **Responsibilities:**
  1. **State Management:** Tracking which patient is in which bed.
  2. **Clinical Policy:** Calculating the `ScoreAcuity` based on medical vitals.
  3. **Alerting/Ops:** Triggering Code Yellows and managing the pager log.
  4. **Presentation/UI:** Formatting the `BuildHandoffNote` string.
  5. **Data Export:** Serializing the state into `ExportCensusCsv`.
* **The Problem:** A change in how nurses want to read the handoff note, or an IT request to add a new column to the CSV, forces a change to the same file that handles critical clinical acuity logic. A bug introduced by formatting a string could accidentally break patient alerting.

### 2. `WarehousePickList`
* **Responsibilities:**
  1. **Order State:** Holding the items needed and inventory on-hand.
  2. **Allocation Math:** Determining how many items can actually be fulfilled.
  3. **Routing/Pathing:** Sorting the `WalkingOrder` based on physical warehouse layout.
  4. **Device UI:** Generating the text instructions for the handheld scanners.
  5. **System Integration:** Formatting the `WmsXmlBatch` string for a third-party API.
* **The Problem:** The warehouse physical layout (which determines the walking path algorithm) changes completely independently of the XML contract dictated by the WMS vendor. Mixing these means an upgrade to the third-party software requires touching the physical routing logic.

### 3. `SupportTicket`
* **Responsibilities:**
  1. **State Management:** Tracking the ticket's core data.
  2. **NLP/Keyword Parsing:** Scanning text to guess priority.
  3. **SLA Business Rules:** Calculating operational deadlines.
  4. **Customer Comms:** Drafting the tone and wording of public replies.
  5. **Internal Ops:** Formatting internal escalation blurbs.
* **The Problem:** Customer Experience (CX) teams constantly tweak the tone of public replies, while Operations dictates SLA rules. Grouping these creates merge conflicts among different departments and risks breaking strict SLA math just to fix a typo in an apology template.

### 4. `SubscriptionBilling`
* **Responsibilities:**
  1. **State Management:** Storing the subscription timeline and price.
  2. **Financial Math:** Calculating prorated billing amounts.
  3. **ID Generation:** Maintaining static sequence state to create invoice numbers.
  4. **Customer Comms:** Writing collections/dunning emails (with hidden side-effects).
  5. **Data Export:** Formatting ledger lines for accounting software.
* **The Problem:** Proration is a strict accounting rule, while dunning emails involve customer retention strategies. Because the email generator mutates invoice state, generating an email preview would completely corrupt the financial sequence records.

### 5. `LoanDesk`
* **Responsibilities:**
  1. **State Management:** Holding application data.
  2. **Risk Modeling:** Calculating the financial risk score and eligibility.
  3. **Compliance Policy:** Determining the required documents checklist.
  4. **Customer Comms:** Writing the text for the decision letter.
  5. **Data Export:** Generating CSV rows for analytics.
* **The Problem:** Risk committees update financial formulas, compliance officers update documentation rules, and legal departments update letter wording. Having them in one class means a developer updating a CSV export schema might accidentally alter the core underwriting formula.

### 6. `KitchenTicket`
* **Responsibilities:**
  1. **State Management:** Tracking food items and ingredients.
  2. **Health/Safety Rules:** Parsing ingredients to detect allergens.
  3. **Kitchen Ops:** Calculating estimated prep times based on parallel capacity.
  4. **Hardware Formatting:** Controlling character widths for thermal receipt printers.
  5. **Routing:** Generating the expeditor lane hint.
* **The Problem:** The FDA dictates allergen definitions, while a printer manufacturer dictates that thermal receipts are 32 characters wide. Buying a new kitchen printer should never require opening the file responsible for keeping customers with food allergies safe.

### 7. `GradeBook`
* **Responsibilities:**
  1. **State Management:** Storing student scores.
  2. **Math:** Calculating averages.
  3. **Academic Policy:** Mapping averages to letter grades and honor roll eligibility.
  4. **Document Generation:** Formatting plain-text transcripts.
  5. **Data Export:** Generating CSV exports.
* **The Problem:** The faculty senate changes grading bands, but the IT department is responsible for the CSV layout. Mixing these makes the system rigid and hard to test; you cannot test academic rules without also instantiating formatting logic.

### 8. `CourseEnrollmentDesk`
* **Responsibilities:**
  1. **State Management:** Managing the seated list and waitlist.
  2. **Enrollment Rules:** Moving students between lists.
  3. **Marketing/Comms:** Generating Markdown for welcome packets.
  4. **Finance/Taxes:** Calculating VAT and formatting tuition invoices.
* **The Problem:** Seating capacity is an operational concern, welcome packets are a student-relations concern, and VAT calculation is a legal/financial concern. A change to the tax code requires editing the exact same class that holds a Discord invite link.

### 9. `CheckoutBasket`
* **Responsibilities:**
  1. **Cart State:** Tracking lines, quantities, and flags.
  2. **Pricing Math & Parsing:** Subtotals, parsing marketing codes, and calculating discounts.
  3. **Customer Comms:** Writing the text for gift cards.
  4. **Security/Integration:** Hashing payloads for payment gateway authorization.
* **The Problem:** Payment gateway authorization is highly sensitive code that rarely changes, whereas marketing coupons and gift card text change weekly. Parsing raw strings inside core financial math is also highly brittle.

### 10. `AppointmentDesk`
* **Responsibilities:**
  1. **State Management:** Tracking booked slots.
  2. **Calendar Math & HR Rules:** Determining business hours and finding slots.
  3. **Interoperability:** Serializing data to the `.ics` RFC format.
  4. **Customer Comms:** Writing text for SMS reminders.
* **The Problem:** The `.ics` calendar format is an RFC standard that rarely changes, while SMS reminder copy might change frequently to reduce no-shows. If the clinic decides to change its operating hours, developers shouldn't have to navigate past SMS text templates to update the core schedule logic.

---

## Conclusion
To refactor these classes and adhere to the Single Responsibility Principle, each domain concern (e.g., UI formatting, data export, external integration, business logic) should be extracted into its own dedicated class or service.