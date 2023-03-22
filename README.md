
# tSwitch - A simple tool to switch between Type Stream Visual databases

Type Stream Visual (TSV) uses Microsoft SQL Server Express as database backend. 
The upper limit of database size for the Express version of SQL Server is restricted to 10GB. 
Before databases reach 90% of the maximum size, TSV may start failing to analyse NGS runs with a non-descriptive error message "Analysis Error". 
The solution is to move on to a new database.
There is built-in utility in TSV to switch databases, but it may not work due to various reasons. 
This simple tool is to provide an easy way to switch between different databases.

