/*
Add missing leave carry forward details for Employee - C4754
*/
INSERT INTO LeaveCreditAndUtilization (EmployeeId,LeaveTypeId,NumberOfDays,AddedUtilized,CarryForward,AnnualLeaves,LeaveAccountingPeriodId)
VALUES ((SELECT Id FROM Employees WHERE EmployeeCode = 'C4754'),1,25,1,4,0,(SELECT Id FROM LeaveAccountingPeriods WHERE Name = '2022'))

/*
Delete Leave Credit And Utilization for deactivated Employee - C4763
*/
DELETE	FROM LeaveCreditAndUtilization 
WHERE	EmployeeId = (SELECT Id FROM Employees WHERE EmployeeCode = 'C4763') 
AND		LeaveAccountingPeriodId = (SELECT Id FROM LeaveAccountingPeriods WHERE Name = '2022')

/*
Set negative carry forward leaves to 0
*/
UPDATE LeaveCreditAndUtilization SET CarryForward = 0 WHERE CarryForward < 0