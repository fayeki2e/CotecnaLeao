Update LeaveCreditAndUtilization 
Set NumberOfDays = 25, 
CarryForward=0, 
AnnualLeaves=25, 
LeaveAccountingPeriodId=1
Where LeaveTypeId=1 and 
EmployeeId = (select ID from Employees where EmployeeCode = 'C4400');
