update TimeSheets set BasicSalary = (select EmployeeBasicSalaries.BaseSalary from EmployeeBasicSalaries where 
	EmployeeId=TimeSheets.EmployeeId) where OvertimeHours>0 and BasicSalary=0;

update TimeSheets set OvertimeAmount = (BasicSalary/30/8)*OvertimeHours*r.OvertimeMultiplier
from TimeSheets ts inner join Employees e on e.Id = ts.EmployeeId
inner join OvertimeRule r on r.Id = e.OvertimeMultiplierId
where OvertimeHours>0 and OvertimeAmount=0;
