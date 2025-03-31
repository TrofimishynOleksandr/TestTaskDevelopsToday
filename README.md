# Test task for DevelopsToday

## **Task Completion Summary**  

This project is a **CLI-based ETL tool** that imports taxi trip data from a CSV file into an **MS SQL Server** database, ensuring **data integrity, optimization, and efficient processing**.  

### **Key Implementations**  

- **Database Setup:**  
  - Created **`TaxiTrips`** table with appropriate data types and constraints.  
  - Added **indexes** to optimize required queries.  

- **Data Processing & Import:**  
  - Implemented **bulk insertion** for efficient data import.  
  - Removed **duplicates** based on `pickup_datetime`, `dropoff_datetime`, and `passenger_count` and stored them in `duplicates.csv`.  
  - Normalized `store_and_fwd_flag` values (`N → No`, `Y → Yes`).  
  - Trimmed leading and trailing whitespaces from text fields.  
  - Converted **timestamps from EST to UTC** before inserting into the database.  

- **Query Optimization:**  
  - Indexed **`PULocationID`** for optimized searches.  
  - Indexed **`TripDistance`** for fetching longest fares.  

### **Executed Queries**  

#### **1. Highest Average Tip by Pickup Location**  
```sql
SELECT TOP 1 PULocationID, AVG(TipAmount) AS AvgTip
FROM TaxiTrips
GROUP BY PULocationID
ORDER BY AvgTip DESC;
```
#### **2️. Top 100 Longest Trips (Distance-Based)**
```sql
SELECT TOP 100 * FROM TaxiTrips ORDER BY TripDistance DESC;
```
#### **3️. Top 100 Longest Trips (Time-Based)**
```sql
SELECT TOP 100 *, DATEDIFF(SECOND, PickupDatetime, DropoffDatetime) AS TripTime
FROM TaxiTrips ORDER BY TripTime DESC;
```
#### **4️. Search by Pickup Location ID**
```sql
SELECT * FROM TaxiTrips WHERE PULocationID = @LocationID;
```

### **Results**
After processing data, there are 29794 rows of data in database and 112 rows of duplicates.

### **Security Considerations**
This application is designed to handle **potentially unsafe data sources**. To ensure security, the following measures are implemented:

#### **SQL Injection Protection**  
- Uses **SqlBulkCopy** to prevent SQL injection.

#### **Data Validation & Sanitization**  
- Ensures correct data types, trims text fields, and normalizes `store_and_fwd_flag`.  
- Validates dates to prevent incorrect or malicious entries.

### **Scalability Considerations (10GB CSV Handling)**
For larger datasets, optimizations would include:
- Reading the CSV file in chunks instead of loading it all at once.
- Partitioning the database table for better query performance.
- Using parallel processing and streaming
