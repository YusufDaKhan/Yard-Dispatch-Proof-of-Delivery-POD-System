# Yard Dispatch & Proof-of-Delivery (POD) System

A bulk delivery tracker and Proof-of-Delivery (POD) platform built for hardware and building-supply yards, automating the end-to-end order-to-delivery pipeline from sales counter intake to on-site sign-off.

---

## Core Domain Flow

1. **Order Intake:** Sales desk logs a delivery ticket (customer, site address/pin, line items, required vehicle)—or a Python worker ingests an exported POS invoice CSV/PDF.
2. **Yard Queue & Load Verification:** Yard foreman assigns a truck and driver; loaders tick off each bulk item before marking the order as dispatched.
3. **Gate Pass & Transit:** The system generates a digital gate pass and flips the ticket status to `InTransit`.
4. **Proof of Delivery (POD):** At the drop-off site, the driver takes a photo of the offloaded materials, captures the recipient's name and signature, and logs GPS coordinates and a timestamp.
5. **Automated POD PDF & Reconciliation:** Python generates the final stamped POD PDF (photo + signature + item list) and flags any quantity discrepancies (e.g., 2 broken tiles) straight back to the store manager's dashboard.

---

## Tech Stack

| Layer | Tech Stack | Role in System |
| :--- | :--- | :--- |
| **Core Backend & API** | C# (.NET 8/9 ASP.NET Core Web API) | Orders, dispatch state machine, vehicle/driver assignments, and role-based auth. |
| **Database** | SQL Server | Stores delivery tickets, line items, vehicle capacities, and immutable POD audit logs. |
| **Driver & Yard Client** | TypeScript / JavaScript (React Native & Web) | Mobile-first screens for yard load checklists, camera photo uploads, and touch signatures. |
| **Reporting & PDF Worker** | Python (FastAPI, Pandas, WeasyPrint) | Generates stamped POD PDFs, ingests POS exports, and runs daily yard KPI analytics. |

---

## Build Roadmap

- [ ] **Phase 1 — C# REST API & State Machine:** Build the ASP.NET Core Web API and SQL Server schema enforcing strict status transitions (`Created` $\rightarrow$ `Assigned` $\rightarrow$ `Loaded` $\rightarrow$ `OutForDelivery` $\rightarrow$ `Delivered` / `Disputed`) plus vehicle capacity checks.
- [ ] **Phase 2 — TypeScript Yard & Driver Client:** Build mobile-friendly screens for checking off yard loads, capturing canvas signatures, and uploading site POD photos with offline queue support.
- [ ] **Phase 3 — Python POD Generator & Yard Analytics:** Build the Python service to compile each completed delivery into a branded PDF receipt and compute daily KPIs (yard turnaround time, deliveries per truck, discrepancy trends).
