IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [ApprovalLimitProfiles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Approval_Limit] float NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_ApprovalLimitProfiles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [EmployeeProfileId] int NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Clients] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Currencies] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [IsForex] bit NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Designations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Designations] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [EmailNotificationConfiguration] (
    [Id] int NOT NULL IDENTITY,
    [Type] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [TemplatePathText] nvarchar(max) NULL,
    [TemplatePathHtml] nvarchar(max) NULL,
    [SubjectLine] nvarchar(max) NULL,
    [Receiver] nvarchar(max) NULL,
    CONSTRAINT [PK_EmailNotificationConfiguration] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [ExpenseHeads] (
    [Id] int NOT NULL IDENTITY,
    [AccountNumber] nvarchar(max) NULL,
    [ExpenseHeadDesc] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_ExpenseHeads] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [ExpenseProfiles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_ExpenseProfiles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [FinancialYears] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_FinancialYears] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LeaveAccountingPeriods] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Active] bit NOT NULL,
    [NumberOfDaysOfLeave] int NOT NULL,
    [MaxCarryForwardFromLastYear] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveAccountingPeriods] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LeaveCategories] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveCategories] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LeaveRejectionReasons] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveRejectionReasons] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LeaveSubCategories] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveSubCategories] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LeaveTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Paid] bit NOT NULL,
    [Limit] bit NOT NULL,
    [Order] int NOT NULL,
    [IsMaternity] bit NOT NULL,
    [IsMission] bit NOT NULL,
    [IsCommon] bit NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveTypes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Locations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [OvertimeRule] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [OvertimeMultiplier] float NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_OvertimeRule] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [PaymentRequestRejectionReasons] (
    [Id] int NOT NULL IDENTITY,
    [Reason] nvarchar(max) NULL,
    [Type] nvarchar(max) NULL,
    CONSTRAINT [PK_PaymentRequestRejectionReasons] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [PaymentRequestSeriesSequence] (
    [Id] int NOT NULL IDENTITY,
    [AdvanceExpense] nvarchar(max) NULL,
    [LocationCode] nvarchar(max) NULL,
    [FinancialYear] nvarchar(max) NULL,
    [SequenceNumber] int NOT NULL,
    CONSTRAINT [PK_PaymentRequestSeriesSequence] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Jobs] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [ClientId] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Jobs_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [ExpenseHeadExpenseProfileMappings] (
    [Id] int NOT NULL IDENTITY,
    [ExpenseHeadId] int NOT NULL,
    [ExpenseProfileId] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_ExpenseHeadExpenseProfileMappings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExpenseHeadExpenseProfileMappings_ExpenseHeads_ExpenseHeadId] FOREIGN KEY ([ExpenseHeadId]) REFERENCES [ExpenseHeads] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExpenseHeadExpenseProfileMappings_ExpenseProfiles_ExpenseProfileId] FOREIGN KEY ([ExpenseProfileId]) REFERENCES [ExpenseProfiles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [LeaveTypeLeaveCategoryLeaveSubCategoryMapping] (
    [Id] int NOT NULL IDENTITY,
    [LeaveTypeId] int NOT NULL,
    [LeaveCategoryId] int NULL,
    [LeaveSubCategoryId] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LeaveTypeLeaveCategoryLeaveSubCategoryMapping] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveCategories_LeaveCategoryId] FOREIGN KEY ([LeaveCategoryId]) REFERENCES [LeaveCategories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveSubCategories_LeaveSubCategoryId] FOREIGN KEY ([LeaveSubCategoryId]) REFERENCES [LeaveSubCategories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [CustomerMarkets] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [LocationId] int NULL,
    [IsVOC] bit NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_CustomerMarkets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerMarkets_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Holidays] (
    [Id] int NOT NULL IDENTITY,
    [HolidayDate] datetime2 NOT NULL,
    [Reason] nvarchar(max) NULL,
    [IsHalfDay] bit NOT NULL,
    [LocationId] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Holidays] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Holidays_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [LocationOvertimeRule] (
    [Id] int NOT NULL IDENTITY,
    [LocationId] int NULL,
    [OvertimeMultiplier] float NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_LocationOvertimeRule] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LocationOvertimeRule_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [LocationWorkHours] (
    [Id] int NOT NULL IDENTITY,
    [LocationId] int NOT NULL,
    [DayOfWeek] int NOT NULL,
    [WorkingHours] float NOT NULL,
    [WorkDayType] int NOT NULL,
    CONSTRAINT [PK_LocationWorkHours] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LocationWorkHours_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [DesignationId] int NULL,
    [LocationId] int NULL,
    [AuthorizationProfileId] int NULL,
    [ExpenseProfileId] int NULL,
    [EmployeeCode] nvarchar(max) NULL,
    [AccountNumber] nvarchar(max) NULL,
    [ReportingToId] int NULL,
    [Email] nvarchar(max) NULL,
    [Gender] nvarchar(max) NULL,
    [DateOfJoining] datetime2 NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [CanCreateForexRequests] bit NOT NULL,
    [CanApplyMissionLeaves] bit NOT NULL,
    [OvertimeMultiplierId] int NULL,
    [CanHoldCreditCard] bit NOT NULL,
    [OnFieldEmployee] bit NOT NULL,
    [SpecificWeeklyOff] bit NOT NULL,
    [IsHr] bit NOT NULL,
    [LastWorkingDate] datetime2 NULL,
    [ResignationDate] datetime2 NULL,
    [SettlementDate] datetime2 NULL,
    [SettlementAmount] float NOT NULL,
    [Status] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_ApprovalLimitProfiles_AuthorizationProfileId] FOREIGN KEY ([AuthorizationProfileId]) REFERENCES [ApprovalLimitProfiles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_ExpenseProfiles_ExpenseProfileId] FOREIGN KEY ([ExpenseProfileId]) REFERENCES [ExpenseProfiles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_OvertimeRule_OvertimeMultiplierId] FOREIGN KEY ([OvertimeMultiplierId]) REFERENCES [OvertimeRule] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Employees_ReportingToId] FOREIGN KEY ([ReportingToId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [BusinessUnits] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [BUHeadId] int NULL,
    CONSTRAINT [PK_BusinessUnits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusinessUnits_Employees_BUHeadId] FOREIGN KEY ([BUHeadId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [EmployeeBasicSalaries] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [FromDate] datetime2 NOT NULL,
    [ToDate] datetime2 NOT NULL,
    [BaseSalary] float NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_EmployeeBasicSalaries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeBasicSalaries_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [EmployeeClaimSeries] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [FinancialYearId] int NOT NULL,
    [SerialNumber] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_EmployeeClaimSeries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeClaimSeries_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeClaimSeries_FinancialYears_FinancialYearId] FOREIGN KEY ([FinancialYearId]) REFERENCES [FinancialYears] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [EmployeeWeeklyOff] (
    [Id] int NOT NULL IDENTITY,
    [FormDate] datetime2 NOT NULL,
    [ToDate] datetime2 NOT NULL,
    [WeeklyOffDay] int NOT NULL,
    [OtherWeeklyOffDay] int NULL,
    [OtherWeeklyOffRule] int NULL,
    [EmployeeId] int NULL,
    CONSTRAINT [PK_EmployeeWeeklyOff] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeWeeklyOff_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [LeaveCreditAndUtilization] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [CreatedById] int NULL,
    [CreatedByEmployeeId] int NULL,
    [LeaveTypeId] int NOT NULL,
    [NumberOfDays] float NOT NULL,
    [AddedUtilized] int NOT NULL,
    [CarryForward] float NOT NULL,
    [AnnualLeaves] float NOT NULL,
    [AccrualDate] datetime2 NULL,
    [ApprovedDate] datetime2 NULL,
    [ExpiryDate] datetime2 NULL,
    [LeaveAccountingPeriodId] int NULL,
    CONSTRAINT [PK_LeaveCreditAndUtilization] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveCreditAndUtilization_Employees_CreatedByEmployeeId] FOREIGN KEY ([CreatedByEmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveCreditAndUtilization_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LeaveCreditAndUtilization_LeaveAccountingPeriods_LeaveAccountingPeriodId] FOREIGN KEY ([LeaveAccountingPeriodId]) REFERENCES [LeaveAccountingPeriods] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveCreditAndUtilization_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [LeaveDrafts] (
    [Id] int NOT NULL IDENTITY,
    [UserIdentity] nvarchar(max) NULL,
    [LastUpdatedOn] datetime2 NOT NULL,
    [EmployeeId] int NULL,
    [FormData] nvarchar(max) NULL,
    [Type] nvarchar(max) NULL,
    [UniqueId] nvarchar(max) NULL,
    CONSTRAINT [PK_LeaveDrafts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveDrafts_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PaymentRequestDraft] (
    [Id] int NOT NULL IDENTITY,
    [UserIdentity] nvarchar(max) NULL,
    [LastUpdatedOn] datetime2 NOT NULL,
    [EmployeeId] int NULL,
    [FormData] nvarchar(max) NULL,
    [Type] nvarchar(max) NULL,
    [DraftId] nvarchar(max) NULL,
    CONSTRAINT [PK_PaymentRequestDraft] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentRequestDraft_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [TimeSheets] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [TotalWorkHours] float NOT NULL,
    [OvertimeHours] float NOT NULL,
    [TotalWorkedHolidays] float NOT NULL,
    [BasicSalary] float NOT NULL,
    [OvertimeAmount] float NOT NULL,
    [CompOffs] float NOT NULL,
    [TimesheetCreatedById] int NULL,
    [TimesheetCreatedOn] datetime2 NOT NULL,
    [TimesheetApprovedById] int NULL,
    [ApprovedOn] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    [NumberOfDays] int NOT NULL,
    [WeekInMonth] int NOT NULL,
    [WeekInYear] int NOT NULL,
    CONSTRAINT [PK_TimeSheets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TimeSheets_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TimeSheets_Employees_TimesheetApprovedById] FOREIGN KEY ([TimesheetApprovedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TimeSheets_Employees_TimesheetCreatedById] FOREIGN KEY ([TimesheetCreatedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [BusinessActivities] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(max) NOT NULL,
    [IsVOC] bit NOT NULL,
    [BusinessUnitid] int NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_BusinessActivities] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusinessActivities_BusinessUnits_BusinessUnitid] FOREIGN KEY ([BusinessUnitid]) REFERENCES [BusinessUnits] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [LeaveRequests] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NULL,
    [CreatedByEmployeeId] int NULL,
    [LeaveNature] nvarchar(max) NOT NULL,
    [LeaveTypeId] int NULL,
    [LeaveCategoryId] int NULL,
    [LeaveSubCategoryId] int NULL,
    [LeaveAccountingPeriodId] int NULL,
    [RejectionReasonId] int NULL,
    [CompOffAgainstDateId] int NULL,
    [LeaveRequestCreatedDate] datetime2 NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [HalfDayStart] bit NOT NULL,
    [HalfDayEnd] bit NOT NULL,
    [Status] nvarchar(max) NULL,
    [NumberOfDays] float NOT NULL,
    [ActualStartDate] datetime2 NULL,
    [ActualEndDate] datetime2 NULL,
    [ActualHalfDayStart] bit NULL,
    [ActualHalfDayEnd] bit NULL,
    [LeaveRequestApprovedById] int NULL,
    [LeaveEligibility] float NOT NULL,
    [LeavesCarriedForward] float NOT NULL,
    [LeavesOpeningBalance] float NOT NULL,
    [LeavesAvailed] float NOT NULL,
    [LeavesPending] float NOT NULL,
    [LWPDays] float NOT NULL,
    [documentsPath] nvarchar(max) NULL,
    [DocumentsReceivedDate] datetime2 NULL,
    [DocumentsComment] nvarchar(max) NULL,
    [RejectionReasonOther] nvarchar(max) NULL,
    [CancellationReason] nvarchar(max) NULL,
    CONSTRAINT [PK_LeaveRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveRequests_LeaveCreditAndUtilization_CompOffAgainstDateId] FOREIGN KEY ([CompOffAgainstDateId]) REFERENCES [LeaveCreditAndUtilization] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_Employees_CreatedByEmployeeId] FOREIGN KEY ([CreatedByEmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_LeaveAccountingPeriods_LeaveAccountingPeriodId] FOREIGN KEY ([LeaveAccountingPeriodId]) REFERENCES [LeaveAccountingPeriods] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_LeaveCategories_LeaveCategoryId] FOREIGN KEY ([LeaveCategoryId]) REFERENCES [LeaveCategories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_Employees_LeaveRequestApprovedById] FOREIGN KEY ([LeaveRequestApprovedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_LeaveSubCategories_LeaveSubCategoryId] FOREIGN KEY ([LeaveSubCategoryId]) REFERENCES [LeaveSubCategories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveRequests_LeaveRejectionReasons_RejectionReasonId] FOREIGN KEY ([RejectionReasonId]) REFERENCES [LeaveRejectionReasons] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [TimesheetAttendanceRecord] (
    [Id] int NOT NULL IDENTITY,
    [TimeSheetId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [Client] nvarchar(max) NULL,
    [Job] nvarchar(max) NULL,
    [WorkDate] datetime2 NOT NULL,
    [TimeIn] time NOT NULL,
    [TimeOut] time NOT NULL,
    [WorkingTime] time NOT NULL,
    [ApprovedTime] time NOT NULL,
    [IsWeekend] bit NOT NULL,
    [IsHoliday] bit NOT NULL,
    [IsLeave] bit NOT NULL,
    [IsTravellingDay] bit NOT NULL,
    [HolidayReason] nvarchar(max) NULL,
    [IsHalfDay] bit NOT NULL,
    [IsHalfDayLeave] bit NOT NULL,
    CONSTRAINT [PK_TimesheetAttendanceRecord] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TimesheetAttendanceRecord_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TimesheetAttendanceRecord_TimeSheets_TimeSheetId] FOREIGN KEY ([TimeSheetId]) REFERENCES [TimeSheets] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [TimesheetCompOff] (
    [Id] int NOT NULL IDENTITY,
    [TimeSheetId] int NOT NULL,
    [CompOffDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    [ActionById] int NULL,
    [ActinbById] int NULL,
    [ActionDate] datetime2 NULL,
    CONSTRAINT [PK_TimesheetCompOff] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TimesheetCompOff_Employees_ActinbById] FOREIGN KEY ([ActinbById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TimesheetCompOff_TimeSheets_TimeSheetId] FOREIGN KEY ([TimeSheetId]) REFERENCES [TimeSheets] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [BusinessActivityCustomerMarketMapping] (
    [Id] int NOT NULL IDENTITY,
    [BusinessActivityId] int NOT NULL,
    [CustomerMarketId] int NOT NULL,
    [Deactivated] bit NOT NULL,
    CONSTRAINT [PK_BusinessActivityCustomerMarketMapping] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusinessActivityCustomerMarketMapping_BusinessActivities_BusinessActivityId] FOREIGN KEY ([BusinessActivityId]) REFERENCES [BusinessActivities] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BusinessActivityCustomerMarketMapping_CustomerMarkets_CustomerMarketId] FOREIGN KEY ([CustomerMarketId]) REFERENCES [CustomerMarkets] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [PaymentRequests] (
    [Id] int NOT NULL IDENTITY,
    [RequestNumber] nvarchar(max) NULL,
    [VersionNumber] int NULL,
    [EmployeeId] int NOT NULL,
    [Amount] float NOT NULL,
    [ForexAmount] float NOT NULL,
    [ExchangeRate] float NOT NULL,
    [CreditCard] bit NOT NULL,
    [INRAmount] float NOT NULL,
    [SettlementMode] nvarchar(max) NULL,
    [BalanceAmount] float NOT NULL,
    [PaidAmount] float NOT NULL,
    [ClaimedAmount] float NOT NULL,
    [CurrencyId] int NOT NULL,
    [FinancialYearId] int NOT NULL,
    [LocationId] int NOT NULL,
    [BusinessActivityId] int NULL,
    [CustomerMarketId] int NULL,
    [PaymentRequestCreatedDate] datetime2 NOT NULL,
    [PaymentRequestCreatedById] int NULL,
    [PaymentRequestActionedById] int NULL,
    [ActionDate] datetime2 NULL,
    [PostedById] int NULL,
    [PostedOn] datetime2 NULL,
    [PaidDate] datetime2 NULL,
    [Type] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NULL,
    [Comment] nvarchar(max) NOT NULL,
    [AdvancePaymentRequestId] int NULL,
    [RejectionReasonsId] int NULL,
    [RejectionReasonOther] nvarchar(max) NULL,
    [FromDate] datetime2 NULL,
    [ToDate] datetime2 NULL,
    [SupportingDocumentsReceivedDate] datetime2 NULL,
    [SupportingDocumentsComment] nvarchar(max) NULL,
    [SupportingDocumentsPath] nvarchar(max) NULL,
    [Settled] bit NOT NULL,
    CONSTRAINT [PK_PaymentRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentRequests_PaymentRequests_AdvancePaymentRequestId] FOREIGN KEY ([AdvancePaymentRequestId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_BusinessActivities_BusinessActivityId] FOREIGN KEY ([BusinessActivityId]) REFERENCES [BusinessActivities] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequests_CustomerMarkets_CustomerMarketId] FOREIGN KEY ([CustomerMarketId]) REFERENCES [CustomerMarkets] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequests_FinancialYears_FinancialYearId] FOREIGN KEY ([FinancialYearId]) REFERENCES [FinancialYears] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequests_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequests_Employees_PaymentRequestActionedById] FOREIGN KEY ([PaymentRequestActionedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_Employees_PaymentRequestCreatedById] FOREIGN KEY ([PaymentRequestCreatedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_Employees_PostedById] FOREIGN KEY ([PostedById]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PaymentRequests_PaymentRequestRejectionReasons_RejectionReasonsId] FOREIGN KEY ([RejectionReasonsId]) REFERENCES [PaymentRequestRejectionReasons] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [LeaveRequestActions] (
    [Id] int NOT NULL IDENTITY,
    [LeaveRequestId] int NOT NULL,
    [Action] nvarchar(max) NULL,
    [Timestamp] datetime2 NOT NULL,
    [ActionById] int NOT NULL,
    CONSTRAINT [PK_LeaveRequestActions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveRequestActions_Employees_ActionById] FOREIGN KEY ([ActionById]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LeaveRequestActions_LeaveRequests_LeaveRequestId] FOREIGN KEY ([LeaveRequestId]) REFERENCES [LeaveRequests] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AdvanceExpenseAdjustments] (
    [Id] int NOT NULL IDENTITY,
    [ExpenseId] int NULL,
    [AdvanceId] int NULL,
    [AdjustedAdvanceAmount] float NOT NULL,
    CONSTRAINT [PK_AdvanceExpenseAdjustments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AdvanceExpenseAdjustments_PaymentRequests_AdvanceId] FOREIGN KEY ([AdvanceId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AdvanceExpenseAdjustments_PaymentRequests_ExpenseId] FOREIGN KEY ([ExpenseId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PaymentRequestApprovalActions] (
    [Id] int NOT NULL IDENTITY,
    [PaymentRequestId] int NULL,
    [ActionById] int NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [Type] nvarchar(max) NULL,
    [Action] nvarchar(max) NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_PaymentRequestApprovalActions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentRequestApprovalActions_Employees_ActionById] FOREIGN KEY ([ActionById]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequestApprovalActions_PaymentRequests_PaymentRequestId] FOREIGN KEY ([PaymentRequestId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PaymentRequestLineItems] (
    [Id] int NOT NULL IDENTITY,
    [PaymentRequestId] int NULL,
    [ExpenseHeadId] int NOT NULL,
    [BusinessActivityId] int NOT NULL,
    [CustomerMarketId] int NOT NULL,
    [Amount] float NOT NULL,
    [CurrencyId] int NOT NULL,
    [ExpenseVoucherReferenceNumber] nvarchar(max) NULL,
    [VoucherDescription] nvarchar(max) NULL,
    [ExpenseDate] datetime2 NOT NULL,
    CONSTRAINT [PK_PaymentRequestLineItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentRequestLineItems_BusinessActivities_BusinessActivityId] FOREIGN KEY ([BusinessActivityId]) REFERENCES [BusinessActivities] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequestLineItems_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequestLineItems_CustomerMarkets_CustomerMarketId] FOREIGN KEY ([CustomerMarketId]) REFERENCES [CustomerMarkets] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequestLineItems_ExpenseHeads_ExpenseHeadId] FOREIGN KEY ([ExpenseHeadId]) REFERENCES [ExpenseHeads] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentRequestLineItems_PaymentRequests_PaymentRequestId] FOREIGN KEY ([PaymentRequestId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [PaymentRequestPaymentRecords] (
    [Id] int NOT NULL IDENTITY,
    [PaymentRequestId] int NOT NULL,
    [TransactionType] nvarchar(max) NULL,
    [EmployeeCode] nvarchar(max) NULL,
    [BenificiaryCode] nvarchar(max) NULL,
    [TransactionAmount] float NOT NULL,
    [BenificiaryName] nvarchar(max) NULL,
    [PaymentDetail1] nvarchar(max) NULL,
    [PaymentDetail2] nvarchar(max) NULL,
    [PaymentDetail3] nvarchar(max) NULL,
    [PaymentDetail4] nvarchar(max) NULL,
    [PaymentDetail5] nvarchar(max) NULL,
    [PaymentDetail6] nvarchar(max) NULL,
    [PaymentDetail7] nvarchar(max) NULL,
    [ChqTime] nvarchar(max) NULL,
    [TimeStamp] datetime2 NOT NULL,
    CONSTRAINT [PK_PaymentRequestPaymentRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentRequestPaymentRecords_PaymentRequests_PaymentRequestId] FOREIGN KEY ([PaymentRequestId]) REFERENCES [PaymentRequests] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_AdvanceExpenseAdjustments_AdvanceId] ON [AdvanceExpenseAdjustments] ([AdvanceId]);

GO

CREATE INDEX [IX_AdvanceExpenseAdjustments_ExpenseId] ON [AdvanceExpenseAdjustments] ([ExpenseId]);

GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

GO

CREATE INDEX [IX_BusinessActivities_BusinessUnitid] ON [BusinessActivities] ([BusinessUnitid]);

GO

CREATE INDEX [IX_BusinessActivityCustomerMarketMapping_BusinessActivityId] ON [BusinessActivityCustomerMarketMapping] ([BusinessActivityId]);

GO

CREATE INDEX [IX_BusinessActivityCustomerMarketMapping_CustomerMarketId] ON [BusinessActivityCustomerMarketMapping] ([CustomerMarketId]);

GO

CREATE INDEX [IX_BusinessUnits_BUHeadId] ON [BusinessUnits] ([BUHeadId]);

GO

CREATE INDEX [IX_CustomerMarkets_LocationId] ON [CustomerMarkets] ([LocationId]);

GO

CREATE INDEX [IX_EmployeeBasicSalaries_EmployeeId] ON [EmployeeBasicSalaries] ([EmployeeId]);

GO

CREATE INDEX [IX_EmployeeClaimSeries_EmployeeId] ON [EmployeeClaimSeries] ([EmployeeId]);

GO

CREATE INDEX [IX_EmployeeClaimSeries_FinancialYearId] ON [EmployeeClaimSeries] ([FinancialYearId]);

GO

CREATE INDEX [IX_Employees_AuthorizationProfileId] ON [Employees] ([AuthorizationProfileId]);

GO

CREATE INDEX [IX_Employees_DesignationId] ON [Employees] ([DesignationId]);

GO

CREATE INDEX [IX_Employees_ExpenseProfileId] ON [Employees] ([ExpenseProfileId]);

GO

CREATE INDEX [IX_Employees_LocationId] ON [Employees] ([LocationId]);

GO

CREATE INDEX [IX_Employees_OvertimeMultiplierId] ON [Employees] ([OvertimeMultiplierId]);

GO

CREATE INDEX [IX_Employees_ReportingToId] ON [Employees] ([ReportingToId]);

GO

CREATE INDEX [IX_EmployeeWeeklyOff_EmployeeId] ON [EmployeeWeeklyOff] ([EmployeeId]);

GO

CREATE INDEX [IX_ExpenseHeadExpenseProfileMappings_ExpenseHeadId] ON [ExpenseHeadExpenseProfileMappings] ([ExpenseHeadId]);

GO

CREATE INDEX [IX_ExpenseHeadExpenseProfileMappings_ExpenseProfileId] ON [ExpenseHeadExpenseProfileMappings] ([ExpenseProfileId]);

GO

CREATE INDEX [IX_Holidays_LocationId] ON [Holidays] ([LocationId]);

GO

CREATE INDEX [IX_Jobs_ClientId] ON [Jobs] ([ClientId]);

GO

CREATE INDEX [IX_LeaveCreditAndUtilization_CreatedByEmployeeId] ON [LeaveCreditAndUtilization] ([CreatedByEmployeeId]);

GO

CREATE INDEX [IX_LeaveCreditAndUtilization_EmployeeId] ON [LeaveCreditAndUtilization] ([EmployeeId]);

GO

CREATE INDEX [IX_LeaveCreditAndUtilization_LeaveAccountingPeriodId] ON [LeaveCreditAndUtilization] ([LeaveAccountingPeriodId]);

GO

CREATE INDEX [IX_LeaveCreditAndUtilization_LeaveTypeId] ON [LeaveCreditAndUtilization] ([LeaveTypeId]);

GO

CREATE INDEX [IX_LeaveDrafts_EmployeeId] ON [LeaveDrafts] ([EmployeeId]);

GO

CREATE INDEX [IX_LeaveRequestActions_ActionById] ON [LeaveRequestActions] ([ActionById]);

GO

CREATE INDEX [IX_LeaveRequestActions_LeaveRequestId] ON [LeaveRequestActions] ([LeaveRequestId]);

GO

CREATE INDEX [IX_LeaveRequests_CompOffAgainstDateId] ON [LeaveRequests] ([CompOffAgainstDateId]);

GO

CREATE INDEX [IX_LeaveRequests_CreatedByEmployeeId] ON [LeaveRequests] ([CreatedByEmployeeId]);

GO

CREATE INDEX [IX_LeaveRequests_EmployeeId] ON [LeaveRequests] ([EmployeeId]);

GO

CREATE INDEX [IX_LeaveRequests_LeaveAccountingPeriodId] ON [LeaveRequests] ([LeaveAccountingPeriodId]);

GO

CREATE INDEX [IX_LeaveRequests_LeaveCategoryId] ON [LeaveRequests] ([LeaveCategoryId]);

GO

CREATE INDEX [IX_LeaveRequests_LeaveRequestApprovedById] ON [LeaveRequests] ([LeaveRequestApprovedById]);

GO

CREATE INDEX [IX_LeaveRequests_LeaveSubCategoryId] ON [LeaveRequests] ([LeaveSubCategoryId]);

GO

CREATE INDEX [IX_LeaveRequests_LeaveTypeId] ON [LeaveRequests] ([LeaveTypeId]);

GO

CREATE INDEX [IX_LeaveRequests_RejectionReasonId] ON [LeaveRequests] ([RejectionReasonId]);

GO

CREATE INDEX [IX_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveCategoryId] ON [LeaveTypeLeaveCategoryLeaveSubCategoryMapping] ([LeaveCategoryId]);

GO

CREATE INDEX [IX_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveSubCategoryId] ON [LeaveTypeLeaveCategoryLeaveSubCategoryMapping] ([LeaveSubCategoryId]);

GO

CREATE INDEX [IX_LeaveTypeLeaveCategoryLeaveSubCategoryMapping_LeaveTypeId] ON [LeaveTypeLeaveCategoryLeaveSubCategoryMapping] ([LeaveTypeId]);

GO

CREATE INDEX [IX_LocationOvertimeRule_LocationId] ON [LocationOvertimeRule] ([LocationId]);

GO

CREATE INDEX [IX_LocationWorkHours_LocationId] ON [LocationWorkHours] ([LocationId]);

GO

CREATE INDEX [IX_PaymentRequestApprovalActions_ActionById] ON [PaymentRequestApprovalActions] ([ActionById]);

GO

CREATE INDEX [IX_PaymentRequestApprovalActions_PaymentRequestId] ON [PaymentRequestApprovalActions] ([PaymentRequestId]);

GO

CREATE INDEX [IX_PaymentRequestDraft_EmployeeId] ON [PaymentRequestDraft] ([EmployeeId]);

GO

CREATE INDEX [IX_PaymentRequestLineItems_BusinessActivityId] ON [PaymentRequestLineItems] ([BusinessActivityId]);

GO

CREATE INDEX [IX_PaymentRequestLineItems_CurrencyId] ON [PaymentRequestLineItems] ([CurrencyId]);

GO

CREATE INDEX [IX_PaymentRequestLineItems_CustomerMarketId] ON [PaymentRequestLineItems] ([CustomerMarketId]);

GO

CREATE INDEX [IX_PaymentRequestLineItems_ExpenseHeadId] ON [PaymentRequestLineItems] ([ExpenseHeadId]);

GO

CREATE INDEX [IX_PaymentRequestLineItems_PaymentRequestId] ON [PaymentRequestLineItems] ([PaymentRequestId]);

GO

CREATE INDEX [IX_PaymentRequestPaymentRecords_PaymentRequestId] ON [PaymentRequestPaymentRecords] ([PaymentRequestId]);

GO

CREATE INDEX [IX_PaymentRequests_AdvancePaymentRequestId] ON [PaymentRequests] ([AdvancePaymentRequestId]);

GO

CREATE INDEX [IX_PaymentRequests_BusinessActivityId] ON [PaymentRequests] ([BusinessActivityId]);

GO

CREATE INDEX [IX_PaymentRequests_CurrencyId] ON [PaymentRequests] ([CurrencyId]);

GO

CREATE INDEX [IX_PaymentRequests_CustomerMarketId] ON [PaymentRequests] ([CustomerMarketId]);

GO

CREATE INDEX [IX_PaymentRequests_EmployeeId] ON [PaymentRequests] ([EmployeeId]);

GO

CREATE INDEX [IX_PaymentRequests_FinancialYearId] ON [PaymentRequests] ([FinancialYearId]);

GO

CREATE INDEX [IX_PaymentRequests_LocationId] ON [PaymentRequests] ([LocationId]);

GO

CREATE INDEX [IX_PaymentRequests_PaymentRequestActionedById] ON [PaymentRequests] ([PaymentRequestActionedById]);

GO

CREATE INDEX [IX_PaymentRequests_PaymentRequestCreatedById] ON [PaymentRequests] ([PaymentRequestCreatedById]);

GO

CREATE INDEX [IX_PaymentRequests_PostedById] ON [PaymentRequests] ([PostedById]);

GO

CREATE INDEX [IX_PaymentRequests_RejectionReasonsId] ON [PaymentRequests] ([RejectionReasonsId]);

GO

CREATE INDEX [IX_TimesheetAttendanceRecord_EmployeeId] ON [TimesheetAttendanceRecord] ([EmployeeId]);

GO

CREATE INDEX [IX_TimesheetAttendanceRecord_TimeSheetId] ON [TimesheetAttendanceRecord] ([TimeSheetId]);

GO

CREATE INDEX [IX_TimesheetCompOff_ActinbById] ON [TimesheetCompOff] ([ActinbById]);

GO

CREATE INDEX [IX_TimesheetCompOff_TimeSheetId] ON [TimesheetCompOff] ([TimeSheetId]);

GO

CREATE INDEX [IX_TimeSheets_EmployeeId] ON [TimeSheets] ([EmployeeId]);

GO

CREATE INDEX [IX_TimeSheets_TimesheetApprovedById] ON [TimeSheets] ([TimesheetApprovedById]);

GO

CREATE INDEX [IX_TimeSheets_TimesheetCreatedById] ON [TimeSheets] ([TimesheetCreatedById]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200928030641_GoLive', N'2.2.6-servicing-10079');

GO

