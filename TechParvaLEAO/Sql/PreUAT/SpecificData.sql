UPDATE Employees Set Employees.SpecificWeeklyOff = 1 WHERE id IN (
SELECT id FROM Employees WHERE Employees.EmployeeCode IN ('C2901','C3017','C3554','C4251'));

INSERT INTO "EmployeeWeeklyOff" ("Id", "FormDate", "ToDate", "WeeklyOffDay", "EmployeeId", "OtherWeeklyOffDay", "OtherWeeklyOffRule") VALUES (1952, '2020-01-01 00:00:00.0000000', '2099-12-31 00:00:00.0000000', 0, 1447, 6, 2);
INSERT INTO "EmployeeWeeklyOff" ("Id", "FormDate", "ToDate", "WeeklyOffDay", "EmployeeId", "OtherWeeklyOffDay", "OtherWeeklyOffRule") VALUES (1953, '2020-01-01 00:00:00.0000000', '2099-12-31 00:00:00.0000000', 0, 1388, 6, 2);
INSERT INTO "EmployeeWeeklyOff" ("Id", "FormDate", "ToDate", "WeeklyOffDay", "EmployeeId", "OtherWeeklyOffDay", "OtherWeeklyOffRule") VALUES (1955, '2020-01-01 00:00:00.0000000', '2099-12-31 00:00:00.0000000', 0, 1478, 6, 2);
INSERT INTO "EmployeeWeeklyOff" ("Id", "FormDate", "ToDate", "WeeklyOffDay", "EmployeeId", "OtherWeeklyOffDay", "OtherWeeklyOffRule") VALUES (1956, '2020-01-01 00:00:00.0000000', '2099-12-31 00:00:00.0000000', 0, 1073, 6, 2);

--INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES ('17ef2223-ac23-4e94-b981-bb5c1b6d6602', 'FINANCE_MASTER');
--INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES ('8b1be333-8d8f-4d9b-8558-3c068acf2960', 'FINANCE_MASTER');

